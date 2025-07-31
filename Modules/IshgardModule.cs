using ImGuiNET;
using System.Numerics;

namespace ZoneLevelGuide.Modules
{
    public class IshgardModule : BaseZoneModule
    {
        public override string ZoneName => "Ishgard";
        public override string LevelRange => "50-60";
        public override Vector4 Color => new Vector4(1.0f, 0.6f, 0.8f, 1.0f);

        public IshgardModule(ITeleporterIpc? teleporter) : base(teleporter) { }

        public override void DrawContent()
        {
            DrawZoneHeader("Ishgard and Dravania - Heavensward", "50-60", Color);
            
            DrawZoneSection("Foundation & The Pillars", "50-60",
                "Holy See of Ishgard - theocratic city-state and Heavensward starting area.",
                () => DrawTeleportButton("Foundation", 70), Color);
            
            DrawZoneSection("The Sea of Clouds", "51-54",
                "Floating islands in the sky, home to the Vanu Vanu beast tribe.",
                () => {
                    DrawTeleportButton("Camp Cloudtop", 72);
                    ImGui.SameLine();
                    DrawTeleportButton("Ok' Zundu", 73);
                }, Color);
            
            DrawZoneSection("The Churning Mists", "54-56",
                "Ancient home of dragons with floating islands and Sharlayan ruins.",
                () => {
                    DrawTeleportButton("Moghome", 78);
                    ImGui.SameLine();
                    DrawTeleportButton("Zenith", 79);
                }, Color);
            
            DrawZoneSection("The Dravanian Forelands", "52-54",
                "Vast wilderness of Dravania with diverse ecosystems and settlements.",
                () => {
                    DrawTeleportButton("Tailfeather", 76);
                    ImGui.SameLine();
                    DrawTeleportButton("Anyx Trine", 77);
                }, Color);
            
            DrawZoneSection("The Dravanian Hinterlands", "56-60",
                "Site of abandoned Sharlayan colony with Alexander's mysterious presence.",
                () => DrawTeleportButton("Idyllshire", 75), Color);
            
            DrawZoneSection("Azys Lla", "58-60",
                "Ancient Allagan research facility floating in the sky - heart of the empire's secrets.",
                () => DrawTeleportButton("Helix", 74), Color);
        }
    }
}
