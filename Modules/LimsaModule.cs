using ImGuiNET;
using System.Numerics;

namespace ZoneLevelGuide.Modules
{
    public class LimsaModule : BaseZoneModule
    {
        public override string ZoneName => "Limsa Lominsa";
        public override string LevelRange => "1-50";
        public override Vector4 Color => new Vector4(0.4f, 0.7f, 1.0f, 1.0f);

        public LimsaModule(ITeleporterIpc? teleporter) : base(teleporter) { }

        public override void DrawContent()
        {
            DrawZoneHeader("Limsa Lominsa and La Noscea", "1-50", Color);
            
            DrawZoneSection("Limsa Lominsa", "1-15",
                "Maritime city-state and starting area for Marauders, Arcanists, and Rogues.",
                () => DrawTeleportButton("Limsa Lominsa Lower Decks", 8), Color);
            
            DrawZoneSection("Lower La Noscea", "10-20",
                "Coastal lowlands with fishing villages and ancient Allagan ruins.",
                () => {
                    DrawTeleportButton("Moraby Drydocks", 10);
                    ImGui.SameLine();
                    DrawTeleportButton("Aleport", 14);
                    ImGui.SameLine();
                    DrawTeleportButton("Summerford Farms", 52);
                }, Color);
            
            DrawZoneSection("Middle La Noscea", "15-25",
                "Central island region known for its wine production and trade routes.",
                () => DrawTeleportButton("Wineport", 12), Color);
            
            DrawZoneSection("Western La Noscea", "15-30",
                "Rocky western coastline with the bustling port town of Swiftperch.",
                () => DrawTeleportButton("Swiftperch", 13), Color);
            
            DrawZoneSection("Eastern La Noscea", "30-35",
                "Tourist destination featuring the resort town of Costa del Sol.",
                () => DrawTeleportButton("Costa del Sol", 11), Color);
            
            DrawZoneSection("Upper La Noscea", "30-35",
                "Mountainous highlands with Bronze Lake and kobold territories.",
                () => DrawTeleportButton("Camp Bronze Lake", 15), Color);
            
            DrawZoneSection("Outer La Noscea", "40-50",
                "Remote outer islands with imperial installations and dangerous wildlife.",
                () => {
                    DrawTeleportButton("Camp Overlook", 16);
                    ImGui.SameLine();
                    DrawTeleportButton("Ceruleum Processing Plant", 22);
                }, Color);
        }
    }
}
