using Dalamud.Game.Command;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Interface.Windowing;
using Dalamud.Game;
using System;

namespace ZoneLevelGuide
{
    public sealed class PluginMain : IDalamudPlugin
    {
        public string Name => "Zone Level Guide";

        private readonly ICommandManager commandManager;
        private readonly WindowSystem windowSystem;
        private readonly ZoneLevelWindow zoneLevelWindow;
        private readonly IDalamudPluginInterface pluginInterface;
        private readonly TeleporterService teleporterService;

        public PluginMain(
            IDalamudPluginInterface pluginInterface,
            ICommandManager commandManager,
            IGameGui gameGui,
            IChatGui chatGui,
            IClientState clientState,
            ICondition condition,
            ISigScanner sigScanner,
            IGameInteropProvider gameInterop,
            ITextureProvider textureProvider)
        {
            this.pluginInterface = pluginInterface;
            this.commandManager = commandManager;

            // Create teleporter service with command manager for /tp commands
            this.teleporterService = new TeleporterService(pluginInterface, chatGui, commandManager, sigScanner, gameInterop);

            // Set up window system
            windowSystem = new WindowSystem("ZoneLevelGuide");
            zoneLevelWindow = new ZoneLevelWindow(this.teleporterService);
            windowSystem.AddWindow(zoneLevelWindow);

            // Register UI events
            pluginInterface.UiBuilder.Draw += DrawUI;
            pluginInterface.UiBuilder.OpenConfigUi += OpenConfigUI;

            // Register command
            this.commandManager.AddHandler("/zoneguide", new CommandInfo(OnCommand)
            {
                HelpMessage = "Display zone level information"
            });

            // Optionally register additional command
            this.commandManager.AddHandler("/zg", new CommandInfo(OnCommand)
            {
                HelpMessage = "Display zone level information (alternative command)"
            });
        }

        private void OnCommand(string command, string args)
        {
            zoneLevelWindow.IsOpen = !zoneLevelWindow.IsOpen;
        }

        private void OpenConfigUI()
        {
            zoneLevelWindow.IsOpen = true;
        }

        private void DrawUI()
        {
            windowSystem.Draw();
        }

        public void Dispose()
        {
            pluginInterface.UiBuilder.Draw -= DrawUI;
            pluginInterface.UiBuilder.OpenConfigUi -= OpenConfigUI;

            windowSystem.RemoveAllWindows();
            commandManager.RemoveHandler("/zoneguide");
            commandManager.RemoveHandler("/zg");
        }
    }
}
