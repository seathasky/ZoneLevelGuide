using Dalamud.Bindings.ImGui;
using System.Numerics;

namespace ZoneLevelGuide.Modules
{
    public class TuralModule : BaseZoneModule
    {
        public override string ZoneName => "Tural";
        public override string LevelRange => "90-100";
        public override Vector4 Color => new Vector4(0.5f, 0.9f, 0.9f, 1.0f);

        public TuralModule(ITeleporterIpc? teleporter) : base(teleporter) { }

        public override void DrawContent()
        {
            DrawZoneHeader("Tural - Dawntrail", "90-100", Color);
            
            DrawZoneSection("Tuliyollal", "90-91",
                "Golden city and capital of Tural, hub of the rite of succession.",
                () => DrawTeleportButton("Tuliyollal", 216), Color);
            
            DrawZoneSection("Solution Nine", "97-100",
                "Technological wonder city and gateway to the mysteries of the golden legacy.",
                () => DrawTeleportButton("Solution Nine", 217), Color);
            
            DrawZoneSection("Urqopacha", "90-92",
                "Highland region of the Pelupelu people with terraced fields and ancient traditions.",
                () => {
                    DrawTeleportButton("Wachunpelo", 200);
                    ImGui.SameLine();
                    DrawTeleportButton("Worlar's Echo", 201);
                }, Color);
            
            DrawZoneSection("Kozama'uka", "94-96",
                "Tropical rainforest homeland of the Hanuhanu with lush jungles and coastal villages.",
                () => {
                    DrawTeleportButton("Ok'hanu", 202);
                    ImGui.SameLine();
                    DrawTeleportButton("Many Fires", 203);
                    ImGui.SameLine();
                    DrawTeleportButton("Earthenshire", 204);
                    ImGui.SameLine();
                    DrawTeleportButton("Dock Poga", 238);
                }, Color);
            
            DrawZoneSection("Yak T'el", "92-94",
                "Vast plains of the Mamool Ja with nomadic tribes and ancient stone cities.",
                () => {
                    DrawTeleportButton("Iq Br'aax", 205);
                    ImGui.SameLine();
                    DrawTeleportButton("Mamook", 206);
                }, Color);
            
            DrawZoneSection("Shaaloani", "93-95",
                "Desert frontier territory with cowboy culture and railway construction.",
                () => {
                    DrawTeleportButton("Hhusatahwi", 207);
                    ImGui.SameLine();
                    DrawTeleportButton("Sheshenewezi Springs", 208);
                    ImGui.SameLine();
                    DrawTeleportButton("Mehwahhetsoan", 209);
                }, Color);
            
            DrawZoneSection("Heritage Found", "96-98",
                "Technological wonderland with advanced research facilities and mysteries.",
                () => {
                    DrawTeleportButton("Electrope Strike", 212);
                    ImGui.SameLine();
                    DrawTeleportButton("Yyasulani Station", 210);
                    ImGui.SameLine();
                    DrawTeleportButton("The Outskirts", 211);
                }, Color);
            
            DrawZoneSection("Living Memory", "99-100",
                "Digital paradise where memories and reality blur in eternal preservation.",
                () => {
                    DrawTeleportButton("Leynode Mnemo", 213);
                    ImGui.SameLine();
                    DrawTeleportButton("Leynode Pyro", 214);
                    ImGui.SameLine();
                    DrawTeleportButton("Leynode Aero", 215);
                }, Color);
        }
    }
}
