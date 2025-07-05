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
        private bool[] tabOpen;
        private readonly ITeleporterIpc? teleporter;

        private readonly (string name, string levels, Vector4 color)[] tabInfo = {
            ("Gridania", "1-30", new Vector4(0.4f, 0.8f, 0.4f, 1.0f)),
            ("Limsa Lominsa", "1-50", new Vector4(0.4f, 0.7f, 1.0f, 1.0f)),
            ("Ul'dah", "1-50", new Vector4(1.0f, 0.8f, 0.4f, 1.0f)),
            ("Mor Dhona", "45-50", new Vector4(0.8f, 0.6f, 0.9f, 1.0f)),
            ("Coerthas", "35-53", new Vector4(0.9f, 0.9f, 1.0f, 1.0f)),
            ("Ishgard", "50-60", new Vector4(1.0f, 0.6f, 0.8f, 1.0f)),
            ("Far East", "60-70", new Vector4(0.9f, 0.5f, 0.5f, 1.0f)),
            ("Norvrandt", "70-80", new Vector4(0.7f, 0.5f, 0.9f, 1.0f)),
            ("Ilsabard", "80-90", new Vector4(0.5f, 0.8f, 0.8f, 1.0f)),
            ("Tural", "90-100", new Vector4(0.5f, 0.9f, 0.9f, 1.0f))
        };

        public ZoneLevelWindow(ITeleporterIpc? teleporter = null) : base(
            "Zone Level Guide",
            ImGuiWindowFlags.NoCollapse)
        {
            this.teleporter = teleporter;
            SizeConstraints = new WindowSizeConstraints
            {
                MinimumSize = new Vector2(750, 400),
                MaximumSize = new Vector2(1200, 800)
            };
            tabOpen = new bool[tabInfo.Length];
        }

        public override void Draw()
        {
            // Professional dark theme with better contrast
            ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0.12f, 0.12f, 0.14f, 0.98f));
            ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0.15f, 0.15f, 0.17f, 0.9f));
            ImGui.PushStyleColor(ImGuiCol.PopupBg, new Vector4(0.12f, 0.12f, 0.14f, 0.95f));
            
            // Header
            DrawHeader();
            
            ImGui.Spacing();
            ImGui.Separator();
            ImGui.Spacing();

            // Main layout - narrower sidebar with reduced height calculation
            float tabWidth = 180f;
            var windowSize = ImGui.GetWindowSize();
            
            ImGui.BeginGroup();
            {
                // Left sidebar with tabs - no scrolling allowed
                ImGui.BeginChild("##Sidebar", new Vector2(tabWidth, windowSize.Y - 80), true, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
                DrawTabSidebar();
                ImGui.EndChild();
            }
            ImGui.EndGroup();

            ImGui.SameLine();

            ImGui.BeginGroup();
            {
                // Main content area - no scrolling allowed
                ImGui.BeginChild("##MainContent", new Vector2(0, windowSize.Y - 80), true, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
                DrawMainContent();
                ImGui.EndChild();
            }
            ImGui.EndGroup();

            ImGui.PopStyleColor(3);
        }

        private void DrawHeader()
        {
            ImGui.PushFont(ImGui.GetIO().Fonts.Fonts[0]); // Use default font but larger
            
            // Title
            var titleColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            ImGui.PushStyleColor(ImGuiCol.Text, titleColor);
            ImGui.SetWindowFontScale(1.4f);
            
            var windowWidth = ImGui.GetWindowWidth();
            var titleText = "FINAL FANTASY XIV - Zone Level Guide";
            var titleWidth = ImGui.CalcTextSize(titleText).X;
            ImGui.SetCursorPosX((windowWidth - titleWidth) * 0.5f);
            ImGui.Text(titleText);
            
            ImGui.SetWindowFontScale(1.0f);
            ImGui.PopStyleColor();
            
            // Subtitle
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.8f, 0.8f, 0.8f, 0.9f));
            var subtitleText = teleporter != null ? 
                "Click any location to teleport • Requires Teleporter plugin" : 
                "Install Teleporter plugin to enable fast travel";
            var subtitleWidth = ImGui.CalcTextSize(subtitleText).X;
            ImGui.SetCursorPosX((windowWidth - subtitleWidth) * 0.5f);
            ImGui.Text(subtitleText);
            ImGui.PopStyleColor();
            
            ImGui.PopFont();
        }

        private void DrawTabSidebar()
        {
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(12, 10));
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 3));
            ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 2.0f);
            
            for (int i = 0; i < tabInfo.Length; i++)
            {
                var tab = tabInfo[i];
                bool isSelected = (selectedTab == i);
                
                // Professional tab styling
                if (isSelected)
                {
                    ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(tab.color.X * 0.2f, tab.color.Y * 0.2f, tab.color.Z * 0.2f, 0.9f));
                    ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(tab.color.X * 0.25f, tab.color.Y * 0.25f, tab.color.Z * 0.25f, 1.0f));
                    ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(tab.color.X * 0.3f, tab.color.Y * 0.3f, tab.color.Z * 0.3f, 1.0f));
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.95f, 0.95f, 0.95f, 1.0f));
                }
                else
                {
                    ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.18f, 0.18f, 0.20f, 0.7f));
                    ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.22f, 0.22f, 0.25f, 0.8f));
                    ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.25f, 0.25f, 0.28f, 0.9f));
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.8f, 0.8f, 0.8f, 0.9f));
                }
                
                if (ImGui.Button($"{tab.name}##tab_{i}", new Vector2(-1, 0)))
                {
                    selectedTab = i;
                }
                
                ImGui.PopStyleColor(4);
            }
            
            ImGui.PopStyleVar(3);
        }

        private void DrawMainContent()
        {
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(20, 20));
            
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
            
            ImGui.PopStyleVar();
        }

        private void DrawTeleportButton(string locationName, uint aetheryteId)
        {
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(10, 6));
            ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 3.0f);
            
            if (teleporter != null)
            {
                // Softer gray-blue theme that blends better with dark design
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.35f, 0.38f, 0.42f, 0.8f));
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.42f, 0.45f, 0.50f, 0.9f));
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.48f, 0.52f, 0.58f, 1.0f));
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.95f, 0.95f, 0.95f, 1.0f));

                bool pressed = ImGui.Button($"🗺️ {locationName}###tp_{aetheryteId}");

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
                ImGui.Button($"📍 {locationName}###disabled_{aetheryteId}");
                ImGui.PopStyleColor(2);
            }
            
            ImGui.PopStyleVar(2);
        }

        private void DrawZoneHeader(string zoneName, string levelRange, Vector4 color)
        {
            // Lighter, more readable color variant
            var headerColor = new Vector4(
                Math.Min(color.X + 0.3f, 1.0f),
                Math.Min(color.Y + 0.3f, 1.0f),
                Math.Min(color.Z + 0.3f, 1.0f),
                1.0f
            );
            
            ImGui.PushStyleColor(ImGuiCol.Text, headerColor);
            ImGui.SetWindowFontScale(1.2f);
            ImGui.Text(zoneName);
            ImGui.SetWindowFontScale(1.0f);
            ImGui.PopStyleColor();
            
            ImGui.SameLine();
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.85f, 0.85f, 0.85f, 1.0f));
            ImGui.Text($"({levelRange})");
            ImGui.PopStyleColor();
            
            ImGui.Spacing();
            ImGui.Separator();
            ImGui.Spacing();
        }

        private void DrawZoneSection(string sectionName, string levelRange, string description, Action drawButtons, Vector4 color)
        {
            ImGui.Spacing();
            
            // Section header with better readability
            var sectionColor = new Vector4(
                Math.Min(color.X + 0.2f, 0.95f),
                Math.Min(color.Y + 0.2f, 0.95f),
                Math.Min(color.Z + 0.2f, 0.95f),
                1.0f
            );
            
            ImGui.PushStyleColor(ImGuiCol.Text, sectionColor);
            ImGui.SetWindowFontScale(1.05f);
            ImGui.Text($"🏛️ {sectionName}");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.PopStyleColor();
            
            ImGui.SameLine();
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.9f, 0.3f, 1.0f)); // Softer yellow
            ImGui.Text($"Level {levelRange}");
            ImGui.PopStyleColor();
            
            if (!string.IsNullOrEmpty(description))
            {
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.9f, 0.9f, 0.9f, 1.0f)); // Softer white
                ImGui.TextWrapped(description);
                ImGui.PopStyleColor();
                ImGui.Spacing();
            }
            
            // Buttons with better spacing
            ImGui.Indent(10.0f);
            drawButtons();
            ImGui.Unindent(10.0f);
            
            ImGui.Spacing();
        }

        private void DrawGridaniaContent()
        {
            DrawZoneHeader("Gridania and The Black Shroud", "1-30", tabInfo[0].color);
            
            DrawZoneSection("New Gridania & Old Gridania", "1-15", 
                "Starting area for Conjurers, Archers, and Lancers",
                () => DrawTeleportButton("New Gridania", 2), tabInfo[0].color);
            
            DrawZoneSection("Central Shroud", "5-15",
                "",
                () => DrawTeleportButton("Bentbranch Meadows", 3), tabInfo[0].color);
            
            DrawZoneSection("East Shroud", "15-25",
                "",
                () => DrawTeleportButton("The Hawthorne Hut", 4), tabInfo[0].color);
            
            DrawZoneSection("South Shroud", "20-30",
                "",
                () => DrawTeleportButton("Quarrymill", 5), tabInfo[0].color);
            
            DrawZoneSection("North Shroud", "15-30",
                "",
                () => {
                    DrawTeleportButton("Camp Tranquil", 6);
                    ImGui.SameLine();
                    DrawTeleportButton("Fallgourd Float", 7);
                }, tabInfo[0].color);
        }

        private void DrawLimsaContent()
        {
            DrawZoneHeader("Limsa Lominsa and La Noscea", "1-50", tabInfo[1].color);
            
            DrawZoneSection("Limsa Lominsa", "1-15",
                "Starting area for Marauders, Arcanists, and Rogues",
                () => DrawTeleportButton("Limsa Lominsa Lower Decks", 8), tabInfo[1].color);
            
            DrawZoneSection("Lower La Noscea", "10-20",
                "",
                () => {
                    DrawTeleportButton("Moraby Drydocks", 10);
                    ImGui.SameLine();
                    DrawTeleportButton("Aleport", 14);
                    ImGui.SameLine();
                    DrawTeleportButton("Summerford Farms", 52);
                }, tabInfo[1].color);
            
            DrawZoneSection("Middle La Noscea", "15-25",
                "",
                () => DrawTeleportButton("Wineport", 12), tabInfo[1].color);
            
            DrawZoneSection("Western La Noscea", "15-30",
                "",
                () => DrawTeleportButton("Swiftperch", 13), tabInfo[1].color);
            
            DrawZoneSection("Eastern La Noscea", "30-35",
                "",
                () => DrawTeleportButton("Costa del Sol", 11), tabInfo[1].color);
            
            DrawZoneSection("Upper La Noscea", "30-35",
                "",
                () => DrawTeleportButton("Camp Bronze Lake", 15), tabInfo[1].color);
            
            DrawZoneSection("Outer La Noscea", "40-50",
                "",
                () => {
                    DrawTeleportButton("Camp Overlook", 16);
                    ImGui.SameLine();
                    DrawTeleportButton("Ceruleum Processing Plant", 22);
                }, tabInfo[1].color);
        }

        private void DrawUldahContent()
        {
            DrawZoneHeader("Ul'dah and Thanalan", "1-50", tabInfo[2].color);
            
            DrawZoneSection("Ul'dah - Steps of Nald/Thal", "1-15",
                "Starting area for Gladiators, Pugilists, and Thaumaturges",
                () => DrawTeleportButton("Ul'dah - Steps of Nald", 9), tabInfo[2].color);
            
            DrawZoneSection("Western Thanalan", "5-15",
                "",
                () => {
                    DrawTeleportButton("Horizon", 17);
                    ImGui.SameLine();
                    DrawTeleportButton("Black Brush Station", 53);
                }, tabInfo[2].color);
            
            DrawZoneSection("Eastern Thanalan", "20-25",
                "",
                () => DrawTeleportButton("Camp Drybone", 18), tabInfo[2].color);
            
            DrawZoneSection("Southern Thanalan", "35-40",
                "",
                () => {
                    DrawTeleportButton("Little Ala Mhigo", 19);
                    ImGui.SameLine();
                    DrawTeleportButton("Forgotten Springs", 20);
                }, tabInfo[2].color);
            
            DrawZoneSection("Northern Thanalan", "45-50",
                "",
                () => DrawTeleportButton("Camp Bluefog", 21), tabInfo[2].color);
            
            DrawZoneSection("Gold Saucer", "15+",
                "Entertainment district",
                () => DrawTeleportButton("The Gold Saucer", 61), tabInfo[2].color);
        }

        private void DrawMorDhonaContent()
        {
            DrawZoneHeader("Mor Dhona", "45-50", tabInfo[3].color);
            
            DrawZoneSection("Mor Dhona", "45-50",
                "Gateway to Heavensward content and endgame hub",
                () => DrawTeleportButton("Revenant's Toll", 24), tabInfo[3].color);
        }

        private void DrawCoerthasContent()
        {
            DrawZoneHeader("Coerthas - Pre-Calamity Regions", "35-53", tabInfo[4].color);
            
            DrawZoneSection("Coerthas Central Highlands", "35-45",
                "Mountainous region south of Ishgard",
                () => DrawTeleportButton("Camp Dragonhead", 23), tabInfo[4].color);
            
            DrawZoneSection("Coerthas Western Highlands", "50-53",
                "Frozen wastes west of Ishgard",
                () => DrawTeleportButton("Falcon's Nest", 71), tabInfo[4].color);
        }

        private void DrawIshgardContent()
        {
            DrawZoneHeader("Ishgard and Dravania - Heavensward", "50-60", tabInfo[5].color);
            
            DrawZoneSection("Foundation & The Pillars", "50-60",
                "City of Ishgard - Heavensward starting area",
                () => DrawTeleportButton("Foundation", 70), tabInfo[5].color);
            
            DrawZoneSection("Coerthas Western Highlands", "50-53",
                "",
                () => DrawTeleportButton("Falcon's Nest", 71), tabInfo[5].color);
            
            DrawZoneSection("The Sea of Clouds", "51-54",
                "Series of floating islands in the sky",
                () => {
                    DrawTeleportButton("Camp Cloudtop", 72);
                    ImGui.SameLine();
                    DrawTeleportButton("Ok' Zundu", 73);
                }, tabInfo[5].color);
            
            DrawZoneSection("The Churning Mists", "54-56",
                "Ancient home of the dragons",
                () => {
                    DrawTeleportButton("Moghome", 78);
                    ImGui.SameLine();
                    DrawTeleportButton("Zenith", 79);
                }, tabInfo[5].color);
            
            DrawZoneSection("The Dravanian Forelands", "52-54",
                "Vast wilderness of Dravania",
                () => {
                    DrawTeleportButton("Tailfeather", 76);
                    ImGui.SameLine();
                    DrawTeleportButton("Anyx Trine", 77);
                }, tabInfo[5].color);
            
            DrawZoneSection("The Dravanian Hinterlands", "56-60",
                "Site of the abandoned city of Sharlayan",
                () => DrawTeleportButton("Idyllshire", 75), tabInfo[5].color);
            
            DrawZoneSection("Azys Lla", "58-60",
                "Ancient Allagan research facility",
                () => DrawTeleportButton("Helix", 74), tabInfo[5].color);
        }

        private void DrawFarEastContent()
        {
            DrawZoneHeader("The Far East - Stormblood", "60-70", tabInfo[6].color);
            
            DrawZoneSection("Kugane", "60-70",
                "Main city hub for Stormblood",
                () => DrawTeleportButton("Kugane", 111), tabInfo[6].color);
            
            DrawZoneSection("The Ruby Sea", "61-63",
                "",
                () => {
                    DrawTeleportButton("Tamamizu", 105);
                    ImGui.SameLine();
                    DrawTeleportButton("Onokoro", 106);
                }, tabInfo[6].color);
            
            DrawZoneSection("Yanxia", "64-65",
                "",
                () => {
                    DrawTeleportButton("Namai", 107);
                    ImGui.SameLine();
                    DrawTeleportButton("The House of the Fierce", 108);
                }, tabInfo[6].color);
            
            DrawZoneSection("The Azim Steppe", "65-67",
                "",
                () => {
                    DrawTeleportButton("Reunion", 109);
                    ImGui.SameLine();
                    DrawTeleportButton("The Dawn Throne", 110);
                    ImGui.SameLine();
                    DrawTeleportButton("Dhoro Iloh", 128);
                }, tabInfo[6].color);
            
            DrawZoneSection("The Fringes", "60-62",
                "",
                () => {
                    DrawTeleportButton("Rhalgr's Reach", 104);
                    ImGui.SameLine();
                    DrawTeleportButton("Castrum Oriens", 98);
                }, tabInfo[6].color);
            
            DrawZoneSection("The Peaks", "63-64",
                "",
                () => {
                    DrawTeleportButton("Ala Gannha", 100);
                    ImGui.SameLine();
                    DrawTeleportButton("Ala Ghiri", 101);
                }, tabInfo[6].color);
            
            DrawZoneSection("The Lochs", "67-70",
                "",
                () => {
                    DrawTeleportButton("The Ala Mhigan Quarter", 103);
                    ImGui.SameLine();
                    DrawTeleportButton("Porta Praetoria", 102);
                    ImGui.SameLine();
                    DrawTeleportButton("The Peering Stones", 99);
                }, tabInfo[6].color);
            
            DrawZoneSection("Other Areas", "60-70",
                "",
                () => DrawTeleportButton("The Doman Enclave", 127), tabInfo[6].color);
        }

        private void DrawNorvrandtContent()
        {
            DrawZoneHeader("Norvrandt - Shadowbringers", "70-80", tabInfo[7].color);
            
            DrawZoneSection("The Crystarium", "70-80",
                "Main city hub for Shadowbringers",
                () => DrawTeleportButton("The Crystarium", 133), tabInfo[7].color);
            
            DrawZoneSection("Eulmore", "70-80",
                "Secondary city hub",
                () => DrawTeleportButton("Eulmore", 134), tabInfo[7].color);
            
            DrawZoneSection("Lakeland", "70-72",
                "",
                () => {
                    DrawTeleportButton("Fort Jobb", 132);
                    ImGui.SameLine();
                    DrawTeleportButton("The Ostall Imperative", 136);
                }, tabInfo[7].color);
            
            DrawZoneSection("Kholusia", "70-72",
                "",
                () => {
                    DrawTeleportButton("Stilltide", 137);
                    ImGui.SameLine();
                    DrawTeleportButton("Wright", 138);
                    ImGui.SameLine();
                    DrawTeleportButton("Tomra", 139);
                }, tabInfo[7].color);
            
            DrawZoneSection("Amh Araeng", "75-77",
                "",
                () => {
                    DrawTeleportButton("Mord Souq", 140);
                    ImGui.SameLine();
                    DrawTeleportButton("Twine", 141);
                    ImGui.SameLine();
                    DrawTeleportButton("The Inn at Journey's Head", 161);
                }, tabInfo[7].color);
            
            DrawZoneSection("Il Mheg", "73-75",
                "",
                () => {
                    DrawTeleportButton("Lydha Lran", 144);
                    ImGui.SameLine();
                    DrawTeleportButton("Pla Enni", 145);
                    ImGui.SameLine();
                    DrawTeleportButton("Wolekdorf", 146);
                }, tabInfo[7].color);
            
            DrawZoneSection("The Rak'tika Greatwood", "74-76",
                "",
                () => {
                    DrawTeleportButton("Slitherbough", 142);
                    ImGui.SameLine();
                    DrawTeleportButton("Fanow", 143);
                }, tabInfo[7].color);
            
            DrawZoneSection("The Tempest", "78-80",
                "",
                () => {
                    DrawTeleportButton("The Ondo Cups", 147);
                    ImGui.SameLine();
                    DrawTeleportButton("The Macareneses Angle", 148);
                }, tabInfo[7].color);
        }

        private void DrawIlsabardContent()
        {
            DrawZoneHeader("Ilsabard and Beyond - Endwalker", "80-90", tabInfo[8].color);
            
            DrawZoneSection("Old Sharlayan", "80-90",
                "Main city hub for Endwalker",
                () => DrawTeleportButton("Old Sharlayan", 182), tabInfo[8].color);
            
            DrawZoneSection("Radz-at-Han", "80-90",
                "Secondary city hub for Endwalker",
                () => DrawTeleportButton("Radz-at-Han", 183), tabInfo[8].color);
            
            DrawZoneSection("Labyrinthos", "81-83",
                "",
                () => {
                    DrawTeleportButton("The Archeion", 166);
                    ImGui.SameLine();
                    DrawTeleportButton("Sharlayan Hamlet", 167);
                    ImGui.SameLine();
                    DrawTeleportButton("Aporia", 168);
                }, tabInfo[8].color);
            
            DrawZoneSection("Thavnair", "80-82",
                "",
                () => {
                    DrawTeleportButton("Yedlihmad", 169);
                    ImGui.SameLine();
                    DrawTeleportButton("The Great Work", 170);
                    ImGui.SameLine();
                    DrawTeleportButton("Palaka's Stand", 171);
                }, tabInfo[8].color);
            
            DrawZoneSection("Garlemald", "83-85",
                "",
                () => {
                    DrawTeleportButton("Tertium", 173);
                    ImGui.SameLine();
                    DrawTeleportButton("Camp Broken Glass", 172);
                }, tabInfo[8].color);
            
            DrawZoneSection("Mare Lamentorum (Moon)", "85-87",
                "",
                () => {
                    DrawTeleportButton("Bestways Burrow", 175);
                    ImGui.SameLine();
                    DrawTeleportButton("Sinus Lacrimarum", 174);
                }, tabInfo[8].color);
            
            DrawZoneSection("Ultima Thule", "88-90",
                "",
                () => {
                    DrawTeleportButton("Base Omicron", 181);
                    ImGui.SameLine();
                    DrawTeleportButton("Abode of the Ea", 180);
                    ImGui.SameLine();
                    DrawTeleportButton("Reah Tahra", 179);
                }, tabInfo[8].color);
            
            DrawZoneSection("Elpis", "86-88",
                "",
                () => {
                    DrawTeleportButton("Anagnorisis", 176);
                    ImGui.SameLine();
                    DrawTeleportButton("The Twelve Wonders", 177);
                    ImGui.SameLine();
                    DrawTeleportButton("Poieten Oikos", 178);
                }, tabInfo[8].color);
        }

        private void DrawDawntrailContent()
        {
            DrawZoneHeader("Tural - Dawntrail", "90-100", tabInfo[9].color);
            
            DrawZoneSection("Tuliyollal", "90-91",
                "Main city hub for Dawntrail",
                () => DrawTeleportButton("Tuliyollal", 216), tabInfo[9].color);
            
            DrawZoneSection("Solution Nine", "97-100",
                "Second major city for Dawntrail",
                () => DrawTeleportButton("Solution Nine", 217), tabInfo[9].color);
            
            DrawZoneSection("Urqopacha", "90-92",
                "",
                () => {
                    DrawTeleportButton("Wachunpelo", 200);
                    ImGui.SameLine();
                    DrawTeleportButton("Worlar's Echo", 201);
                }, tabInfo[9].color);
            
            DrawZoneSection("Kozama'uka", "94-96",
                "",
                () => {
                    DrawTeleportButton("Ok'hanu", 202);
                    ImGui.SameLine();
                    DrawTeleportButton("Many Fires", 203);
                    ImGui.SameLine();
                    DrawTeleportButton("Earthenshire", 204);
                    ImGui.SameLine();
                    DrawTeleportButton("Dock Poga", 238);
                }, tabInfo[9].color);
            
            DrawZoneSection("Yak T'el", "92-94",
                "",
                () => {
                    DrawTeleportButton("Iq Br'aax", 205);
                    ImGui.SameLine();
                    DrawTeleportButton("Mamook", 206);
                }, tabInfo[9].color);
            
            DrawZoneSection("Shaaloani", "93-95",
                "",
                () => {
                    DrawTeleportButton("Hhusatahwi", 207);
                    ImGui.SameLine();
                    DrawTeleportButton("Sheshenewezi Springs", 208);
                    ImGui.SameLine();
                    DrawTeleportButton("Mehwahhetsoan", 209);
                }, tabInfo[9].color);
            
            DrawZoneSection("Heritage Found", "96-98",
                "",
                () => {
                    DrawTeleportButton("Electrope Strike", 212);
                    ImGui.SameLine();
                    DrawTeleportButton("Yyasulani Station", 210);
                    ImGui.SameLine();
                    DrawTeleportButton("The Outskirts", 211);
                }, tabInfo[9].color);
            
            DrawZoneSection("Living Memory", "99-100",
                "",
                () => {
                    DrawTeleportButton("Leynode Mnemo", 213);
                    ImGui.SameLine();
                    DrawTeleportButton("Leynode Pyro", 214);
                    ImGui.SameLine();
                    DrawTeleportButton("Leynode Aero", 215);
                }, tabInfo[9].color);
        }
    }
}