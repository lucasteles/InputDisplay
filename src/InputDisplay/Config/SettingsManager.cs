namespace InputDisplay.Config;

public sealed class SettingsManager : IDisposable
{
    readonly bool fileWatch;
    readonly FileSystemWatcher watcher = new();

    const string FileName = "settings.json";
    DateTime lastFileSaved = DateTime.MinValue;
    readonly TimeSpan saveCooldown = TimeSpan.FromSeconds(2);
    DateTime saveThreshold = DateTime.MinValue;
    bool pendingSave;
    bool watching;

    public Settings CurrentConfig { get; private set; }

    public SettingsManager(bool fileWatch = true)
    {
        this.fileWatch = fileWatch;
        CurrentConfig = Load();
        ConfigureWatcher();
    }

    public void StartWatch() => watching = true;
    public void PauseWatch() => watching = false;

    void ConfigureWatcher()
    {
        try
        {
            if (!fileWatch) return;
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

    bool WasInternalSaved(string path) => File.GetLastWriteTimeUtc(path) <= lastFileSaved;

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
        if (!fileWatch) return;
        watcher.Created -= WatcherHandler;
        watcher.Deleted -= WatcherHandler;
        watcher.Changed -= WatcherHandler;
        watcher.Renamed -= WatcherHandler;
    }

    public void Save()
    {
        saveThreshold = DateTime.UtcNow + saveCooldown;
        pendingSave = true;
    }

    public void SaveFile()
    {
        Log.Info("Saving config...");
        PauseWatch();
        try
        {
            lastFileSaved = Json.SerializeToFile(CurrentConfig, FileName);
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

    public void Update()
    {
        if (!pendingSave || DateTime.UtcNow < saveThreshold) return;
        SaveFile();
        pendingSave = false;
    }

    public Settings CreateFile()
    {
        Settings config = new();
        Save();
        return config;
    }

    public Settings Load()
    {
        if (!File.Exists(FileName))
        {
            Log.Info("No config file found. Creating new");
            return CreateFile();
        }

        try
        {
            Log.Info("Loading config file");
            if (Json.DeserializeFromFile<Settings>(FileName) is { } config)
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
            if (Json.DeserializeFromFile<Settings>(FileName) is { } newConfig)
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
