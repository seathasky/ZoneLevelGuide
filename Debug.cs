using Dalamud.Interface.Windowing;
using ImGuiNET;
using System.Numerics;

namespace ZoneLevelGuide
{
    public class DebugWindow : Window
    {
        private readonly ITeleporterIpc? teleporter;
        private string teleportIdInput = "";

        public DebugWindow(ITeleporterIpc? teleporter) : base(
            "ZoneLevelGuide Debug",
            ImGuiWindowFlags.AlwaysAutoResize)
        {
            this.teleporter = teleporter;
            this.IsOpen = false;
        }

        public override void Draw()
        {
            ImGui.Text("Manual Teleport (Aetheryte ID):");
            ImGui.InputText("##DebugTeleportId", ref teleportIdInput, 16);

            if (ImGui.Button("Teleport"))
            {
                if (uint.TryParse(teleportIdInput, out var id))
                {
                    teleporter?.Teleport(id);
                }
            }
        }
    }
}
