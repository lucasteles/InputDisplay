using InputDisplay.Config;

var isConfig = args is ["config", ..];
using Game game = isConfig ? new SettingsGame() : new InputDisplay.GameMain();
game.Run();
