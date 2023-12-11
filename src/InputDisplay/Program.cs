using InputDisplay.Config;
using InputDisplay.Config.Screen;

var isConfig = args is ["config", ..];
using Game game = isConfig ? new SettingsGame() : new InputDisplay.GameMain();
game.Run();
