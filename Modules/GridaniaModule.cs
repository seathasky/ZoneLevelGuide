using ImGuiNET;
using System.Numerics;

namespace ZoneLevelGuide.Modules
{
    public class GridaniaModule : BaseZoneModule
    {
        public override string ZoneName => "Gridania";
        public override string LevelRange => "1-30";
        public override Vector4 Color => new Vector4(0.4f, 0.8f, 0.4f, 1.0f);

        public GridaniaModule(ITeleporterIpc? teleporter) : base(teleporter) { }

        public override void DrawContent()
        {
            DrawZoneHeader("Gridania and The Black Shroud", "1-30", Color);
            
            DrawZoneSection("New Gridania & Old Gridania", "1-15", 
                "Starting area for Conjurers, Archers, and Lancers. City of nature and the elementals.",
                () => DrawTeleportButton("New Gridania", 2), Color);
            
            DrawZoneSection("Central Shroud", "5-15",
                "Peaceful woodland area with the Bentbranch Meadows settlement.",
                () => DrawTeleportButton("Bentbranch Meadows", 3), Color);
            
            DrawZoneSection("East Shroud", "15-25",
                "Dense forest region home to sylphs and various woodland creatures.",
                () => DrawTeleportButton("The Hawthorne Hut", 4), Color);
            
            DrawZoneSection("South Shroud", "20-30",
                "Southern forest area featuring the mining town of Quarrymill.",
                () => DrawTeleportButton("Quarrymill", 5), Color);
            
            DrawZoneSection("North Shroud", "15-30",
                "Northern reaches of the Black Shroud with diverse settlements.",
                () => {
                    DrawTeleportButton("Camp Tranquil", 6);
                    ImGui.SameLine();
                    DrawTeleportButton("Fallgourd Float", 7);
                }, Color);
        }
    }
}
