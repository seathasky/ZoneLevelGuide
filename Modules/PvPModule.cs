using ImGuiNET;
using System.Numerics;

namespace ZoneLevelGuide.Modules
{
    public class PvPModule : BaseZoneModule
    {
        public override string ZoneName => "PvP";
        public override string LevelRange => "Level 30+";
        public override Vector4 Color => new Vector4(0.9f, 0.4f, 0.4f, 1.0f); // Red theme for PvP

        public PvPModule(ITeleporterIpc? teleporter) : base(teleporter) { }

        public override void DrawContent()
        {
            DrawZoneHeader("Player vs Player Areas", "Level 30+", Color);
            
            DrawZoneSection("Wolves' Den Pier", "Level 30+",
                "Main PvP hub and training area",
                () => {
                    DrawPvPTeleportButton("Wolves' Den Pier", 54);
                    ImGui.Spacing();
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.9f, 0.8f, 0.8f, 1.0f)); // Light red
                    ImGui.TextWrapped("Central hub for all PvP activities:");
                    ImGui.Indent(10.0f);
                    ImGui.Text("• Access PvP modes and queues");
                    ImGui.Text("• Purchase PvP gear and rewards");
                    ImGui.Text("• Practice with training dummies");
                    ImGui.Text("• Access PvP Profile and Series rewards");
                    ImGui.Unindent(10.0f);
                    ImGui.PopStyleColor();
                }, Color);
            
            DrawPvPInformation();
        }

        private void DrawPvPTeleportButton(string locationName, uint aetheryteId)
        {
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(10, 6));
            ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 3.0f);
            
            if (teleporter != null)
            {
                // Red theme for PvP button
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.7f, 0.2f, 0.2f, 0.8f));
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.8f, 0.3f, 0.3f, 0.9f));
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.9f, 0.4f, 0.4f, 1.0f));
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.95f, 0.95f, 0.95f, 1.0f));

                bool pressed = ImGui.Button($"⚔️ {locationName}###tp_{aetheryteId}");

                ImGui.PopStyleColor(4);

                if (pressed)
                {
                    try
                    {
                        teleporter.Teleport(aetheryteId);
                    }
                    catch (System.Exception)
                    {
                        // Silently fail if teleport is not available
                    }
                }
            }
            else
            {
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.35f, 0.35f, 0.35f, 0.6f));
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.75f, 0.75f, 0.75f, 0.8f));
                ImGui.Button($"⚔️ {locationName}###disabled_{aetheryteId}");
                ImGui.PopStyleColor(2);
            }
            
            ImGui.PopStyleVar(2);
        }

        private void DrawPvPInformation()
        {
            ImGui.Spacing();
            ImGui.Separator();
            ImGui.Spacing();
            
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.9f, 0.9f, 0.9f, 1.0f));
            ImGui.SetWindowFontScale(1.1f);
            ImGui.Text("⚔️ PvP Information");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.PopStyleColor();
            
            ImGui.Spacing();
            
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.85f, 0.85f, 0.85f, 1.0f));
            ImGui.TextWrapped("PvP Game Modes:");
            ImGui.Indent(10.0f);
            ImGui.Text("• Crystalline Conflict (5v5) - Ranked and Casual");
            ImGui.Text("• Frontline (24v24v24) - Large scale battles");
            ImGui.Text("• Rival Wings (24v24) - MOBA-style gameplay");
            ImGui.Unindent(10.0f);
            
            ImGui.Spacing();
            ImGui.TextWrapped("PvP Features:");
            ImGui.Indent(10.0f);
            ImGui.Text("• Separate PvP actions and job balance");
            ImGui.Text("• Series Malmstones with exclusive rewards");
            ImGui.Text("• PvP-specific gear and glamour");
            ImGui.Text("• Adventure Plates and portraits");
            ImGui.Unindent(10.0f);
            
            ImGui.Spacing();
            ImGui.TextWrapped("Requirements:");
            ImGui.Indent(10.0f);
            ImGui.Text("• Level 30+ on any job");
            ImGui.Text("• Complete quest 'A Pup No Longer' (Limsa Lominsa)");
            ImGui.Text("• Unlocks at Wolves' Den Pier");
            ImGui.Unindent(10.0f);
            
            ImGui.Spacing();
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.9f, 0.7f, 0.7f, 1.0f)); // Light red
            ImGui.TextWrapped("Note: PvP uses a separate progression system with PvP ranks, " +
                            "Series Malmstones, and unique rewards. All jobs are viable in PvP!");
            ImGui.PopStyleColor();
            ImGui.PopStyleColor();
        }
    }
}
