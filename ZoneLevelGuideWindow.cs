using Dalamud.Interface.Windowing;
using ImGuiNET;
using System.Numerics;

namespace ZoneLevels
{
    public class ZoneLevelWindow : Window
    {
        private int selectedTab = 0;
        private bool[] tabOpen;

        private readonly (string name, string levels)[] tabInfo = {
            ("Gridania", "1-30"),
            ("Limsa Lominsa", "1-50"),
            ("Ul'dah", "1-50"),
            ("Ishgard", "50-60"),
            ("Far East", "60-70"),
            ("Norvrandt", "70-80"),
            ("Ilsabard", "80-90"),
            ("Tural & Solution", "90-100")
        };

        public ZoneLevelWindow() : base(
            "Zone Level Guide",
            ImGuiWindowFlags.AlwaysAutoResize)
        {
            SizeConstraints = new WindowSizeConstraints
            {
                MinimumSize = new Vector2(600, 300),
                MaximumSize = new Vector2(1000, 800)
            };
            tabOpen = new bool[tabInfo.Length]; // Initialize in constructor
        }

        public override void Draw()
        {
            // Draw our window contents
            if (ImGui.BeginTabBar("ZoneTabs", ImGuiTabBarFlags.FittingPolicyScroll))
            {
                for (int i = 0; i < tabInfo.Length; i++)
                {
                    var tabId = $"###{i}";
                    var fullLabel = $"{tabInfo[i].name} {tabInfo[i].levels}{tabId}";
                    
                    if (ImGui.BeginTabItem(fullLabel))
                    {
                        selectedTab = i;
                        ImGui.EndTabItem();
                    }
                }
                ImGui.EndTabBar();
            }

            ImGui.Spacing(); // Add extra spacing after tabs
            ImGui.Separator();

            // Display content based on selected tab
            switch (selectedTab)
            {
                case 0: // Gridania
                    DrawGridaniaContent();
                    break;
                case 1: // Limsa Lominsa
                    DrawLimsaContent();
                    break;
                case 2: // Ul'dah
                    DrawUldahContent();
                    break;
                case 3: // Ishgard
                    DrawIshgardContent();
                    break;
                case 4: // Far East
                    DrawFarEastContent();
                    break;
                case 5: // Norvrandt
                    DrawNorvrandtContent();
                    break;
                case 6: // Ilsabard
                    DrawIlsabardContent();
                    break;
                case 7: // Dawntrail
                    DrawDawntrailContent();
                    break;
            }

            ImGui.Spacing();
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
            ImGui.Text("Notable locations: Carline Canopy, Lancers' Guild, Archers' Guild");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.0f, 0.5f, 0.0f, 1.0f), "Central Shroud:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "5-15");
            ImGui.Text("Notable locations: Bentbranch Meadows, Guardian Tree, Jadeite Thick");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.0f, 0.5f, 0.0f, 1.0f), "East Shroud:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "15-25");
            ImGui.Text("Notable locations: Hawthorne Hut, The Honey Yard, Nine Ivies");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.0f, 0.5f, 0.0f, 1.0f), "South Shroud:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "20-30");
            ImGui.Text("Notable locations: Quarrymill, Camp Tranquil, Buscarron's Druthers");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.0f, 0.5f, 0.0f, 1.0f), "North Shroud:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "15-30");
            ImGui.Text("Notable locations: Fallgourd Float, Spirithold, Gelmorra Ruins");
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
            ImGui.Text("Notable locations: Drowning Wench, Arcanists' Guild, Coral Tower");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.1f, 0.6f, 0.9f, 1.0f), "Middle La Noscea:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "5-15");
            ImGui.Text("Notable locations: Summerford Farms, Seasong Grotto, Three-malm Bend");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.1f, 0.6f, 0.9f, 1.0f), "Lower La Noscea:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "10-20");
            ImGui.Text("Notable locations: Moraby Drydocks, Cedarwood, Aleport");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.1f, 0.6f, 0.9f, 1.0f), "Western La Noscea:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "15-30");
            ImGui.Text("Notable locations: Swiftperch, Skull Valley, Halfstone");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.1f, 0.6f, 0.9f, 1.0f), "Eastern La Noscea:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "30-35");
            ImGui.Text("Notable locations: Costa del Sol, Bloodshore, Hidden Falls");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.1f, 0.6f, 0.9f, 1.0f), "Upper La Noscea:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "30-35");
            ImGui.Text("Notable locations: Camp Bronze Lake, Bronze Lake, Quarterstone");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.1f, 0.6f, 0.9f, 1.0f), "Outer La Noscea:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "40-50");
            ImGui.Text("Notable locations: Camp Overlook, U'Ghamaro Mines, Thalaos");
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
            ImGui.TextColored(new Vector4(0.9f, 0.8f, 0.1f, 1.0f), "Ul'dah - Steps of Nald/Thal:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "1-15");
            ImGui.Text("Starting Area for Gladiators, Pugilists, and Thaumaturges");
            ImGui.Text("Notable locations: Quicksand, Pugilists' Guild, Gladiators' Guild");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.9f, 0.8f, 0.1f, 1.0f), "Western Thanalan:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "5-15");
            ImGui.Text("Notable locations: Horizon, Scorpion Crossing, Footprint");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.9f, 0.8f, 0.1f, 1.0f), "Central Thanalan:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "15-20");
            ImGui.Text("Notable locations: Black Brush Station, Spineless Basin, Cactuar Cut");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.9f, 0.8f, 0.1f, 1.0f), "Eastern Thanalan:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "20-25");
            ImGui.Text("Notable locations: Camp Drybone, The Burning Wall, Sandgate");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.9f, 0.8f, 0.1f, 1.0f), "Southern Thanalan:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "35-40");
            ImGui.Text("Notable locations: Little Ala Mhigo, Forgotten Springs, Byregot's Strike");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.9f, 0.8f, 0.1f, 1.0f), "Northern Thanalan:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "45-50");
            ImGui.Text("Notable locations: Bluefog, Ceruleum Processing Plant, Camp Bluefog");
        }

        private void DrawIshgardContent()
        {
            ImGui.SetWindowFontScale(1.5f);
            ImGui.Text("Ishgard and Coerthas/Dravania - Heavensward ");
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
            ImGui.Text("Notable locations: The Forgotten Knight, Saint Reymanaud Cathedral");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(1.0f, 0.4f, 0.7f, 1.0f), "Coerthas Central Highlands:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "35-45");
            ImGui.Text("Mountainous region south of Ishgard");
            ImGui.Text("Notable locations: Camp Dragonhead, Whitebrim Front, Observatorium");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(1.0f, 0.4f, 0.7f, 1.0f), "Coerthas Western Highlands:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "50-53");
            ImGui.Text("Frozen wastes west of Ishgard");
            ImGui.Text("Notable locations: Falcon's Nest, Riversmeet, Gorgagne Holding");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(1.0f, 0.4f, 0.7f, 1.0f), "The Sea of Clouds:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "51-54");
            ImGui.Text("Series of floating islands in the sky");
            ImGui.Text("Notable locations: Camp Cloudtop, Voor Sian Siran, The Blue Window");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(1.0f, 0.4f, 0.7f, 1.0f), "The Churning Mists:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "54-56");
            ImGui.Text("Ancient home of the dragons");
            ImGui.Text("Notable locations: Moghome, Zenith, Tharl Oom Khash");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(1.0f, 0.4f, 0.7f, 1.0f), "The Dravanian Forelands:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "52-54");
            ImGui.Text("Vast wilderness of Dravania");
            ImGui.Text("Notable locations: Tailfeather, Anyx Trine, Whilom River");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(1.0f, 0.4f, 0.7f, 1.0f), "The Dravanian Hinterlands:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "56-60");
            ImGui.Text("Site of the abandoned city of Sharlayan");
            ImGui.Text("Notable locations: Idyllshire, Great Gubal Library, Matoya's Cave");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(1.0f, 0.4f, 0.7f, 1.0f), "Azys Lla:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "58-60");
            ImGui.Text("Ancient Allagan research facility");
            ImGui.Text("Notable locations: Helix, Gamma Quadrant, Flagship");
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
            ImGui.Text("Notable locations: Shiokaze Hostelry, Markets, Bokairo Inn");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.8f, 0.3f, 0.3f, 1.0f), "The Ruby Sea:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "61-63");
            ImGui.Text("Notable locations: Tamamizu, The One River, Isari");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.8f, 0.3f, 0.3f, 1.0f), "Yanxia:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "64-65");
            ImGui.Text("Notable locations: Namai, The House of the Fierce, Doma Castle");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.8f, 0.3f, 0.3f, 1.0f), "The Azim Steppe:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "65-67");
            ImGui.Text("Notable locations: Reunion, The Dawn Throne, Mol Iloh");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.8f, 0.3f, 0.3f, 1.0f), "The Fringes:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "60-62");
            ImGui.Text("Notable locations: Castrum Oriens, The Peering Stones, Rhalgr's Reach");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.8f, 0.3f, 0.3f, 1.0f), "The Peaks:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "63-64");
            ImGui.Text("Notable locations: Ala Gannha, Specula Imperatoris, The Ziggurat");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.8f, 0.3f, 0.3f, 1.0f), "The Lochs:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "67-70");
            ImGui.Text("Notable locations: Ala Mhigo, The Saltery, Porta Praetoria");
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
            ImGui.Text("Notable locations: Musica Universalis, The Wandering Stairs");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.6f, 0.4f, 0.8f, 1.0f), "Eulmore:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "70-80");
            ImGui.Text("Secondary city hub");
            ImGui.Text("Notable locations: The Canopy, The Buttress, The Derelicts");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.6f, 0.4f, 0.8f, 1.0f), "Lakeland:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "70-72");
            ImGui.Text("Notable locations: Fort Jobb, The Ostall Imperative, The Source");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.6f, 0.4f, 0.8f, 1.0f), "Kholusia:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "70-72");
            ImGui.Text("Notable locations: Stilltide, Wright, Tomra");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.6f, 0.4f, 0.8f, 1.0f), "Amh Araeng:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "75-77");
            ImGui.Text("Notable locations: Mord Souq, The Inn at Journey's Head, Twine");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.6f, 0.4f, 0.8f, 1.0f), "Il Mheg:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "73-75");
            ImGui.Text("Notable locations: Lydha Lran, Pla Enni, Wolekdorf");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.6f, 0.4f, 0.8f, 1.0f), "The Rak'tika Greatwood:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "74-76");
            ImGui.Text("Notable locations: Slitherbough, Fanow, The Ox'Dalan Gap");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.6f, 0.4f, 0.8f, 1.0f), "The Tempest:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "78-80");
            ImGui.Text("Notable locations: The Ondo Cups, Macarenses Angle, The Amaurotine Relic");
        }

        private void DrawIlsabardContent()
        {
            ImGui.SetWindowFontScale(1.5f);
            ImGui.Text("Ilsabard and Beyond - Endwalker ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "(80-90)");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.Separator();
            
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.3f, 0.7f, 0.7f, 1.0f), "Old Sharlayan:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "80-90");
            ImGui.Text("Main city hub for Endwalker");
            ImGui.Text("Notable locations: Baldesion Annex, The Studium, The Last Stand");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.3f, 0.7f, 0.7f, 1.0f), "Labyrinthos:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "81-83");
            ImGui.Text("Notable locations: The Archeion, Sharlayan Hamlet, The Twelve Wonders");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.3f, 0.7f, 0.7f, 1.0f), "Thavnair:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "80-82");
            ImGui.Text("Notable locations: Radz-at-Han, The Great Work, Palaka's Stand");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.3f, 0.7f, 0.7f, 1.0f), "Garlemald:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "83-85");
            ImGui.Text("Notable locations: Tertium, Camp Broken Glass, The Eblan Rime");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.3f, 0.7f, 0.7f, 1.0f), "Mare Lamentorum (Moon):");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "85-87");
            ImGui.Text("Notable locations: Bestways Burrow, Salthrack, The Seat of Sacrifice");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.3f, 0.7f, 0.7f, 1.0f), "Ultima Thule:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "88-90");
            ImGui.Text("Notable locations: Base Omicron, The Dead Ends, The Mourning Star");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.3f, 0.7f, 0.7f, 1.0f), "Elpis:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "86-88");
            ImGui.Text("Notable locations: Anagnorisis, Poieten Oikos, The Vitrified Fort");
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
            ImGui.Text("Notable locations: The Mamook Markets, Tuliyollal Gardens");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.0f, 0.8f, 0.8f, 1.0f), "Solution:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "97-100");
            ImGui.Text("Second major city for Dawntrail");
            ImGui.Text("Notable locations: The Circular Forum, The Misted Hall");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.0f, 0.8f, 0.8f, 1.0f), "Kozama'uka:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "94-96");
            ImGui.Text("Notable locations: The Burning Peaks, Ardorous Lake");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.0f, 0.8f, 0.8f, 1.0f), "Heritage Found:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "90-92");
            ImGui.Text("Notable locations: Urqopacha, The Singing Trees");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.0f, 0.8f, 0.8f, 1.0f), "Shaaloani:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "92-94");
            ImGui.Text("Notable locations: The Lost City, The Mystic Jungle");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.0f, 0.8f, 0.8f, 1.0f), "Yak T'el:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "91-93");
            ImGui.Text("Notable locations: Mount T'el, The Golden River");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.0f, 0.8f, 0.8f, 1.0f), "The Bluescent Deep:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "96-98");
            ImGui.Text("Notable locations: The Seafloor Ruins, The Coral Gardens");
            
            ImGui.Spacing();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.0f, 0.8f, 0.8f, 1.0f), "The Sundered Canopy:");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "Level Range: ");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), "99-100");
            ImGui.Text("End-game zone for Dawntrail");
            ImGui.Text("Notable locations: The Golden Temple, The Celestial Gates");
        }
    }
}