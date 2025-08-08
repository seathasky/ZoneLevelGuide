using System.Numerics;

namespace ZoneLevelGuide.Modules
{
    public class MorDhonaModule : BaseZoneModule
    {
        public override string ZoneName => "Mor Dhona";
        public override string LevelRange => "45-50";
        public override Vector4 Color => new Vector4(0.8f, 0.6f, 0.9f, 1.0f);

        public MorDhonaModule(ITeleporterIpc? teleporter) : base(teleporter) { }

        public override void DrawContent()
        {
            DrawZoneHeader("Mor Dhona", "45-50", Color);
            
            DrawZoneSection("Mor Dhona", "45-50",
                "Crystallized wasteland and gateway to Heavensward. Former site of the Battle of Silvertear Skies.",
                () => DrawTeleportButton("Revenant's Toll", 24), Color);
        }
    }
}
