using Dalamud.Game.Command;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Interface.Windowing;
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
        public Configuration Configuration { get; private set; }

        public PluginMain(
            IDalamudPluginInterface pluginInterface,
            ICommandManager commandManager,
            IChatGui chatGui,
            IClientState clientState)
        {
            this.pluginInterface = pluginInterface;
            this.commandManager = commandManager;

            Configuration = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Configuration.Initialize(pluginInterface);

            this.teleporterService = new TeleporterService(pluginInterface, chatGui, commandManager, clientState);

            windowSystem = new WindowSystem("ZoneLevelGuide");
            zoneLevelWindow = new ZoneLevelWindow(this.teleporterService, Configuration);
            windowSystem.AddWindow(zoneLevelWindow);

            pluginInterface.UiBuilder.Draw += DrawUI;
            pluginInterface.UiBuilder.OpenConfigUi += OpenConfigUI;

            pluginInterface.UiBuilder.OpenMainUi += () => zoneLevelWindow.IsOpen = true;

            this.commandManager.AddHandler("/zoneguide", new CommandInfo(OnCommand)
            {
                HelpMessage = "Display zone level information"
            });

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
            pluginInterface.UiBuilder.OpenMainUi -= () => zoneLevelWindow.IsOpen = true;

            windowSystem.RemoveAllWindows();
            commandManager.RemoveHandler("/zoneguide");
            commandManager.RemoveHandler("/zg");
        }
    }
}
