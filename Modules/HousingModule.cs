using ImGuiNET;
using System.Numerics;

namespace ZoneLevelGuide.Modules
{
    public class HousingModule : BaseZoneModule
    {
        public override string ZoneName => "Housing";
        public override string LevelRange => "Any Level";
        public override Vector4 Color => new Vector4(0.8f, 0.6f, 0.4f, 1.0f); // Warm brown/orange for housing

        public HousingModule(ITeleporterIpc? teleporter) : base(teleporter) { }

        public override void DrawContent()
        {
            DrawZoneHeader("Housing & Residences", "Any Level", Color);
            
            DrawZoneSection("Estate Teleportation", "Any Level",
                "Direct teleport to your housing (if you own any)",
                () => {
                    DrawEstateButtons();
                    ImGui.Spacing();
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.8f, 0.9f, 0.8f, 1.0f)); // Light green
                    ImGui.TextWrapped("These buttons use the game's estate teleportation system:");
                    ImGui.Indent(10.0f);
                    ImGui.Text("• Only work if you own housing or have FC housing access");
                    ImGui.Text("• Will show an error if you don't have housing");
                    ImGui.Unindent(10.0f);
                    ImGui.PopStyleColor();
                }, Color);
            
            DrawZoneSection("Housing Districts", "Any Level",
                "Teleport to main cities near housing areas",
                () => {
                    DrawTeleportButton("The Mist (via Limsa)", 8); // Limsa Lominsa Lower Decks
                    ImGui.SameLine();
                    DrawTeleportButton("Lavender Beds (via Gridania)", 2); // New Gridania
                    ImGui.SameLine();
                    DrawTeleportButton("The Goblet (via Ul'dah)", 9); // Ul'dah - Steps of Nald
                    ImGui.Spacing();
                    DrawTeleportButton("Shirogane (via Kugane)", 111); // Kugane
                    ImGui.SameLine();
                    DrawTeleportButton("Empyreum (via Ishgard)", 70); // Foundation
                    ImGui.Spacing();
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.8f, 0.8f, 0.6f, 1.0f)); // Light yellow
                    ImGui.Text("(Walk to housing districts from these main cities)");
                    ImGui.PopStyleColor();
                }, Color);
            
            DrawHousingInformation();
            DrawTeleportInfoPopup();
        }

        private void DrawEstateButtons()
        {
            // Private Estate Button
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(10, 6));
            ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 3.0f);
            
            if (teleporter != null)
            {
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.4f, 0.6f, 0.4f, 0.8f)); // Green theme for estate
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.5f, 0.7f, 0.5f, 0.9f));
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.6f, 0.8f, 0.6f, 1.0f));
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.95f, 0.95f, 0.95f, 1.0f));

                bool pressedPrivate = ImGui.Button("🏠 Private Estate Hall###private_estate");
                ImGui.PopStyleColor(4);

                if (pressedPrivate)
                {
                    ExecuteEstateCommand("Estate Hall");
                }
            }
            else
            {
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.35f, 0.35f, 0.35f, 0.6f));
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.75f, 0.75f, 0.75f, 0.8f));
                ImGui.Button("🏠 Private Estate Hall###disabled_private");
                ImGui.PopStyleColor(2);
            }

            ImGui.SameLine();

            // FC Estate Button
            if (teleporter != null)
            {
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.4f, 0.4f, 0.6f, 0.8f)); // Blue theme for FC
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.5f, 0.5f, 0.7f, 0.9f));
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.6f, 0.6f, 0.8f, 1.0f));
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.95f, 0.95f, 0.95f, 1.0f));

                bool pressedFC = ImGui.Button("🏰 FC Estate Hall###fc_estate");
                ImGui.PopStyleColor(4);

                if (pressedFC)
                {
                    ExecuteEstateCommand("Estate Hall (Free Company)");
                }
            }
            else
            {
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.35f, 0.35f, 0.35f, 0.6f));
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.75f, 0.75f, 0.75f, 0.8f));
                ImGui.Button("🏰 FC Estate Hall###disabled_fc");
                ImGui.PopStyleColor(2);
            }

            ImGui.SameLine();

            // Shared Estate (Apartment) Button
            if (teleporter != null)
            {
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.6f, 0.4f, 0.6f, 0.8f)); // Purple theme for apartments
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.7f, 0.5f, 0.7f, 0.9f));
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.8f, 0.6f, 0.8f, 1.0f));
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.95f, 0.95f, 0.95f, 1.0f));

                bool pressedShared = ImGui.Button("🏢 Shared Estate###shared_estate");
                ImGui.PopStyleColor(4);

                if (pressedShared)
                {
                    ExecuteEstateCommand("Shared Estate");
                }
            }
            else
            {
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.35f, 0.35f, 0.35f, 0.6f));
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.75f, 0.75f, 0.75f, 0.8f));
                ImGui.Button("🏢 Shared Estate###disabled_shared");
                ImGui.PopStyleColor(2);
            }

            ImGui.PopStyleVar(2);
        }

        private void ExecuteEstateCommand(string estateName)
        {
            try
            {
                // Cast to TeleporterService to access the ExecuteEstateCommand method
                var teleporterService = teleporter as TeleporterService;
                if (teleporterService != null)
                {
                    teleporterService.ExecuteEstateCommand(estateName);
                }
                else
                {
                    // Fallback: show the command info popup
                    ImGui.OpenPopup($"Estate Command Info###{estateName}");
                }
            }
            catch (System.Exception)
            {
                ImGui.OpenPopup($"Estate Command Info###{estateName}");
            }
        }

        private void DrawNativeTeleportButton()
        {
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(10, 6));
            ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 3.0f);
            
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.2f, 0.5f, 0.8f, 0.8f)); // Blue theme
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.3f, 0.6f, 0.9f, 0.9f));
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.4f, 0.7f, 1.0f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.95f, 0.95f, 0.95f, 1.0f));

            bool pressed = ImGui.Button("🌐 Type '/teleport' in Chat###native_teleport_info");

            ImGui.PopStyleColor(4);
            ImGui.PopStyleVar(2);

            if (pressed)
            {
                ShowTeleportInfo();
            }
        }

        private void ShowTeleportInfo()
        {
            ImGui.OpenPopup("Native Teleport Info");
        }

        private void DrawTeleportInfoPopup()
        {
            // Show estate command info popup
            if (ImGui.BeginPopup("Estate Command Info###Estate Hall") || 
                ImGui.BeginPopup("Estate Command Info###Estate Hall (Free Company)") ||
                ImGui.BeginPopup("Estate Command Info###Shared Estate"))
            {
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.9f, 0.9f, 0.9f, 1.0f));
                ImGui.Text("To teleport to your estate:");
                ImGui.Spacing();
                ImGui.Text("Type in chat:");
                ImGui.Indent(10.0f);
                ImGui.Text("/tp Estate Hall (for private housing)");
                ImGui.Text("/tp Estate Hall (Free Company) (for FC housing)");
                ImGui.Text("/tp Shared Estate (for apartments)");
                ImGui.Unindent(10.0f);
                ImGui.PopStyleColor();
                
                if (ImGui.Button("Close"))
                {
                    ImGui.CloseCurrentPopup();
                }
                
                ImGui.EndPopup();
            }
        }

        private void DrawHousingInformation()
        {
            ImGui.Spacing();
            ImGui.Separator();
            ImGui.Spacing();
            
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.9f, 0.9f, 0.9f, 1.0f));
            ImGui.SetWindowFontScale(1.1f);
            ImGui.Text("🏠 Housing Information");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.PopStyleColor();
            
            ImGui.Spacing();
            
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.85f, 0.85f, 0.85f, 1.0f));
            ImGui.TextWrapped("Housing Districts & Access:");
            ImGui.Indent(10.0f);
            ImGui.Text("• The Mist (Limsa Lominsa) - Ferry from Lower Decks");
            ImGui.Text("• The Lavender Beds (Gridania) - Exit from New Gridania");
            ImGui.Text("• The Goblet (Ul'dah) - Exit from Steps of Nald");
            ImGui.Text("• Shirogane (Kugane) - Boat from Kugane docks");
            ImGui.Text("• Empyreum (Ishgard) - Lift from Foundation");
            ImGui.Unindent(10.0f);
            
            ImGui.Spacing();
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.6f, 0.9f, 0.9f, 1.0f)); // Light cyan
            ImGui.TextWrapped("Note: Estate teleportation only works if you own housing or have FC housing access. " +
                            "Shared Estate teleports to apartments if you own one.");
            ImGui.PopStyleColor();
            ImGui.PopStyleColor();
        }
    }
}