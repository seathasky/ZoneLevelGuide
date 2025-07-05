using ImGuiNET;
using System;
using System.Numerics;

namespace ZoneLevelGuide.Modules
{
    public abstract class BaseZoneModule : IZoneModule
    {
        protected readonly ITeleporterIpc? teleporter;

        public abstract string ZoneName { get; }
        public abstract string LevelRange { get; }
        public abstract Vector4 Color { get; }

        protected BaseZoneModule(ITeleporterIpc? teleporter)
        {
            this.teleporter = teleporter;
        }

        public abstract void DrawContent();

        protected void DrawTeleportButton(string locationName, uint aetheryteId)
        {
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(10, 6));
            ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 3.0f);
            
            if (teleporter != null)
            {
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.35f, 0.38f, 0.42f, 0.8f));
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.42f, 0.45f, 0.50f, 0.9f));
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.48f, 0.52f, 0.58f, 1.0f));
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.95f, 0.95f, 0.95f, 1.0f));

                bool pressed = ImGui.Button($"🗺️ {locationName}###tp_{aetheryteId}");

                ImGui.PopStyleColor(4);

                if (pressed)
                {
                    try
                    {
                        teleporter.Teleport(aetheryteId);
                    }
                    catch (System.Exception)
                    {
                        // Silently fail if teleport is not available
                    }
                }
            }
            else
            {
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.35f, 0.35f, 0.35f, 0.6f));
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.75f, 0.75f, 0.75f, 0.8f));
                ImGui.Button($"📍 {locationName}###disabled_{aetheryteId}");
                ImGui.PopStyleColor(2);
            }
            
            ImGui.PopStyleVar(2);
        }

        protected void DrawZoneHeader(string zoneName, string levelRange, Vector4 color)
        {
            var headerColor = new Vector4(
                Math.Min(color.X + 0.3f, 1.0f),
                Math.Min(color.Y + 0.3f, 1.0f),
                Math.Min(color.Z + 0.3f, 1.0f),
                1.0f
            );
            
            ImGui.PushStyleColor(ImGuiCol.Text, headerColor);
            ImGui.SetWindowFontScale(1.2f);
            ImGui.Text(zoneName);
            ImGui.SetWindowFontScale(1.0f);
            ImGui.PopStyleColor();
            
            ImGui.SameLine();
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.85f, 0.85f, 0.85f, 1.0f));
            ImGui.Text($"({levelRange})");
            ImGui.PopStyleColor();
            
            ImGui.Spacing();
            ImGui.Separator();
            ImGui.Spacing();
        }

        protected void DrawZoneSection(string sectionName, string levelRange, string description, Action drawButtons, Vector4 color)
        {
            ImGui.Spacing();
            
            var sectionColor = new Vector4(
                Math.Min(color.X + 0.2f, 0.95f),
                Math.Min(color.Y + 0.2f, 0.95f),
                Math.Min(color.Z + 0.2f, 0.95f),
                1.0f
            );
            
            ImGui.PushStyleColor(ImGuiCol.Text, sectionColor);
            ImGui.SetWindowFontScale(1.05f);
            ImGui.Text($"🏛️ {sectionName}");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.PopStyleColor();
            
            ImGui.SameLine();
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.9f, 0.3f, 1.0f));
            ImGui.Text($"Level {levelRange}");
            ImGui.PopStyleColor();
            
            if (!string.IsNullOrEmpty(description))
            {
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.9f, 0.9f, 0.9f, 1.0f));
                ImGui.TextWrapped(description);
                ImGui.PopStyleColor();
                ImGui.Spacing();
            }
            
            ImGui.Indent(10.0f);
            drawButtons();
            ImGui.Unindent(10.0f);
            
            ImGui.Spacing();
        }
    }
}
