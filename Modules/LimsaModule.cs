using Dalamud.Bindings.ImGui;
using System.Numerics;

namespace ZoneLevelGuide.Modules
{
    public class LimsaModule : BaseZoneModule
    {
        public override string ZoneName => "La Noscea";
        public override string LevelRange => "1-50";
        public override Vector4 Color => new Vector4(0.4f, 0.7f, 1.0f, 1.0f);

        public LimsaModule(ITeleporterIpc? teleporter) : base(teleporter) { }

        public override void DrawContent()
        {
            DrawZoneHeader("Limsa Lominsa and La Noscea", "1-50", Color);
            
            DrawZoneSection("Limsa Lominsa", "1-15",
                "Maritime city-state and starting area for Marauders, Arcanists, and Rogues.",
                () => DrawTeleportButtonWithStar("Limsa Lominsa Lower Decks", 8, ZoneName, new Vector4(0.35f, 0.38f, 0.42f, 0.8f)), Color);
            
            DrawZoneSection("Lower La Noscea", "10-20",
                "Coastal lowlands with fishing villages and ancient Allagan ruins.",
                () => {
                    DrawTeleportButtonWithStar("Moraby Drydocks", 10, ZoneName, new Vector4(0.35f, 0.38f, 0.42f, 0.8f));
                    ImGui.SameLine();
                    DrawTeleportButtonWithStar("Aleport", 14, ZoneName, new Vector4(0.35f, 0.38f, 0.42f, 0.8f));
                    ImGui.SameLine();
                    DrawTeleportButtonWithStar("Summerford Farms", 52, ZoneName, new Vector4(0.35f, 0.38f, 0.42f, 0.8f));
                }, Color);
            
            DrawZoneSection("Middle La Noscea", "15-25",
                "Central island region known for its wine production and trade routes.",
                () => DrawTeleportButtonWithStar("Wineport", 12, ZoneName, new Vector4(0.35f, 0.38f, 0.42f, 0.8f)), Color);
            
            DrawZoneSection("Western La Noscea", "15-30",
                "Rocky western coastline with the bustling port town of Swiftperch.",
                () => DrawTeleportButtonWithStar("Swiftperch", 13, ZoneName, new Vector4(0.35f, 0.38f, 0.42f, 0.8f)), Color);
            
            DrawZoneSection("Eastern La Noscea", "30-35",
                "Tourist destination featuring the resort town of Costa del Sol.",
                () => DrawTeleportButtonWithStar("Costa del Sol", 11, ZoneName, new Vector4(0.35f, 0.38f, 0.42f, 0.8f)), Color);
            
            DrawZoneSection("Upper La Noscea", "30-35",
                "Mountainous highlands with Bronze Lake and kobold territories.",
                () => DrawTeleportButtonWithStar("Camp Bronze Lake", 15, ZoneName, new Vector4(0.35f, 0.38f, 0.42f, 0.8f)), Color);
            
            DrawZoneSection("Outer La Noscea", "40-50",
                "Remote outer islands with imperial installations and dangerous wildlife.",
                () => {
                    DrawTeleportButtonWithStar("Camp Overlook", 16, ZoneName, new Vector4(0.35f, 0.38f, 0.42f, 0.8f));
                    ImGui.SameLine();
                    DrawTeleportButtonWithStar("Ceruleum Processing Plant", 22, ZoneName, new Vector4(0.35f, 0.38f, 0.42f, 0.8f));
                }, Color);
        }
    }
}
