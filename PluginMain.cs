using Dalamud.Game.Command;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Interface.Windowing;
using System;

namespace ZoneLevels
{
    public sealed class PluginMain : IDalamudPlugin
    {
        public string Name => "Zone Level Guide";

        private readonly ICommandManager commandManager;
        private readonly WindowSystem windowSystem;
        private readonly ZoneLevelWindow zoneLevelWindow;
        private readonly IDalamudPluginInterface pluginInterface;

        public PluginMain(
            IDalamudPluginInterface pluginInterface,
            ICommandManager commandManager)
        {
            this.pluginInterface = pluginInterface;
            this.commandManager = commandManager;

            // Set up window system
            windowSystem = new WindowSystem("ZoneLevelGuide");
            zoneLevelWindow = new ZoneLevelWindow();
            windowSystem.AddWindow(zoneLevelWindow);

            // Register UI events
            pluginInterface.UiBuilder.Draw += DrawUI;
            pluginInterface.UiBuilder.OpenConfigUi += OpenConfigUI;

            // Register command
            this.commandManager.AddHandler("/zonelevel", new CommandInfo(OnCommand)
            {
                HelpMessage = "Display zone level information"
            });

            // Optionally register additional command
            this.commandManager.AddHandler("/tplevels", new CommandInfo(OnCommand)
            {
                HelpMessage = "Display zone level information (alternative commands)"
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
            commandManager.RemoveHandler("/zonelevel");
            commandManager.RemoveHandler("/tplevels");
        }
    }
}
