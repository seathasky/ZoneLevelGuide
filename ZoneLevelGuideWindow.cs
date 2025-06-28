using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using ImGuiNET;
using System.Numerics;
using Dalamud.Interface.Utility;
using System;

namespace ZoneLevelGuide
{
    public class ZoneLevelWindow : Window
    {
        private int selectedTab = 0;
        private readonly ITeleporterIpc? teleporter;

        private const float TabWidth = 220f;
        private const float ContentPadding = 48f;

        private readonly (string name, string levels)[] tabInfo = {
            ("Gridania", "1-30"),
            ("Limsa Lominsa", "1-50"),
            ("Ul'dah", "1-50"),
            ("Mor Dhona", "45-50"),
            ("Coerthas", "35-53"),
            ("Ishgard", "50-60"),
            ("Far East", "60-70"),
            ("Norvrandt", "70-80"),
            ("Ilsabard", "80-90"),
            ("Tural", "90-100")
        };

        public ZoneLevelWindow(ITeleporterIpc? teleporter = null) : base(
            "Zone Level Guide",
            ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
        {
            this.teleporter = teleporter;
            this.SizeConstraints = new WindowSizeConstraints
            {
                MinimumSize = new Vector2(900, 600),
                MaximumSize = new Vector2(1400, 900)
            };
            this.Size = new Vector2(1100, 700); // Clean, centered, not too wide/tall
        }

        public override void Draw()
        {
            // Suppress nullable warning by using .GetValueOrDefault() with fallback
            ImGui.SetNextWindowSize(this.Size.GetValueOrDefault(new Vector2(1100, 700)), ImGuiCond.FirstUseEver);
            ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0.15f, 0.16f, 0.18f, 1.0f)); // dark gray

            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.85f, 0.2f, 1.0f));
            ImGui.TextWrapped("Teleport feature requires the 'Teleporter' plugin from the plugin installer.");
            ImGui.PopStyleColor();

            ImGui.Spacing();

            float contentWidth = this.Size.GetValueOrDefault(new Vector2(1100, 700)).X - TabWidth - ContentPadding;

            ImGui.BeginGroup();
            ImGui.BeginChild("##ZoneTabs", new Vector2(TabWidth, 0), true);
            for (int i = 0; i < tabInfo.Length; i++)
            {
                bool selected = (selectedTab == i);
                if (ImGui.Selectable($"{tabInfo[i].name} {tabInfo[i].levels}", selected))
                {
                    selectedTab = i;
                }
            }
            ImGui.EndChild();
            ImGui.EndGroup();

            ImGui.SameLine();

            ImGui.BeginGroup();
            ImGui.BeginChild("##ZoneTabContent", new Vector2(contentWidth, 0), false, ImGuiWindowFlags.AlwaysVerticalScrollbar);

            ImGui.Spacing();
            ImGui.Separator();

            switch (selectedTab)
            {
                case 0: DrawGridaniaContent(); break;
                case 1: DrawLimsaContent(); break;
                case 2: DrawUldahContent(); break;
                case 3: DrawMorDhonaContent(); break;
                case 4: DrawCoerthasContent(); break;
                case 5: DrawIshgardContent(); break;
                case 6: DrawFarEastContent(); break;
                case 7: DrawNorvrandtContent(); break;
                case 8: DrawIlsabardContent(); break;
                case 9: DrawDawntrailContent(); break;
            }

            ImGui.Spacing();
            ImGui.EndChild();
            ImGui.EndGroup();

            ImGui.PopStyleColor(); // Pop WindowBg
        }

        private void DrawTeleportButton(string locationName, uint aetheryteId)
        {
            if (teleporter != null)
            {
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.22f, 0.28f, 0.38f, 1.0f));         // Very dark blue-gray
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.18f, 0.23f, 0.32f, 1.0f));   // Even darker on hover
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.15f, 0.19f, 0.27f, 1.0f));    // Even darker when pressed
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1f, 1f, 1f, 1.0f));                     // White text

                bool pressed = ImGui.Button($"{locationName}###tp_{aetheryteId}");

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
                ImGui.TextColored(new Vector4(0.8f, 0.8f, 0.8f, 1.0f), $"{locationName}");
            }
        }

        private void DrawDiscoveryButton()
        {
        }

        private void DrawGridaniaContent()
        {
            ImGui.SetWindowFontScale(1.5f);
            ImGui.Text("Gridania and The Black Shroud ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "(1-30)");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.Separator();

            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.0f, 0.5f, 0.0f, 1.0f), "New Gridania & Old Gridania:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "1-15");
            ImGui.Text("Starting Area for Conjurers, Archers, and Lancers");
            DrawTeleportButton("New Gridania", 2);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.0f, 0.5f, 0.0f, 1.0f), "Central Shroud:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "5-15");
            DrawTeleportButton("Bentbranch Meadows", 3);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.0f, 0.5f, 0.0f, 1.0f), "East Shroud:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "15-25");
            DrawTeleportButton("Hawthorne Hut", 4);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.0f, 0.5f, 0.0f, 1.0f), "South Shroud:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "20-30");
            DrawTeleportButton("Quarrymill", 5);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.0f, 0.5f, 0.0f, 1.0f), "North Shroud:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "15-30");
            DrawTeleportButton("Camp Tranquil", 6);
            DrawTeleportButton("Fallgourd Float", 7);

        }

        private void DrawLimsaContent()
        {
            ImGui.SetWindowFontScale(1.5f);
            ImGui.Text("Limsa Lominsa and La Noscea ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "(1-50)");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.Separator();

            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.1f, 0.6f, 0.9f, 1.0f), "Limsa Lominsa Upper/Lower Decks:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "1-15");
            ImGui.Text("Starting Area for Marauders, Arcanists, and Rogues");
            DrawTeleportButton("Limsa Lominsa Lower Decks", 8);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.1f, 0.6f, 0.9f, 1.0f), "Middle La Noscea:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "5-15");
            ImGui.Text("Notable locations: Summerford Farms, Seasong Grotto, Three-malm Bend");
            DrawTeleportButton("Summerford Farms", 52);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.1f, 0.6f, 0.9f, 1.0f), "Lower La Noscea:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "10-20");
            ImGui.Text("Notable locations: Moraby Drydocks, Cedarwood, Aleport");
            DrawTeleportButton("Moraby Drydocks", 10);
            DrawTeleportButton("Aleport", 14);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.1f, 0.6f, 0.9f, 1.0f), "Western La Noscea:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "15-30");
            ImGui.Text("Notable locations: Wineport, Skull Valley, Halfstone");
            DrawTeleportButton("Swiftperch", 13);     // Fixed: use actual ID 13

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.1f, 0.6f, 0.9f, 1.0f), "Eastern La Noscea:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "30-35");
            ImGui.Text("Notable locations: Costa del Sol, Bloodshore, Hidden Falls");
            DrawTeleportButton("Costa del Sol", 11);     // Fixed: use actual ID 11

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.1f, 0.6f, 0.9f, 1.0f), "Upper La Noscea:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "30-35");
            ImGui.Text("Notable locations: Camp Bronze Lake, Bronze Lake, Quarterstone");
            DrawTeleportButton("Camp Bronze Lake", 15);   // Fixed: use actual ID 15

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.1f, 0.6f, 0.9f, 1.0f), "Outer La Noscea:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "40-50");
            ImGui.Text("Notable locations: Camp Overlook, U'Ghamaro Mines, Thalaos");
            DrawTeleportButton("Camp Overlook", 16);   // Fixed: use actual ID 16
        }

        private void DrawUldahContent()
        {
            ImGui.SetWindowFontScale(1.5f);
            ImGui.Text("Ul'dah and Thanalan ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "(1-50)");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.Separator();

            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.9f, 0.8f, 1.0f, 1.0f), "Ul'dah - Steps of Nald/Thal:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "1-15");
            ImGui.Text("Starting Area for Gladiators, Pugilists, and Thaumaturges");
            DrawTeleportButton("Ul'dah - Steps of Nald", 9);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.9f, 0.8f, 1.0f, 1.0f), "Western Thanalan:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "5-15");
            ImGui.Text("Notable locations: Horizon, Scorpion Crossing, Footprint");
            DrawTeleportButton("Horizon", 17);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.9f, 0.8f, 1.0f, 1.0f), "Central Thanalan:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "15-20");
            ImGui.Text("Notable locations: Black Brush Station, Spineless Basin, Cactuar Cut");
            DrawTeleportButton("Black Brush Station", 53);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.9f, 0.8f, 1.0f, 1.0f), "Eastern Thanalan:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "20-25");
            ImGui.Text("Notable locations: Camp Drybone, The Burning Wall, Sandgate");
            DrawTeleportButton("Camp Drybone", 20);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.9f, 0.8f, 1.0f, 1.0f), "Southern Thanalan:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "35-40");
            ImGui.Text("Notable locations: Little Ala Mhigo, Forgotten Springs, Byregot's Strike");
            DrawTeleportButton("Little Ala Mhigo", 19);
            DrawTeleportButton("Forgotten Springs", 20);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.9f, 0.8f, 1.0f, 1.0f), "Northern Thanalan:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "45-50");
            ImGui.Text("Notable locations: Bluefog, Ceruleum Processing Plant, Camp Bluefog");
            DrawTeleportButton("Camp Bluefog", 21);

        }

        private void DrawCoerthasContent()
        {
            ImGui.SetWindowFontScale(1.5f);
            ImGui.Text("Coerthas - Pre-Calamity Regions ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "(35-53)");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.Separator();

            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.8f, 0.9f, 1.0f, 1.0f), "Coerthas Central Highlands:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "35-45");
            ImGui.Text("Mountainous region south of Ishgard");
            DrawTeleportButton("Camp Dragonhead", 23);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.8f, 0.9f, 1.0f, 1.0f), "Coerthas Western Highlands:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "50-53");
            ImGui.Text("Frozen wastes west of Ishgard");
            DrawTeleportButton("Falcon's Nest", 71);
        }

        private void DrawMorDhonaContent()
        {
            ImGui.SetWindowFontScale(1.5f);
            ImGui.Text("Mor Dhona ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "(45-50)");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.Separator();

            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.7f, 0.5f, 0.9f, 1.0f), "Mor Dhona:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "45-50");
            ImGui.Text("Gateway to Heavensward content and endgame hub");
            ImGui.Text("Notable locations: Revenant's Toll, The Rising Stones, Crystal Tower");
            DrawTeleportButton("Revenant's Toll", 24);
        }

        private void DrawIshgardContent()
        {
            ImGui.SetWindowFontScale(1.5f);
            ImGui.Text("Ishgard and Dravania - Heavensward ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "(50-60)");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.Separator();

            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(1.0f, 0.4f, 0.7f, 1.0f), "Foundation & The Pillars:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "50-60");
            ImGui.Text("City of Ishgard - Heavensward starting area");
            DrawTeleportButton("Foundation", 70);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(1.0f, 0.4f, 0.7f, 1.0f), "The Sea of Clouds:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "51-54");
            ImGui.Text("Series of floating islands in the sky");
            DrawTeleportButton("Camp Cloudtop", 72);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(1.0f, 0.4f, 0.7f, 1.0f), "The Churning Mists:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "54-56");
            ImGui.Text("Ancient home of the dragons");
            DrawTeleportButton("Moghome", 78);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(1.0f, 0.4f, 0.7f, 1.0f), "The Dravanian Forelands:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "52-54");
            ImGui.Text("Vast wilderness of Dravania");
            DrawTeleportButton("Tailfeather", 76);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(1.0f, 0.4f, 0.7f, 1.0f), "The Dravanian Hinterlands:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "56-60");
            ImGui.Text("Site of the abandoned city of Sharlayan");
            DrawTeleportButton("Idyllshire", 75);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(1.0f, 0.4f, 0.7f, 1.0f), "Azys Lla:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "58-60");
            ImGui.Text("Ancient Allagan research facility");
            DrawTeleportButton("Helix", 74);

            // Discovery button removed for production
        }

        private void DrawFarEastContent()
        {
            ImGui.SetWindowFontScale(1.5f);
            ImGui.Text("The Far East - Stormblood ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "(60-70)");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.Separator();

            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.8f, 0.3f, 0.3f, 1.0f), "Kugane:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "60-70");
            ImGui.Text("Main city hub for Stormblood");
            DrawTeleportButton("Kugane", 111);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.8f, 0.3f, 0.3f, 1.0f), "The Ruby Sea:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "61-63");
            DrawTeleportButton("Tamamizu", 105);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.8f, 0.3f, 0.3f, 1.0f), "Yanxia:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "64-65");
            DrawTeleportButton("Namai", 107);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.8f, 0.3f, 0.3f, 1.0f), "The Azim Steppe:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "65-67");
            DrawTeleportButton("Reunion", 109);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.8f, 0.3f, 0.3f, 1.0f), "The Fringes:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "60-62");
            DrawTeleportButton("Rhalgr's Reach", 104);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.8f, 0.3f, 0.3f, 1.0f), "The Peaks:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "63-64");
            DrawTeleportButton("Ala Gannha", 100);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.8f, 0.3f, 0.3f, 1.0f), "The Lochs:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "67-70");
            DrawTeleportButton("The Ala Mhigan Quarter", 103);
        }

        private void DrawNorvrandtContent()
        {
            ImGui.SetWindowFontScale(1.5f);
            ImGui.Text("Norvrandt - Shadowbringers ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "(70-80)");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.Separator();

            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.6f, 0.4f, 0.8f, 1.0f), "The Crystarium:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "70-80");
            ImGui.Text("Main city hub for Shadowbringers");
            DrawTeleportButton("The Crystarium", 133);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.6f, 0.4f, 0.8f, 1.0f), "Eulmore:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "70-80");
            ImGui.Text("Secondary city hub");
            DrawTeleportButton("Eulmore", 134);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.6f, 0.4f, 0.8f, 1.0f), "Lakeland:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "70-72");
            ImGui.Text("Notable locations: Fort Jobb, The Ostall Imperative, The Source");
            DrawTeleportButton("Fort Jobb", 132);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.6f, 0.4f, 0.8f, 1.0f), "Kholusia:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "70-72");
            ImGui.Text("Notable locations: Stilltide, Wright, Tomra");
            DrawTeleportButton("Stilltide", 137);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.6f, 0.4f, 0.8f, 1.0f), "Amh Araeng:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "75-77");
            ImGui.Text("Notable locations: Mord Souq, The Inn at Journey's Head, Twine");
            DrawTeleportButton("Mord Souq", 140);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.6f, 0.4f, 0.8f, 1.0f), "Il Mheg:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "73-75");
            ImGui.Text("Notable locations: Lydha Lran, Pla Enni, Wolekdorf");
            DrawTeleportButton("Lydha Lran", 144);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.6f, 0.4f, 0.8f, 1.0f), "The Rak'tika Greatwood:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "74-76");
            ImGui.Text("Notable locations: Slitherbough, Fanow, The Ox'Dalan Gap");
            DrawTeleportButton("Slitherbough", 142);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.6f, 0.4f, 0.8f, 1.0f), "The Tempest:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "78-80");
            ImGui.Text("Notable locations: The Ondo Cups, Macarenses Angle, The Amaurotine Relic");
            DrawTeleportButton("The Ondo Cups", 147);
        }

        private void DrawDawntrailContent()
        {
            ImGui.SetWindowFontScale(1.5f);
            ImGui.Text("Tural & Solution - Dawntrail ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "(90-100)");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.Separator();

            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.0f, 0.8f, 0.8f, 1.0f), "Tuliyollal:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "90-91");
            ImGui.Text("Main city hub for Dawntrail");
            DrawTeleportButton("Tuliyollal", 216);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.0f, 0.8f, 0.8f, 1.0f), "Solution Nine:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "97-100");
            ImGui.Text("Second major city for Dawntrail");
            DrawTeleportButton("Solution Nine", 217);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.0f, 0.8f, 0.8f, 1.0f), "Urqopacha:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "90-92");
            DrawTeleportButton("Wachunpelo", 200);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.0f, 0.8f, 0.8f, 1.0f), "Kozama'uka:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "94-96");
            DrawTeleportButton("Ok'hanu", 202);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.0f, 0.8f, 0.8f, 1.0f), "Yak T'el:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "92-94");
            DrawTeleportButton("Iq Br'aax", 205);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.0f, 0.8f, 0.8f, 1.0f), "Shaaloani:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "93-95");
            DrawTeleportButton("Hhusatahwi", 207);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.0f, 0.8f, 0.8f, 1.0f), "Heritage Found:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "96-98");
            DrawTeleportButton("Electrope Strike", 212);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.0f, 0.8f, 0.8f, 1.0f), "Living Memory:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "99-100");
            DrawTeleportButton("Leynode Mnemo", 213);
        }

        // Add missing DrawIlsabardContent method
        private void DrawIlsabardContent()
        {
            ImGui.SetWindowFontScale(1.5f);
            ImGui.Text("Ilsabard and Beyond - Endwalker ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "(80-90)");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.Separator();

            // Radz-at-Han (main city, first)
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.3f, 0.7f, 0.7f, 1.0f), "Radz-at-Han:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "80-90");
            ImGui.Text("City hub for Endwalker");
            DrawTeleportButton("Radz-at-Han", 183);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.3f, 0.7f, 0.7f, 1.0f), "Old Sharlayan:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "80-90");
            ImGui.Text("City hub for Endwalker");
            DrawTeleportButton("Old Sharlayan", 182);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.3f, 0.7f, 0.7f, 1.0f), "Labyrinthos:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "81-83");
            ImGui.Text("Notable locations: The Archeion, Sharlayan Hamlet, The Twelve Wonders");
            DrawTeleportButton("The Archeion", 166);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.3f, 0.7f, 0.7f, 1.0f), "Thavnair:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "80-82");
            ImGui.Text("Notable locations: Yedlihmad, The Great Work, Palaka's Stand");
            DrawTeleportButton("Yedlihmad", 169);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.3f, 0.7f, 0.7f, 1.0f), "Garlemald:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "83-85");
            ImGui.Text("Notable locations: Tertium, Camp Broken Glass, The Eblan Rime");
            DrawTeleportButton("Tertium", 173);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.3f, 0.7f, 0.7f, 1.0f), "Mare Lamentorum (Moon):");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "85-87");
            ImGui.Text("Notable locations: Bestways Burrow, Salthrack, The Seat of Sacrifice");
            DrawTeleportButton("Bestways Burrow", 175);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.3f, 0.7f, 0.7f, 1.0f), "Ultima Thule:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "88-90");
            ImGui.Text("Notable locations: Base Omicron, The Dead Ends, The Mourning Star");
            DrawTeleportButton("Base Omicron", 181);

            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.3f, 0.7f, 0.7f, 1.0f), "Elpis:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "86-88");
            ImGui.Text("Notable locations: Anagnorisis, Poieten Oikos, The Vitrified Fort");
            DrawTeleportButton("Anagnorisis", 176);
        }

        private void DrawZoneSection(Action drawContent)
        {
            ImGui.Dummy(new Vector2(0, 6));
            ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0.20f, 0.22f, 0.25f, 0.85f));
            ImGui.PushStyleColor(ImGuiCol.Border, new Vector4(0.25f, 0.45f, 0.75f, 0.5f));
            ImGui.PushStyleVar(ImGuiStyleVar.ChildBorderSize, 1.5f);
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(10, 7));
            ImGui.BeginChild(Guid.NewGuid().ToString(), new Vector2(0, 0), true, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
            drawContent();
            ImGui.EndChild();
            ImGui.PopStyleVar(2);
            ImGui.PopStyleColor(2);
            ImGui.Separator();
        }
    }
}