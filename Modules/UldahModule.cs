using ImGuiNET;
using System.Numerics;

namespace ZoneLevelGuide.Modules
{
    public class UldahModule : BaseZoneModule
    {
        public override string ZoneName => "Ul'dah";
        public override string LevelRange => "1-50";
        public override Vector4 Color => new Vector4(1.0f, 0.8f, 0.4f, 1.0f);

        public UldahModule(ITeleporterIpc? teleporter) : base(teleporter) { }

        public override void DrawContent()
        {
            DrawZoneHeader("Ul'dah and Thanalan", "1-50", Color);
            
            DrawZoneSection("Ul'dah - Steps of Nald/Thal", "1-15",
                "Desert jewel and starting area for Gladiators, Pugilists, and Thaumaturges.",
                () => DrawTeleportButton("Ul'dah - Steps of Nald", 9), Color);
            
            DrawZoneSection("Western Thanalan", "5-15",
                "Western desert region with the frontier settlement of Horizon.",
                () => {
                    DrawTeleportButton("Horizon", 17);
                    ImGui.SameLine();
                    DrawTeleportButton("Black Brush Station", 53);
                }, Color);
            
            DrawZoneSection("Eastern Thanalan", "20-25",
                "Eastern badlands featuring the oasis town of Camp Drybone.",
                () => DrawTeleportButton("Camp Drybone", 18), Color);
            
            DrawZoneSection("Southern Thanalan", "35-40",
                "Southern desert with Ala Mhigan refugees and ancient ruins.",
                () => {
                    DrawTeleportButton("Little Ala Mhigo", 19);
                    ImGui.SameLine();
                    DrawTeleportButton("Forgotten Springs", 20);
                }, Color);
            
            DrawZoneSection("Northern Thanalan", "45-50",
                "Harsh northern desert with imperial presence and ceruleum mines.",
                () => DrawTeleportButton("Camp Bluefog", 21), Color);
            
            DrawZoneSection("Gold Saucer", "15+",
                "Manderville's entertainment paradise with games, races, and prizes.",
                () => DrawTeleportButton("The Gold Saucer", 61), Color);
        }
    }
}
