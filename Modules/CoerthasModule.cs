using System.Numerics;

namespace ZoneLevelGuide.Modules
{
    public class CoerthasModule : BaseZoneModule
    {
        public override string ZoneName => "Coerthas";
        public override string LevelRange => "35-53";
        public override Vector4 Color => new Vector4(0.9f, 0.9f, 1.0f, 1.0f);

        public CoerthasModule(ITeleporterIpc? teleporter) : base(teleporter) { }

        public override void DrawContent()
        {
            DrawZoneHeader("Coerthas - Pre-Calamity Regions", "35-53", Color);
            
            DrawZoneSection("Coerthas Central Highlands", "35-45",
                "Perpetually frozen highlands south of Ishgard, changed by the Calamity.",
                () => DrawTeleportButton("Camp Dragonhead", 23), Color);
            
            DrawZoneSection("Coerthas Western Highlands", "50-53",
                "Frozen frontier region with Ishgardian outposts and dragon threats.",
                () => DrawTeleportButton("Falcon's Nest", 71), Color);
        }
    }
}
