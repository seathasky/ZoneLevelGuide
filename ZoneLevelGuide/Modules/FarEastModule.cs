using Dalamud.Bindings.ImGui;
using System.Numerics;

namespace ZoneLevelGuide.Modules
{
    public class FarEastModule : BaseZoneModule
    {
        public override string ZoneName => "Far East";
        public override string LevelRange => "60-70";
        public override Vector4 Color => new Vector4(0.9f, 0.5f, 0.5f, 1.0f);

        public FarEastModule(ITeleporterIpc? teleporter) : base(teleporter) { }

        public override void DrawContent()
        {
            DrawZoneHeader("The Far East - Stormblood", "60-70", Color);
            
            DrawZoneSection("Kugane", "60-70",
                "Port city of the Far East and main hub for Stormblood adventures.",
                () => DrawTeleportButton("Kugane", 111), Color);
            
            DrawZoneSection("The Ruby Sea", "61-63",
                "Tropical waters around Hingashi with the underwater Kojin settlements.",
                () => {
                    DrawTeleportButton("Tamamizu", 105);
                    ImGui.SameLine();
                    DrawTeleportButton("Onokoro", 106);
                }, Color);
            
            DrawZoneSection("Yanxia", "64-65",
                "War-torn region under Garlean occupation with rice fields and ancient temples.",
                () => {
                    DrawTeleportButton("Namai", 107);
                    ImGui.SameLine();
                    DrawTeleportButton("The House of the Fierce", 108);
                }, Color);
            
            DrawZoneSection("The Azim Steppe", "65-67",
                "Vast grasslands home to the nomadic Au Ra tribes and their eternal conflicts.",
                () => {
                    DrawTeleportButton("Reunion", 109);
                    ImGui.SameLine();
                    DrawTeleportButton("The Dawn Throne", 110);
                    ImGui.SameLine();
                    DrawTeleportButton("Dhoro Iloh", 128);
                }, Color);
            
            DrawZoneSection("The Fringes", "60-62",
                "Borderlands between Gyr Abania and Garlemald with Ala Mhigan resistance.",
                () => {
                    DrawTeleportButton("Rhalgr's Reach", 104);
                    ImGui.SameLine();
                    DrawTeleportButton("Castrum Oriens", 98);
                }, Color);
            
            DrawZoneSection("The Peaks", "63-64",
                "Mountainous heartland of Gyr Abania with ancient Ala Mhigan heritage.",
                () => {
                    DrawTeleportButton("Ala Gannha", 100);
                    ImGui.SameLine();
                    DrawTeleportButton("Ala Ghiri", 101);
                }, Color);
            
            DrawZoneSection("The Lochs", "67-70",
                "Salt flats and the ruined capital of Ala Mhigo, site of the final liberation.",
                () => {
                    DrawTeleportButton("The Ala Mhigan Quarter", 103);
                    ImGui.SameLine();
                    DrawTeleportButton("Porta Praetoria", 102);
                    ImGui.SameLine();
                    DrawTeleportButton("The Peering Stones", 99);
                }, Color);
            
            DrawZoneSection("Other Areas", "60-70",
                "Reconstruction efforts in the Doman Enclave.",
                () => DrawTeleportButton("The Doman Enclave", 127), Color);
        }
    }
}
