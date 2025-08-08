using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using ImGuiNET;
using System.Numerics;
using Dalamud.Interface.Utility;
using System;
using ZoneLevelGuide.Modules;

namespace ZoneLevelGuide
{
    public class ZoneLevelWindow : Window
    {
        // Constants for better maintainability
        private const float TabWidth = 180f;
        private const float HeaderHeight = 80f;
        private const float MinWindowWidth = 750f;
        private const float MinWindowHeight = 400f;
        private const float MaxWindowWidth = 1200f;
        private const float MaxWindowHeight = 900f;
        private const float DefaultWindowWidth = 1000f;
        private const float DefaultWindowHeight = 900f;

        // UI Colors
        private static readonly Vector4 WindowBgColor = new(0.12f, 0.12f, 0.14f, 0.98f);
        private static readonly Vector4 ChildBgColor = new(0.15f, 0.15f, 0.17f, 0.9f);
        private static readonly Vector4 PopupBgColor = new(0.12f, 0.12f, 0.14f, 0.95f);
        private static readonly Vector4 TitleColor = new(1.0f, 1.0f, 1.0f, 1.0f);
        private static readonly Vector4 SubtitleColor = new(0.8f, 0.8f, 0.8f, 0.9f);
        private static readonly Vector4 InactiveTabColor = new(0.18f, 0.18f, 0.20f, 0.7f);
        private static readonly Vector4 InactiveTabHovered = new(0.22f, 0.22f, 0.25f, 0.8f);
        private static readonly Vector4 InactiveTabActive = new(0.25f, 0.25f, 0.28f, 0.9f);
        private static readonly Vector4 InactiveTabText = new(0.8f, 0.8f, 0.8f, 0.9f);
        private static readonly Vector4 ActiveTabText = new(0.95f, 0.95f, 0.95f, 1.0f);

        // Instance variables
        private int selectedTab = 0;
        private bool[] tabOpen;
        private readonly ITeleporterIpc? teleporter;
        private readonly IZoneModule[] zoneModules;

        private readonly (string name, string levels, Vector4 color)[] tabInfo = {
            ("★ Favorites", "Quick Access", new Vector4(1.0f, 0.8f, 0.2f, 1.0f)),
            ("Gridania", "1-30", new Vector4(0.4f, 0.8f, 0.4f, 1.0f)),
            ("Limsa Lominsa", "1-50", new Vector4(0.4f, 0.7f, 1.0f, 1.0f)),
            ("Ul'dah", "1-50", new Vector4(1.0f, 0.8f, 0.4f, 1.0f)),
            ("Mor Dhona", "45-50", new Vector4(0.8f, 0.6f, 0.9f, 1.0f)),
            ("Coerthas", "35-53", new Vector4(0.9f, 0.9f, 1.0f, 1.0f)),
            ("Ishgard", "50-60", new Vector4(1.0f, 0.6f, 0.8f, 1.0f)),
            ("Far East", "60-70", new Vector4(0.9f, 0.5f, 0.5f, 1.0f)),
            ("Norvrandt", "70-80", new Vector4(0.7f, 0.5f, 0.9f, 1.0f)),
            ("Ilsabard", "80-90", new Vector4(0.5f, 0.8f, 0.8f, 1.0f)),
            ("Tural", "90-100", new Vector4(0.5f, 0.9f, 0.9f, 1.0f)),
            ("Housing", "Any Level", new Vector4(0.8f, 0.6f, 0.4f, 1.0f)),
            ("PvP", "Level 30+", new Vector4(0.9f, 0.4f, 0.4f, 1.0f))
        };

        public ZoneLevelWindow(ITeleporterIpc? teleporter = null) : base(
            "Zone Level Guide",
            ImGuiWindowFlags.NoCollapse)
        {
            this.teleporter = teleporter;

            // Initialize zone modules
            var favoritesModule = new FavoritesModule(teleporter);
            BaseZoneModule.FavoritesManager = favoritesModule;
            
            zoneModules = new IZoneModule[]
            {
                favoritesModule,
                new GridaniaModule(teleporter),
                new LimsaModule(teleporter),
                new UldahModule(teleporter),
                new MorDhonaModule(teleporter),
                new CoerthasModule(teleporter),
                new IshgardModule(teleporter),
                new FarEastModule(teleporter),
                new NorvrandtModule(teleporter),
                new IlsabardModule(teleporter),
                new TuralModule(teleporter),
                new HousingModule(teleporter),
                new PvPModule(teleporter)
            };

            SetupWindow();
            tabOpen = new bool[tabInfo.Length];
        }

        private void SetupWindow()
        {
            SizeConstraints = new WindowSizeConstraints
            {
                MinimumSize = new Vector2(MinWindowWidth, MinWindowHeight),
                MaximumSize = new Vector2(MaxWindowWidth, MaxWindowHeight)
            };

            this.Size = new Vector2(DefaultWindowWidth, DefaultWindowHeight);
            this.SizeCondition = ImGuiCond.FirstUseEver;
        }

        public override void Draw()
        {
            ApplyTheme();
            
            DrawHeader();
            DrawSeparator();
            DrawMainLayout();
            
            PopTheme();
        }

        private void ApplyTheme()
        {
            ImGui.PushStyleColor(ImGuiCol.WindowBg, WindowBgColor);
            ImGui.PushStyleColor(ImGuiCol.ChildBg, ChildBgColor);
            ImGui.PushStyleColor(ImGuiCol.PopupBg, PopupBgColor);
        }

        private void PopTheme()
        {
            ImGui.PopStyleColor(3);
        }

        private void DrawSeparator()
        {
            ImGui.Spacing();
            ImGui.Separator();
            ImGui.Spacing();
        }

        private void DrawMainLayout()
        {
            var windowSize = ImGui.GetWindowSize();
            var contentHeight = windowSize.Y - HeaderHeight;

            // Left sidebar
            ImGui.BeginGroup();
            {
                ImGui.BeginChild("##Sidebar", new Vector2(TabWidth, contentHeight), true, 
                    ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
                DrawTabSidebar();
                ImGui.EndChild();
            }
            ImGui.EndGroup();

            ImGui.SameLine();

            // Main content area
            ImGui.BeginGroup();
            {
                ImGui.BeginChild("##MainContent", new Vector2(0, contentHeight), true, ImGuiWindowFlags.None);
                DrawMainContent();
                ImGui.EndChild();
            }
            ImGui.EndGroup();
        }

        private void DrawHeader()
        {
            ImGui.PushFont(ImGui.GetIO().Fonts.Fonts[0]);
            
            var windowWidth = ImGui.GetWindowWidth();
            
            // Title
            DrawCenteredText("FINAL FANTASY XIV - Zone Level Guide", windowWidth, TitleColor, 1.4f);
            
            // Subtitle
            var subtitleText = teleporter != null ? 
                "Click any location to teleport - (Requires Teleporter plugin)" : 
                "Install Teleporter plugin to enable fast travel";
            DrawCenteredText(subtitleText, windowWidth, SubtitleColor);
            
            ImGui.PopFont();
        }

        private void DrawCenteredText(string text, float windowWidth, Vector4 color, float fontScale = 1.0f)
        {
            ImGui.PushStyleColor(ImGuiCol.Text, color);
            
            if (fontScale != 1.0f)
                ImGui.SetWindowFontScale(fontScale);
            
            var textWidth = ImGui.CalcTextSize(text).X;
            ImGui.SetCursorPosX((windowWidth - textWidth) * 0.5f);
            ImGui.Text(text);
            
            if (fontScale != 1.0f)
                ImGui.SetWindowFontScale(1.0f);
            
            ImGui.PopStyleColor();
        }

        private void DrawTabSidebar()
        {
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(12, 10));
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 3));
            ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 2.0f);
            
            // Draw Favorites tab first (index 0)
            DrawTabButton(0);
            
            // Add divider after Favorites
            ImGui.Spacing();
            ImGui.PushStyleColor(ImGuiCol.Separator, new Vector4(1.0f, 0.8f, 0.2f, 0.8f)); // Golden separator
            ImGui.Separator();
            ImGui.PopStyleColor();
            ImGui.Spacing();
            
            // Draw main zone tabs (indexes 1 through tabInfo.Length - 3)
            for (int i = 1; i < tabInfo.Length - 2; i++)
            {
                DrawTabButton(i);
            }
            
            // Add divider before housing and PvP
            ImGui.Spacing();
            ImGui.PushStyleColor(ImGuiCol.Separator, new Vector4(0.5f, 0.5f, 0.5f, 0.8f));
            ImGui.Separator();
            ImGui.PopStyleColor();
            ImGui.Spacing();
            
            // Draw housing tab (second to last)
            DrawTabButton(tabInfo.Length - 2);
            
            // Draw PvP tab (last one)
            DrawTabButton(tabInfo.Length - 1);
            
            ImGui.PopStyleVar(3);
        }

        private void DrawTabButton(int index)
        {
            var tab = tabInfo[index];
            bool isSelected = (selectedTab == index);
            
            ApplyTabButtonColors(tab, isSelected);
            
            if (ImGui.Button($"{tab.name}##tab_{index}", new Vector2(-1, 0)))
            {
                selectedTab = index;
            }
            
            ImGui.PopStyleColor(4);
        }

        private void ApplyTabButtonColors((string name, string levels, Vector4 color) tab, bool isSelected)
        {
            if (isSelected)
            {
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(tab.color.X * 0.2f, tab.color.Y * 0.2f, tab.color.Z * 0.2f, 0.9f));
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(tab.color.X * 0.25f, tab.color.Y * 0.25f, tab.color.Z * 0.25f, 1.0f));
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(tab.color.X * 0.3f, tab.color.Y * 0.3f, tab.color.Z * 0.3f, 1.0f));
                ImGui.PushStyleColor(ImGuiCol.Text, ActiveTabText);
            }
            else
            {
                ImGui.PushStyleColor(ImGuiCol.Button, InactiveTabColor);
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, InactiveTabHovered);
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, InactiveTabActive);
                ImGui.PushStyleColor(ImGuiCol.Text, InactiveTabText);
            }
        }

        private void DrawMainContent()
        {
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(20, 20));
            
            // Create scrollable content area
            ImGui.BeginChild("##ScrollableContent", new Vector2(0, 0), false, ImGuiWindowFlags.None);
            
            if (selectedTab >= 0 && selectedTab < zoneModules.Length)
            {
                zoneModules[selectedTab].DrawContent();
            }
            
            ImGui.EndChild();
            ImGui.PopStyleVar();
        }
    }
}