using System.Text.Json;
using System.Text.Json.Serialization;

namespace InputDisplay.Config;

public sealed class GameConfigManager : IDisposable
{
    readonly FileSystemWatcher watcher = new();

    static readonly JsonSerializerOptions jsonOptions = new()
    {
        Converters =
        {
            new JsonStringEnumConverter(),
        },
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    const string FileName = "config.json";
    bool watching;
    DateTime lastSave = DateTime.MinValue;

    public GameConfig CurrentConfig { get; private set; }

    public GameConfigManager()
    {
        CurrentConfig = Load();
        ConfigureWatcher();
    }

    public void StartWatch() => watching = true;
    public void PauseWatch() => watching = false;

    void ConfigureWatcher()
    {
        try
        {
            watcher.Filter = $"*{FileName}";
            watcher.Path = Directory.GetCurrentDirectory();
            watcher.EnableRaisingEvents = true;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Created += WatcherHandler;
            watcher.Changed += WatcherHandler;
            watcher.Renamed += WatcherHandler;
            watcher.Deleted += WatcherHandler;
        }
        catch (Exception e)
        {
#if DEBUG
            throw;
#endif
#pragma warning disable CS0162 // Unreachable code detected
            Log.Error("File watch failure", e);
#pragma warning restore CS0162 // Unreachable code detected
        }
    }

    bool WasInternalSaved(string path) => File.GetLastWriteTimeUtc(path) <= lastSave;

    void WatcherHandler(object sender, FileSystemEventArgs e)
    {
        if (!watching || e.Name is null || File.GetAttributes(e.FullPath.TrimEnd('~'))
                .HasFlag(FileAttributes.Directory))
            return;

        if (!string.Equals(e.Name, FileName, StringComparison.OrdinalIgnoreCase))
            return;

        if (WasInternalSaved(e.FullPath))
        {
            Log.Info("Skipping reload");
            return;
        }

        ReloadFromDisk();
    }

    public void Dispose()
    {
        watcher.Created -= WatcherHandler;
        watcher.Deleted -= WatcherHandler;
        watcher.Changed -= WatcherHandler;
        watcher.Renamed -= WatcherHandler;
    }

    public void Save()
    {
        Log.Info("Saving config...");
        PauseWatch();
        try
        {
            var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(CurrentConfig, jsonOptions);
            File.WriteAllBytes(FileName, jsonBytes);
            lastSave = File.GetLastWriteTimeUtc(FileName);
        }
        catch (Exception e)
        {
            Log.Error("Fail to save configuration", e);
            throw;
        }
        finally
        {
            StartWatch();
        }
    }

    public GameConfig CreateFile()
    {
        GameConfig config = new();
        Save();
        return config;
    }

    public GameConfig Load()
    {
        if (!File.Exists(FileName))
        {
            Log.Info("No config file found. Creating new");
            return CreateFile();
        }

        try
        {
            Log.Info("Loading config file");
            var content = File.ReadAllBytes(FileName);
            if (JsonSerializer.Deserialize<GameConfig>(content, jsonOptions) is { } config)
                return config;
        }
        catch (Exception ex)
        {
#if DEBUG
            throw;
#endif
#pragma warning disable CS0162 // Unreachable code detected
            Log.Error("Fail to load configuration", ex);
            File.Delete(FileName);
#pragma warning restore CS0162 // Unreachable code detected
        }

        return CreateFile();
    }

    void ReloadFromDisk()
    {
        try
        {
            if (!File.Exists(FileName))
            {
                Log.Info("No config file for reload. Recreating");
                Save();
                return;
            }

            Log.Info("Reloading config from disk");
            var content = File.ReadAllBytes(FileName);
            if (JsonSerializer.Deserialize<GameConfig>(content, jsonOptions) is { } newConfig)
                CurrentConfig.CopyFrom(newConfig);
        }
        catch (Exception ex)
        {
#if DEBUG
            throw;
#endif
#pragma warning disable CS0162 // Unreachable code detected
            Log.Error("Fail to reload configuration", ex);
#pragma warning restore CS0162 // Unreachable code detected
        }
    }
}
