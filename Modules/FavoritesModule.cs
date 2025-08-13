using Dalamud.Bindings.ImGui;
using System.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ZoneLevelGuide.Modules
{
    public class FavoritesModule : BaseZoneModule
    {
        private Dictionary<string, FavoriteTeleport> favoriteTeleports = new();
        private readonly Configuration? configuration;
        private readonly ITeleporterIpc? teleporterIpc;
        
        public override string ZoneName => "Favorites";
        public override string LevelRange => "Quick Access";
        public override Vector4 Color => new Vector4(1.0f, 0.8f, 0.2f, 1.0f);

        public FavoritesModule(ITeleporterIpc? teleporter, Configuration? configuration = null) : base(teleporter) 
        {
            this.configuration = configuration;
            this.teleporterIpc = teleporter;
            if (configuration != null)
            {
                LoadFavoritesFromConfiguration();
            }
        }

        public struct FavoriteTeleport
        {
            public string Name { get; set; }
            public string Zone { get; set; }
            public string Command { get; set; }
            public uint AetheryteId { get; set; }
            public Vector4 ButtonColor { get; set; }
            public DateTime AddedDate { get; set; }
            public int Order { get; set; }
        }

        private void LoadFavoritesFromConfiguration()
        {
            favoriteTeleports.Clear();
            if (configuration == null || teleporterIpc == null) return;
            
            try
            {
                string characterKey = teleporterIpc.GetCurrentCharacterKey();
                var characterProfile = configuration.GetCharacterProfile(characterKey);
                
                foreach (var kvp in characterProfile.FavoriteTeleports)
                {
                    var data = kvp.Value;
                    favoriteTeleports[kvp.Key] = new FavoriteTeleport
                    {
                        Name = data.Name,
                        Zone = data.Zone,
                        Command = data.Command,
                        AetheryteId = data.AetheryteId,
                        ButtonColor = data.ButtonColor,
                        AddedDate = data.AddedDate,
                        Order = data.Order
                    };
                }
            }
            catch (InvalidOperationException)
            {
                // Character information not available yet (not on main thread or no character logged in)
                // This is fine during initialization, favorites will load when character is available
            }
        }        private void SaveFavorites()
        {
            if (configuration == null || teleporterIpc == null) return;
            
            try
            {
                string characterKey = teleporterIpc.GetCurrentCharacterKey();
                var characterProfile = configuration.GetCharacterProfile(characterKey);
                
                characterProfile.FavoriteTeleports.Clear();
                foreach (var kvp in favoriteTeleports)
                {
                    var fav = kvp.Value;
                    characterProfile.FavoriteTeleports[kvp.Key] = new FavoriteTeleportData
                    {
                        Name = fav.Name,
                        Zone = fav.Zone,
                        Command = fav.Command,
                        AetheryteId = fav.AetheryteId,
                        ButtonColor = fav.ButtonColor,
                        AddedDate = fav.AddedDate,
                        Order = fav.Order
                    };
                }
                configuration.Save();
                
                ReassignOrderValues();
            }
            catch (InvalidOperationException)
            {
                // Character information not available yet, skip saving for now
                // This should rarely happen since saves are usually triggered by user actions
            }
        }
        
        private void ReassignOrderValues()
        {
            var favorites = favoriteTeleports.ToList();
            
            favorites.Sort((a, b) => {
                if (a.Value.Order != b.Value.Order)
                    return a.Value.Order.CompareTo(b.Value.Order);
                return b.Value.AddedDate.CompareTo(a.Value.AddedDate);
            });
            
            for (int i = 0; i < favorites.Count; i++)
            {
                var key = favorites[i].Key;
                var fav = favorites[i].Value;
                fav.Order = i;
                favoriteTeleports[key] = fav;
            }
        }

        public void RefreshForCharacter()
        {
            LoadFavoritesFromConfiguration();
        }

        public bool IsFavorite(string teleportId)
        {
            return favoriteTeleports.ContainsKey(teleportId);
        }

        public void AddFavorite(string teleportId, string name, string zone, string command, uint aetheryteId, Vector4 buttonColor)
        {
            int nextOrder = favoriteTeleports.Count > 0 ? favoriteTeleports.Values.Max(f => f.Order) + 1 : 0;
            
            favoriteTeleports[teleportId] = new FavoriteTeleport
            {
                Name = name,
                Zone = zone,
                Command = command,
                AetheryteId = aetheryteId,
                ButtonColor = buttonColor,
                AddedDate = DateTime.Now,
                Order = nextOrder
            };
            SaveFavorites();
        }

        public void RemoveFavorite(string teleportId)
        {
            if (favoriteTeleports.ContainsKey(teleportId))
            {
                favoriteTeleports.Remove(teleportId);
                SaveFavorites();
            }
        }

        public override void DrawContent()
        {
            DrawZoneHeader("Favorite Teleports", "Quick Access", Color);
            
            if (favoriteTeleports.Count == 0)
            {
                DrawEmptyFavoritesMessage();
            }
            else
            {
                DrawZoneSection("Your Favorites", "Quick Access",
                    "Your most-used teleport destinations",
                    () => {
                        DrawFavoritesList();
                    }, Color);
            }
            
            ImGui.Spacing();
            ImGui.Separator();
            ImGui.Spacing();
        }

        private void DrawEmptyFavoritesMessage()
        {
            ImGui.Spacing();
            ImGui.Spacing();
            
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.7f, 0.7f, 0.7f, 1.0f));
            ImGui.TextWrapped("No favorite teleports yet!");
            ImGui.Spacing();
            ImGui.TextWrapped("To add favorites:");
            ImGui.Spacing();
            
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.8f, 0.2f, 1.0f));
            ImGui.TextWrapped("★ Click the star next to any teleport button in other tabs");
            ImGui.PopStyleColor();
            
            ImGui.Spacing();
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.6f, 0.6f, 0.6f, 1.0f));
            ImGui.TextWrapped("Stars will turn yellow when added to favorites, and your favorite teleports will appear here for quick access.");
            ImGui.PopStyleColor(2);
        }

        private static string? highlightedItemId = null;
        private static DateTime highlightStartTime = DateTime.MinValue;
    private const double HIGHLIGHT_DURATION_MS = 500;
        
        private void DrawFavoritesList()
        {
            var sortedFavorites = favoriteTeleports.OrderBy(kvp => kvp.Value.Order).ThenByDescending(kvp => kvp.Value.AddedDate).ToList();
            
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(12, 8));
            ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 4.0f);
            
            for (int i = 0; i < sortedFavorites.Count; i++)
            {
                var kvp = sortedFavorites[i];
                var teleportId = kvp.Key;
                var favorite = kvp.Value;
                
                ImGui.PushID($"favorite_{teleportId}_{i}");
                
                DrawFavoriteItem(teleportId, favorite, i, sortedFavorites.Count);
                
                ImGui.PopID();
            }
            
            ImGui.PopStyleVar(2);
            
            ImGui.Spacing();
            ImGui.Separator();
            ImGui.Spacing();
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.6f, 0.6f, 0.6f, 1.0f));
            ImGui.Text($"Total favorites: {favoriteTeleports.Count}");
            ImGui.PopStyleColor();
        }
        
        private void MoveItemUp(string teleportId)
        {
            if (!favoriteTeleports.ContainsKey(teleportId))
                return;
                
            var sortedFavorites = favoriteTeleports.OrderBy(kvp => kvp.Value.Order).ThenByDescending(kvp => kvp.Value.AddedDate).ToList();
            int currentIndex = sortedFavorites.FindIndex(kvp => kvp.Key == teleportId);
            
            if (currentIndex <= 0)
                return;
                
            
            var currentItem = sortedFavorites[currentIndex];
            var aboveItem = sortedFavorites[currentIndex - 1];
            
            var currentFav = currentItem.Value;
            var aboveFav = aboveItem.Value;
            
            int tempOrder = currentFav.Order;
            currentFav.Order = aboveFav.Order;
            aboveFav.Order = tempOrder;
            
            favoriteTeleports[currentItem.Key] = currentFav;
            favoriteTeleports[aboveItem.Key] = aboveFav;
            
            SaveFavorites();
        }
        
        private void MoveItemDown(string teleportId)
        {
            if (!favoriteTeleports.ContainsKey(teleportId))
                return;
                
            var sortedFavorites = favoriteTeleports.OrderBy(kvp => kvp.Value.Order).ThenByDescending(kvp => kvp.Value.AddedDate).ToList();
            int currentIndex = sortedFavorites.FindIndex(kvp => kvp.Key == teleportId);
            
            if (currentIndex < 0 || currentIndex >= sortedFavorites.Count - 1)
                return;
                
            
            var currentItem = sortedFavorites[currentIndex];
            var belowItem = sortedFavorites[currentIndex + 1];
            
            var currentFav = currentItem.Value;
            var belowFav = belowItem.Value;
            
            int tempOrder = currentFav.Order;
            currentFav.Order = belowFav.Order;
            belowFav.Order = tempOrder;
            
            favoriteTeleports[currentItem.Key] = currentFav;
            favoriteTeleports[belowItem.Key] = belowFav;
            
            SaveFavorites();
        }
        
        private void DrawFavoriteItem(string teleportId, FavoriteTeleport favorite, int index, int totalCount)
        {
            
            double timeElapsed = (DateTime.Now - highlightStartTime).TotalMilliseconds;
            bool shouldHighlight = highlightedItemId == teleportId && timeElapsed < HIGHLIGHT_DURATION_MS;
            
            
            if (shouldHighlight)
            {
                
                float fadeProgress = (float)(timeElapsed / HIGHLIGHT_DURATION_MS);
                float opacity = Math.Max(0.0f, 0.4f * (1.0f - fadeProgress)); // Start at 0.4, fade to 0
                
                Vector2 cursorPos = ImGui.GetCursorScreenPos();
                Vector2 itemSize = new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetTextLineHeightWithSpacing() + 16);
                
                ImGui.GetWindowDrawList().AddRectFilled(
                    cursorPos,
                    new Vector2(cursorPos.X + itemSize.X, cursorPos.Y + itemSize.Y),
                    ImGui.ColorConvertFloat4ToU32(new Vector4(1.0f, 0.8f, 0.2f, opacity)),
                    4.0f
                );
            }
            
            
            if (index == 0)
            {
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.2f, 0.2f, 0.2f, 0.3f));
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.4f, 0.4f, 0.4f, 0.6f));
                ImGui.Button("▲");
                ImGui.PopStyleColor(2);
            }
            else
            {
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.4f, 0.4f, 0.4f, 0.6f));
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.5f, 0.5f, 0.5f, 0.8f));
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.9f, 0.9f, 0.9f, 1.0f));
                if (ImGui.Button("▲"))
                {
                    highlightedItemId = teleportId;
                    highlightStartTime = DateTime.Now;
                    MoveItemUp(teleportId);
                }
                ImGui.PopStyleColor(3);
                
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("Move up");
                }
            }
            
            ImGui.SameLine();
            
            
            if (index == totalCount - 1)
            {
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.2f, 0.2f, 0.2f, 0.3f));
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.4f, 0.4f, 0.4f, 0.6f));
                ImGui.Button("▼");
                ImGui.PopStyleColor(2);
            }
            else
            {
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.4f, 0.4f, 0.4f, 0.6f));
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.5f, 0.5f, 0.5f, 0.8f));
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.9f, 0.9f, 0.9f, 1.0f));
                if (ImGui.Button("▼"))
                {
                    highlightedItemId = teleportId;
                    highlightStartTime = DateTime.Now;
                    MoveItemDown(teleportId);
                }
                ImGui.PopStyleColor(3);
                
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("Move down");
                }
            }
            
            
            ImGui.SameLine();
            ImGui.Dummy(new Vector2(8, 0));
            ImGui.SameLine();
            
            
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.8f, 0.2f, 1.0f));
            if (ImGui.Button("★"))
            {
                RemoveFavorite(teleportId);
                ImGui.PopStyleColor();
                return;
            }
            ImGui.PopStyleColor();
            
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Remove from favorites");
            }
            
            ImGui.SameLine();
            
            
            if (teleporter != null)
            {
                ImGui.PushStyleColor(ImGuiCol.Button, favorite.ButtonColor);
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(
                    Math.Min(favorite.ButtonColor.X + 0.1f, 1.0f),
                    Math.Min(favorite.ButtonColor.Y + 0.1f, 1.0f),
                    Math.Min(favorite.ButtonColor.Z + 0.1f, 1.0f),
                    favorite.ButtonColor.W
                ));
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(
                    Math.Min(favorite.ButtonColor.X + 0.2f, 1.0f),
                    Math.Min(favorite.ButtonColor.Y + 0.2f, 1.0f),
                    Math.Min(favorite.ButtonColor.Z + 0.2f, 1.0f),
                    favorite.ButtonColor.W
                ));
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.95f, 0.95f, 0.95f, 1.0f));

                bool pressed = ImGui.Button(favorite.Name);
                ImGui.PopStyleColor(4);

                if (pressed)
                {
                    ExecuteTeleportCommand(favorite.Command, favorite.AetheryteId);
                }
            }
            else
            {
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.3f, 0.3f, 0.3f, 0.6f));
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.6f, 0.6f, 0.6f, 0.8f));
                ImGui.Button(favorite.Name);
                ImGui.PopStyleColor(2);
            }
            
            ImGui.SameLine();
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.7f, 0.7f, 0.7f, 1.0f));
            ImGui.Text($"({favorite.Zone})");
            ImGui.PopStyleColor();
            
            ImGui.Spacing();
        }
        
        private void ExecuteTeleportCommand(string command, uint aetheryteId)
        {
            try
            {
                var teleporterService = teleporter as TeleporterService;
                if (teleporterService != null)
                {
                    if (command.StartsWith("SharedEstate_") && aetheryteId >= 10000)
                    {
                        int estateIndex = (int)(aetheryteId - 10000);
                        ExecuteSharedEstateTeleport(estateIndex);
                    }
                    else if (aetheryteId > 0 && aetheryteId < 10000)
                    {
                        teleporterService.Teleport(aetheryteId);
                    }
                    else if (command == "Estate Hall" || command == "Estate Hall (Free Company)" || command == "Shared Estate")
                    {
                        teleporterService.ExecuteEstateCommand(command);
                    }
                    else
                    {
                        teleporterService.ExecuteEstateCommand(command);
                    }
                }
            }
            catch (System.Exception)
            {
                ImGui.OpenPopup($"Teleport Info###{command}");
            }
        }

        private void ExecuteSharedEstateTeleport(int estateIndex)
        {
            try
            {
                
                HousingModule.ExecuteSharedEstateTeleportForFavorites(teleporter, estateIndex);
            }
            catch
            {
            }
        }
    }
}
