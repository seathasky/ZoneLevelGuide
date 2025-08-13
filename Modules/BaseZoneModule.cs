using Dalamud.Bindings.ImGui;
using System;
using System.Numerics;

namespace ZoneLevelGuide.Modules
{
    public abstract class BaseZoneModule : IZoneModule
    {
        protected readonly ITeleporterIpc? teleporter;
        
        public static FavoritesModule? FavoritesManager { get; set; }

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
            DrawTeleportButtonWithStar(locationName, aetheryteId, ZoneName, new Vector4(0.35f, 0.38f, 0.42f, 0.8f));
        }

        protected void DrawTeleportButtonWithStar(string locationName, uint aetheryteId, string zoneName, Vector4 buttonColor)
        {
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(10, 6));
            ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 3.0f);
            
            string teleportId = $"{zoneName}_{aetheryteId}_{locationName}";
            bool isFavorite = FavoritesManager?.IsFavorite(teleportId) ?? false;
            
            ImGui.PushStyleColor(ImGuiCol.Text, isFavorite ? 
                new Vector4(1.0f, 0.8f, 0.2f, 1.0f) : 
                new Vector4(0.5f, 0.5f, 0.5f, 1.0f));
            
            if (ImGui.Button($"★##star_{teleportId}"))
            {
                if (FavoritesManager != null)
                {
                    if (isFavorite)
                    {
                        FavoritesManager.RemoveFavorite(teleportId);
                    }
                    else
                    {
                        FavoritesManager.AddFavorite(teleportId, locationName, zoneName, locationName, aetheryteId, buttonColor);
                    }
                }
            }
            ImGui.PopStyleColor();
            
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip(isFavorite ? "Remove from favorites" : "Add to favorites");
            }
            
            ImGui.SameLine();
            
            if (teleporter != null)
            {
                ImGui.PushStyleColor(ImGuiCol.Button, buttonColor);
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(
                    Math.Min(buttonColor.X + 0.1f, 1.0f),
                    Math.Min(buttonColor.Y + 0.1f, 1.0f),
                    Math.Min(buttonColor.Z + 0.1f, 1.0f),
                    buttonColor.W
                ));
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(
                    Math.Min(buttonColor.X + 0.2f, 1.0f),
                    Math.Min(buttonColor.Y + 0.2f, 1.0f),
                    Math.Min(buttonColor.Z + 0.2f, 1.0f),
                    buttonColor.W
                ));
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.95f, 0.95f, 0.95f, 1.0f));

                bool pressed = ImGui.Button($"{locationName}###tp_{teleportId}");
                ImGui.PopStyleColor(4);

                if (pressed)
                {
                    try
                    {
                        teleporter.Teleport(aetheryteId);
                    }
                    catch (System.Exception)
                    {
                    }
                }
            }
            else
            {
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.35f, 0.35f, 0.35f, 0.6f));
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.75f, 0.75f, 0.75f, 0.8f));
                ImGui.Button($"{locationName}###disabled_{teleportId}");
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
            ImGui.Text($"{sectionName}");
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
