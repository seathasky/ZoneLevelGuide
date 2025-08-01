using ImGuiNET;
using System.Numerics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ZoneLevelGuide.Modules
{
    public class FavoritesModule : BaseZoneModule
    {
        // Storage for favorite teleports
        private Dictionary<string, FavoriteTeleport> favoriteTeleports = new();
        
        public override string ZoneName => "Favorites";
        public override string LevelRange => "Quick Access";
        public override Vector4 Color => new Vector4(1.0f, 0.8f, 0.2f, 1.0f); // Golden yellow for favorites

        public FavoritesModule(ITeleporterIpc? teleporter) : base(teleporter) 
        {
            LoadFavorites();
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

        private void SaveFavorites()
        {
            try
            {
                string configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ZoneLevelGuide");
                Directory.CreateDirectory(configPath);
                string configFile = Path.Combine(configPath, "favorites.txt");
                
                using (var writer = new StreamWriter(configFile, false))
                {
                    foreach (var kvp in favoriteTeleports)
                    {
                        var fav = kvp.Value;
                        writer.WriteLine($"{kvp.Key}|{fav.Name}|{fav.Zone}|{fav.Command}|{fav.AetheryteId}|{fav.ButtonColor.X},{fav.ButtonColor.Y},{fav.ButtonColor.Z},{fav.ButtonColor.W}|{fav.AddedDate:yyyy-MM-dd HH:mm:ss}|{fav.Order}");
                    }
                }
            }
            catch { }
        }

        private void LoadFavorites()
        {
            favoriteTeleports.Clear();
            try
            {
                string configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ZoneLevelGuide");
                string configFile = Path.Combine(configPath, "favorites.txt");
                
                if (File.Exists(configFile))
                {
                    foreach (var line in File.ReadAllLines(configFile))
                    {
                        var parts = line.Split('|');
                        if (parts.Length >= 7)
                        {
                            var colorParts = parts[5].Split(',');
                            if (colorParts.Length == 4 && 
                                float.TryParse(colorParts[0], out float r) &&
                                float.TryParse(colorParts[1], out float g) &&
                                float.TryParse(colorParts[2], out float b) &&
                                float.TryParse(colorParts[3], out float a) &&
                                uint.TryParse(parts[4], out uint aetheryteId) &&
                                DateTime.TryParse(parts[6], out DateTime addedDate))
                            {
                                int order = 0;
                                if (parts.Length >= 8 && int.TryParse(parts[7], out int parsedOrder))
                                {
                                    order = parsedOrder;
                                }
                                
                                favoriteTeleports[parts[0]] = new FavoriteTeleport
                                {
                                    Name = parts[1],
                                    Zone = parts[2],
                                    Command = parts[3],
                                    AetheryteId = aetheryteId,
                                    ButtonColor = new Vector4(r, g, b, a),
                                    AddedDate = addedDate,
                                    Order = order
                                };
                            }
                        }
                    }
                }
            }
            catch { }
            
            // Ensure all favorites have proper order values
            ReassignOrderValues();
        }
        
        private void ReassignOrderValues()
        {
            var favorites = favoriteTeleports.ToList();
            
            // Sort by existing order, then by added date for items without order
            favorites.Sort((a, b) => {
                if (a.Value.Order != b.Value.Order)
                    return a.Value.Order.CompareTo(b.Value.Order);
                return b.Value.AddedDate.CompareTo(a.Value.AddedDate);
            });
            
            // Reassign sequential order values
            for (int i = 0; i < favorites.Count; i++)
            {
                var key = favorites[i].Key;
                var fav = favorites[i].Value;
                fav.Order = i;
                favoriteTeleports[key] = fav;
            }
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
        private const double HIGHLIGHT_DURATION_MS = 500; // Half second highlight
        
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
            
            if (currentIndex <= 0) // Already at top or not found
                return;
                
            // Swap order values with the item above
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
            
            if (currentIndex < 0 || currentIndex >= sortedFavorites.Count - 1) // Already at bottom or not found
                return;
                
            // Swap order values with the item below
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
            // Check if this item should be highlighted and calculate fade
            double timeElapsed = (DateTime.Now - highlightStartTime).TotalMilliseconds;
            bool shouldHighlight = highlightedItemId == teleportId && timeElapsed < HIGHLIGHT_DURATION_MS;
            
            // Draw highlight background for just this row with fade effect
            if (shouldHighlight)
            {
                // Calculate fade: start at full opacity, fade to 0 over the duration
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
            
            // Up arrow button (disabled if first item)
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
            
            // Down arrow button (disabled if last item)
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
            
            // Add spacing between arrows and star
            ImGui.SameLine();
            ImGui.Dummy(new Vector2(8, 0)); // 8 pixels of spacing
            ImGui.SameLine();
            
            // Star button to remove
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
            
            // Teleport button
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
                // Use the HousingModule's shared estate teleportation logic
                HousingModule.ExecuteSharedEstateTeleportForFavorites(teleporter, estateIndex);
            }
            catch
            {
            }
        }
    }
}
