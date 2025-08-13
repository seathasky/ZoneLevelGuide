using Dalamud.Bindings.ImGui;
using System.Numerics;

namespace ZoneLevelGuide.Modules
{
    public class IlsabardModule : BaseZoneModule
    {
        public override string ZoneName => "Ilsabard";
        public override string LevelRange => "80-90";
        public override Vector4 Color => new Vector4(0.5f, 0.8f, 0.8f, 1.0f);

        public IlsabardModule(ITeleporterIpc? teleporter) : base(teleporter) { }

        public override void DrawContent()
        {
            DrawZoneHeader("Ilsabard and Beyond - Endwalker", "80-90", Color);
            
            DrawZoneSection("Old Sharlayan", "80-90",
                "Scholarly city-state of the Forum and main hub for Endwalker's journey.",
                () => DrawTeleportButton("Old Sharlayan", 182), Color);
            
            DrawZoneSection("Radz-at-Han", "80-90",
                "Merchant city of the Near East with diverse cultures and trade routes.",
                () => DrawTeleportButton("Radz-at-Han", 183), Color);
            
            DrawZoneSection("Labyrinthos", "81-83",
                "Sharlayan's underground research facility for preserving life and knowledge.",
                () => {
                    DrawTeleportButton("The Archeion", 166);
                    ImGui.SameLine();
                    DrawTeleportButton("Sharlayan Hamlet", 167);
                    ImGui.SameLine();
                    DrawTeleportButton("Aporia", 168);
                }, Color);
            
            DrawZoneSection("Thavnair", "80-82",
                "Island nation known for alchemy, trade, and now facing the Final Days.",
                () => {
                    DrawTeleportButton("Yedlihmad", 169);
                    ImGui.SameLine();
                    DrawTeleportButton("The Great Work", 170);
                    ImGui.SameLine();
                    DrawTeleportButton("Palaka's Stand", 171);
                }, Color);
            
            DrawZoneSection("Garlemald", "83-85",
                "Frozen heart of the fallen Empire, ravaged by civil war and despair.",
                () => {
                    DrawTeleportButton("Tertium", 173);
                    ImGui.SameLine();
                    DrawTeleportButton("Camp Broken Glass", 172);
                }, Color);
            
            DrawZoneSection("Mare Lamentorum (Moon)", "85-87",
                "Lunar surface with ancient Loporrits preparing for a new beginning.",
                () => {
                    DrawTeleportButton("Bestways Burrow", 175);
                    ImGui.SameLine();
                    DrawTeleportButton("Sinus Lacrimarum", 174);
                }, Color);
            
            DrawZoneSection("Ultima Thule", "88-90",
                "Edge of the universe where hope and despair converge in the final battle.",
                () => {
                    DrawTeleportButton("Base Omicron", 181);
                    ImGui.SameLine();
                    DrawTeleportButton("Abode of the Ea", 180);
                    ImGui.SameLine();
                    DrawTeleportButton("Reah Tahra", 179);
                }, Color);
            
            DrawZoneSection("Elpis", "86-88",
                "Ancient world of creation where the Ancients shaped life itself.",
                () => {
                    DrawTeleportButton("Anagnorisis", 176);
                    ImGui.SameLine();
                    DrawTeleportButton("The Twelve Wonders", 177);
                    ImGui.SameLine();
                    DrawTeleportButton("Poieten Oikos", 178);
                }, Color);
        }
    }
}
