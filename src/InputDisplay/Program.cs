using InputDisplay.Config.Screen;

var isConfig = args is ["config", ..];
using Game game = isConfig ? new SettingsGame(args.ElementAtOrDefault(1)) : new InputDisplay.GameMain();
game.Run();
