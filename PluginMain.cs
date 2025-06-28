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
        private readonly DebugWindow debugWindow; // Add this line

        private readonly Action openMainUiCallback;

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

            // Initialize teleporter service for teleport commands
            this.teleporterService = new TeleporterService(pluginInterface, chatGui, commandManager, sigScanner, gameInterop);

            // Initialize window system and main window
            windowSystem = new WindowSystem("ZoneLevelGuide");
            zoneLevelWindow = new ZoneLevelWindow(this.teleporterService);
            debugWindow = new DebugWindow(this.teleporterService); // Add this line
            windowSystem.AddWindow(zoneLevelWindow);
            windowSystem.AddWindow(debugWindow); // Add this line

            // Register UI events
            pluginInterface.UiBuilder.Draw += DrawUI;
            pluginInterface.UiBuilder.OpenConfigUi += OpenConfigUI;

            // Register OpenMainUi callback
            openMainUiCallback = () => zoneLevelWindow.IsOpen = true;
            pluginInterface.UiBuilder.OpenMainUi += openMainUiCallback;

            // Register main command
            this.commandManager.AddHandler("/zg", new CommandInfo(OnCommand)
            {
                HelpMessage = "Display zone level information"
            });
        }

        private void OnCommand(string command, string args)
        {
            zoneLevelWindow.IsOpen = !zoneLevelWindow.IsOpen;
        }

        private void OpenConfigUI()
        {
            debugWindow.IsOpen = true; // Open the debug window from settings
        }

        private void DrawUI()
        {
            windowSystem.Draw();
        }

        public void Dispose()
        {
            pluginInterface.UiBuilder.Draw -= DrawUI;
            pluginInterface.UiBuilder.OpenConfigUi -= OpenConfigUI;
            pluginInterface.UiBuilder.OpenMainUi -= openMainUiCallback;

            windowSystem.RemoveAllWindows();
            commandManager.RemoveHandler("/zg");
        }
    }
}
