using ImGuiNET;
using System.Numerics;

namespace ZoneLevelGuide.Modules
{
    public class NorvrandtModule : BaseZoneModule
    {
        public override string ZoneName => "Norvrandt";
        public override string LevelRange => "70-80";
        public override Vector4 Color => new Vector4(0.7f, 0.5f, 0.9f, 1.0f);

        public NorvrandtModule(ITeleporterIpc? teleporter) : base(teleporter) { }

        public override void DrawContent()
        {
            DrawZoneHeader("Norvrandt - Shadowbringers", "70-80", Color);
            
            DrawZoneSection("The Crystarium", "70-80",
                "Crystal city and beacon of hope in the world consumed by Light.",
                () => DrawTeleportButton("The Crystarium", 133), Color);
            
            DrawZoneSection("Eulmore", "70-80",
                "Decadent city of indulgence built atop Gatetown's suffering.",
                () => DrawTeleportButton("Eulmore", 134), Color);
            
            DrawZoneSection("Lakeland", "70-72",
                "Crystalline landscape surrounding the Crystarium with remnants of civilization.",
                () => {
                    DrawTeleportButton("Fort Jobb", 132);
                    ImGui.SameLine();
                    DrawTeleportButton("The Ostall Imperative", 136);
                }, Color);
            
            DrawZoneSection("Kholusia", "70-72",
                "Industrial region dominated by Eulmore with diverse settlements below.",
                () => {
                    DrawTeleportButton("Stilltide", 137);
                    ImGui.SameLine();
                    DrawTeleportButton("Wright", 138);
                    ImGui.SameLine();
                    DrawTeleportButton("Tomra", 139);
                }, Color);
            
            DrawZoneSection("Amh Araeng", "75-77",
                "Desert wasteland with the mysterious ruins of the Ronkan Empire.",
                () => {
                    DrawTeleportButton("Mord Souq", 140);
                    ImGui.SameLine();
                    DrawTeleportButton("Twine", 141);
                    ImGui.SameLine();
                    DrawTeleportButton("The Inn at Journey's Head", 161);
                }, Color);
            
            DrawZoneSection("Il Mheg", "73-75",
                "Realm of the fae folk with whimsical dangers and pixie kingdoms.",
                () => {
                    DrawTeleportButton("Lydha Lran", 144);
                    ImGui.SameLine();
                    DrawTeleportButton("Pla Enni", 145);
                    ImGui.SameLine();
                    DrawTeleportButton("Wolekdorf", 146);
                }, Color);
            
            DrawZoneSection("The Rak'tika Greatwood", "74-76",
                "Ancient forest hiding the secrets of the Viis and Ronkan civilization.",
                () => {
                    DrawTeleportButton("Slitherbough", 142);
                    ImGui.SameLine();
                    DrawTeleportButton("Fanow", 143);
                }, Color);
            
            DrawZoneSection("The Tempest", "78-80",
                "Ocean depths hiding the sunken city of Amaurot and ancient truths.",
                () => {
                    DrawTeleportButton("The Ondo Cups", 147);
                    ImGui.SameLine();
                    DrawTeleportButton("The Macareneses Angle", 148);
                }, Color);
        }
    }
}
