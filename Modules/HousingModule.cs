using ImGuiNET;
using System.Numerics;
using System;
using System.Collections.Generic;
using System.IO;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace ZoneLevelGuide.Modules
{
    public class HousingModule : BaseZoneModule
    {
        private const int MAX_SHARED_ESTATES = 2;
        private const int MAX_TELEPORT_ENTRIES = 50;
        private const int MAX_AETHERYTE_ID = 10000;
        
        private static readonly Dictionary<string, uint> HousingDistricts = new()
        {
            { "The Mist", 8 },
            { "Lavender Beds", 2 },
            { "The Goblet", 9 },
            { "Shirogane", 111 },
            { "Empyreum", 70 }
        };
        
        public override string ZoneName => "Housing";
        public override string LevelRange => "Any Level";
        public override Vector4 Color => new Vector4(0.8f, 0.6f, 0.4f, 1.0f);

        private Dictionary<int, string> sharedEstateNames = new();

        public HousingModule(ITeleporterIpc? teleporter) : base(teleporter) 
        {
            LoadSharedEstateNames();
        }
        
        private void SaveSharedEstateNames()
        {
            try
            {
                string configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ZoneLevelGuide");
                Directory.CreateDirectory(configPath);
                string configFile = Path.Combine(configPath, "shared_estate_names.txt");
                using (var writer = new StreamWriter(configFile, false))
                {
                    foreach (var kvp in sharedEstateNames)
                    {
                        writer.WriteLine($"{kvp.Key}:{kvp.Value.Replace("\n", " ").Replace("\r", " ")}");
                    }
                }
            }
            catch { }
        }

        private void LoadSharedEstateNames()
        {
            sharedEstateNames.Clear();
            try
            {
                string configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ZoneLevelGuide");
                string configFile = Path.Combine(configPath, "shared_estate_names.txt");
                if (File.Exists(configFile))
                {
                    foreach (var line in File.ReadAllLines(configFile))
                    {
                        var parts = line.Split(new[] { ':' }, 2);
                        if (parts.Length == 2 && int.TryParse(parts[0], out int idx))
                        {
                            sharedEstateNames[idx] = parts[1];
                        }
                    }
                }
            }
            catch { }
        }

        public override void DrawContent()
        {
            DrawZoneHeader("Housing & Residences", "Any Level", Color);
            
            DrawZoneSection("Estate Teleportation", "Any Level",
                "Direct teleport to your housing (if you own any)",
                () => {
                    DrawEstateButtons();
                    ImGui.Spacing();
                }, Color);
            
            DrawZoneSection("Housing Districts", "Any Level",
                "Teleport to housing district entrances",
                () => {
                    DrawHousingDistrictButtons();
                    ImGui.Spacing();
                }, Color);
            
            DrawHousingInformation();
            DrawTeleportInfoPopup();
        }

        private void DrawEstateButtons()
        {
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(10, 6));
            ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 3.0f);
            
            string privateEstateKey = "Housing_PrivateEstate_EstateHall";
            bool isPrivateFavorited = FavoritesManager?.IsFavorite(privateEstateKey) ?? false;
            
            ImGui.PushStyleColor(ImGuiCol.Text, isPrivateFavorited ? 
                new Vector4(1.0f, 0.8f, 0.2f, 1.0f) : 
                new Vector4(0.5f, 0.5f, 0.5f, 1.0f));
            
            if (ImGui.Button($"★##star_private_estate"))
            {
                if (FavoritesManager != null)
                {
                    if (isPrivateFavorited)
                    {
                        FavoritesManager.RemoveFavorite(privateEstateKey);
                    }
                    else
                    {
                        FavoritesManager.AddFavorite(privateEstateKey, "Private Estate Hall", ZoneName, "Estate Hall", 0, new Vector4(0.639f, 0.745f, 0.549f, 0.8f));
                    }
                }
            }
            ImGui.PopStyleColor();
            
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip(isPrivateFavorited ? "Remove from favorites" : "Add to favorites");
            }
            
            ImGui.SameLine();
            
            if (teleporter != null)
            {
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.639f, 0.745f, 0.549f, 0.8f));
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.739f, 0.845f, 0.649f, 0.9f));
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.839f, 0.945f, 0.749f, 1.0f));
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.95f, 0.95f, 0.95f, 1.0f));

                bool pressedPrivate = ImGui.Button("Private Estate Hall###private_estate");
                ImGui.PopStyleColor(4);

                if (pressedPrivate)
                {
                    ExecuteEstateCommand("Estate Hall");
                }
            }
            else
            {
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.298f, 0.337f, 0.416f, 0.8f));
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.75f, 0.75f, 0.75f, 0.8f));
                ImGui.Button("Private Estate Hall###disabled_private");
                ImGui.PopStyleColor(2);
            }

            ImGui.SameLine();

            string fcEstateKey = "Housing_FCEstate_EstateHallFC";
            bool isFCFavorited = FavoritesManager?.IsFavorite(fcEstateKey) ?? false;
            
            ImGui.PushStyleColor(ImGuiCol.Text, isFCFavorited ? 
                new Vector4(1.0f, 0.8f, 0.2f, 1.0f) : 
                new Vector4(0.5f, 0.5f, 0.5f, 1.0f));
            
            if (ImGui.Button($"★##star_fc_estate"))
            {
                if (FavoritesManager != null)
                {
                    if (isFCFavorited)
                    {
                        FavoritesManager.RemoveFavorite(fcEstateKey);
                    }
                    else
                    {
                        FavoritesManager.AddFavorite(fcEstateKey, "FC Estate Hall", ZoneName, "Estate Hall (Free Company)", 0, new Vector4(0.369f, 0.506f, 0.675f, 0.8f));
                    }
                }
            }
            ImGui.PopStyleColor();
            
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip(isFCFavorited ? "Remove from favorites" : "Add to favorites");
            }
            
            ImGui.SameLine();

            if (teleporter != null)
            {
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.369f, 0.506f, 0.675f, 0.8f));
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.506f, 0.631f, 0.757f, 0.9f));
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.533f, 0.753f, 0.816f, 1.0f));
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.95f, 0.95f, 0.95f, 1.0f));

                bool pressedFC = ImGui.Button("FC Estate Hall###fc_estate");
                ImGui.PopStyleColor(4);

                if (pressedFC)
                {
                    ExecuteEstateCommand("Estate Hall (Free Company)");
                }
            }
            else
            {
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.298f, 0.337f, 0.416f, 0.8f));
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.75f, 0.75f, 0.75f, 0.8f));
                ImGui.Button("FC Estate Hall###disabled_fc");
                ImGui.PopStyleColor(2);
            }

            ImGui.NewLine();
            DrawSharedEstateSection();

            ImGui.PopStyleVar(2);
        }

        private void DrawHousingDistrictButtons()
        {
            DrawTeleportButtonWithStar("The Mist", 8, ZoneName, new Vector4(0.35f, 0.38f, 0.42f, 0.8f));
            ImGui.SameLine();
            DrawTeleportButtonWithStar("Lavender Beds", 2, ZoneName, new Vector4(0.35f, 0.38f, 0.42f, 0.8f));
            ImGui.SameLine();
            DrawTeleportButtonWithStar("The Goblet", 9, ZoneName, new Vector4(0.35f, 0.38f, 0.42f, 0.8f));
            
            DrawTeleportButtonWithStar("Shirogane", 111, ZoneName, new Vector4(0.35f, 0.38f, 0.42f, 0.8f));
            ImGui.SameLine();
            DrawTeleportButtonWithStar("Empyreum", 70, ZoneName, new Vector4(0.35f, 0.38f, 0.42f, 0.8f));
        }

        private void DrawSharedEstateSection()
        {
            ImGui.Separator();
            ImGui.Spacing();
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.922f, 0.796f, 0.545f, 1.0f));
            ImGui.Text("Shared Estates");
            ImGui.PopStyleColor();

            try
            {
                var (hasEstate0, hasEstate1) = GetSharedEstateStatus();
                
                if (hasEstate0 || hasEstate1)
                {
                    DrawAvailableSharedEstates(hasEstate0, hasEstate1);
                }
                else
                {
                    // Fallback: single shared estate button
                    DrawSharedEstateButton("Shared Estate", "Shared Estate", -1);
                }
            }
            catch
            {
                // Ultimate fallback: disabled button
                DrawDisabledSharedEstateButton();
            }
        }

        private (bool hasEstate0, bool hasEstate1) GetSharedEstateStatus()
        {
            try
            {
                unsafe
                {
                    var housingManager = HousingManager.Instance();
                    if (housingManager == null) return (false, false);

                    bool hasEstate0 = TryGetSharedEstate(0, out _);
                    bool hasEstate1 = TryGetSharedEstate(1, out _);
                    
                    return (hasEstate0, hasEstate1);
                }
            }
            catch
            {
                return (false, false);
            }
        }

        private bool TryGetSharedEstate(int index, out HouseId houseId)
        {
            houseId = default;

            try
            {
                var estate = HousingManager.GetOwnedHouseId(EstateType.SharedEstate, index);
                houseId = estate;

                // Validate ID first
                if (estate.Id == 0)
                    return false;

                // Additional sanity checks to avoid garbage data
                if (estate.WardIndex < 0 || estate.WardIndex > 60)
                    return false;

                if (estate.PlotIndex < 0 || estate.PlotIndex > 60)
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void DrawAvailableSharedEstates(bool hasEstate0, bool hasEstate1)
        {
            // Collect only estates that actually exist
            var estates = new List<(int index, HouseId id)>();

            if (hasEstate0 && TryGetSharedEstate(0, out var estate0))
                estates.Add((0, estate0));

            if (hasEstate1 && TryGetSharedEstate(1, out var estate1))
                estates.Add((1, estate1));

            // If no valid estates, draw a generic fallback button
            if (estates.Count == 0)
            {
                DrawSharedEstateButton("Shared Estate", "Shared Estate", -1);
                return;
            }

            // Draw a button for each detected shared estate, with custom name UI
            // Draw all shared estate buttons with their custom titles
            for (int i = 0; i < estates.Count; i++)
            {
                int idx = estates[i].index;
                string estateInfo = GetEstateDisplayName(estates[i].id);
                string customName = sharedEstateNames.ContainsKey(idx) && !string.IsNullOrWhiteSpace(sharedEstateNames[idx])
                    ? sharedEstateNames[idx]
                    : $"Shared Estate {i + 1}";

                // Draw the star and button with consistent styling
                ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(10, 6));
                ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 3.0f);
                
                // Check if this shared estate is favorited
                string sharedEstateKey = $"Shared:{customName}:{estateInfo}";
                bool isFavorited = FavoritesManager?.IsFavorite(sharedEstateKey) ?? false;
                
                // Draw star button first (consistent with other modules)
                ImGui.PushStyleColor(ImGuiCol.Text, isFavorited ? 
                    new Vector4(1.0f, 0.8f, 0.2f, 1.0f) : // Golden when favorited
                    new Vector4(0.5f, 0.5f, 0.5f, 1.0f)); // Gray when not favorited
                
                if (ImGui.Button($"★##star_shared_{idx}"))
                {
                    if (FavoritesManager != null)
                    {
                        if (isFavorited)
                        {
                            FavoritesManager.RemoveFavorite(sharedEstateKey);
                        }
                        else
                        {
                            // Store the estate index in the aetheryte ID field for shared estates (use special range)
                            uint specialId = (uint)(10000 + idx); // Use ID range 10000+ for shared estates
                            FavoritesManager.AddFavorite(sharedEstateKey, $"{customName}: {estateInfo}", ZoneName, $"SharedEstate_{idx}", specialId, new Vector4(0.369f, 0.506f, 0.675f, 1.0f));
                        }
                    }
                }
                ImGui.PopStyleColor();
                
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip(isFavorited ? "Remove from favorites" : "Add to favorites");
                }
                
                ImGui.SameLine();
                
                // Main shared estate button
                if (teleporter != null)
                {
                    // Plot Button BG: Blue (#5E81AC) with hover states
                    ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.369f, 0.506f, 0.675f, 1.0f));
                    ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.506f, 0.631f, 0.757f, 1.0f));
                    ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.533f, 0.753f, 0.816f, 1.0f));
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.925f, 0.937f, 0.957f, 1.0f)); // Plot Button Text: Off-white (#ECEFF4)

                    bool pressed = ImGui.Button($"{customName}: {estateInfo}###{$"Shared Estate {i + 1}".Replace(" ", "_")}");
                    ImGui.PopStyleColor(4);

                    if (pressed)
                    {
                        ExecuteSharedEstateGroup($"Shared Estate {i + 1}", idx);
                    }
                }
                else
                {
                    // Secondary Buttons: Soft Gray (#4C566A)
                    ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.298f, 0.337f, 0.416f, 0.8f));
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.75f, 0.75f, 0.75f, 0.8f));
                    ImGui.Button($"{customName}: {estateInfo}###disabled_{$"Shared Estate {i + 1}".Replace(" ", "_")}");
                    ImGui.PopStyleColor(2);
                }
                
                ImGui.PopStyleVar(2);
                ImGui.Spacing();
            }

            // Clean separator between estate cards and rename section
            ImGui.Spacing();
            ImGui.Separator();
            ImGui.Spacing();

            // --- Dedicated section for renaming shared estate titles ---
            
            // Headers: Soft Gold (#EBCB8B)
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.922f, 0.796f, 0.545f, 1.0f));
            ImGui.Text("Rename Shared Estate Titles");
            ImGui.PopStyleColor();
            ImGui.Spacing();
            ImGui.Spacing();
            
            // Table with transparent background
            ImGui.PushStyleColor(ImGuiCol.TableBorderStrong, new Vector4(0.0f, 0.0f, 0.0f, 0.0f)); // Remove table borders
            ImGui.PushStyleColor(ImGuiCol.TableBorderLight, new Vector4(0.0f, 0.0f, 0.0f, 0.0f)); // Remove table borders
            ImGui.PushStyleColor(ImGuiCol.TableRowBg, new Vector4(0.0f, 0.0f, 0.0f, 0.0f)); // Clear row background
            ImGui.PushStyleColor(ImGuiCol.TableRowBgAlt, new Vector4(0.0f, 0.0f, 0.0f, 0.0f)); // Clear alt row background
            
            // Table-like layout for rename fields with increased row padding
            if (ImGui.BeginTable("SharedEstateRename", 4, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.PadOuterX))
                {
                    ImGui.TableSetupColumn("Plot", ImGuiTableColumnFlags.WidthFixed, 120.0f);
                    ImGui.TableSetupColumn("Title", ImGuiTableColumnFlags.WidthFixed, 200.0f);
                    ImGui.TableSetupColumn("Rename", ImGuiTableColumnFlags.WidthFixed, 90.0f);
                    ImGui.TableSetupColumn("Reset", ImGuiTableColumnFlags.WidthFixed, 80.0f);

                    for (int i = 0; i < estates.Count; i++)
                    {
                        int idx = estates[i].index;
                        string label = $"Shared Plot {i + 1}";
                        string defaultTitle = label;
                        
                        ImGui.TableNextRow();
                        ImGui.PushID($"shared_estate_rename_table_{idx}");
                        
                        // Plot column - Subheaders: Light Gray (#D8DEE9)
                        ImGui.TableSetColumnIndex(0);
                        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.847f, 0.871f, 0.914f, 1.0f));
                        ImGui.AlignTextToFramePadding();
                        ImGui.Text(label);
                        ImGui.PopStyleColor();
                        
                        // Title input column
                        ImGui.TableSetColumnIndex(1);
                        string buffer = renameBuffers.ContainsKey(idx) ? renameBuffers[idx] : "";
                        ImGui.SetNextItemWidth(-1);
                        if (ImGui.InputText($"##RenameInput_{idx}", ref buffer, 64))
                        {
                            renameBuffers[idx] = buffer;
                        }
                        
                        // Rename button column - Primary Buttons: Desaturated Blue
                        ImGui.TableSetColumnIndex(2);
                        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.369f, 0.506f, 0.675f, 0.8f));
                        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.506f, 0.631f, 0.757f, 0.9f));
                        ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.533f, 0.753f, 0.816f, 1.0f));
                        if (ImGui.Button("Rename") && ImGui.IsItemHovered())
                        {
                            ImGui.SetTooltip("Apply custom name to this shared estate");
                        }
                        bool renamePressed = ImGui.IsItemClicked();
                        ImGui.PopStyleColor(3);
                        
                        if (renamePressed)
                        {
                            string oldName = sharedEstateNames.ContainsKey(idx) && !string.IsNullOrWhiteSpace(sharedEstateNames[idx])
                                ? sharedEstateNames[idx]
                                : $"Shared Estate {i + 1}";
                            string newName = (renameBuffers.ContainsKey(idx) ? renameBuffers[idx] : buffer).Trim();
                            
                            sharedEstateNames[idx] = newName;
                            renameBuffers[idx] = string.Empty;
                            SaveSharedEstateNames();
                            
                            // Update the favorite if it exists
                            UpdateSharedEstateFavorite(idx, oldName, newName, estates[i].id);
                        }
                        
                        // Reset button column - Secondary Buttons: Soft Gray
                        ImGui.TableSetColumnIndex(3);
                        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.298f, 0.337f, 0.416f, 0.8f));
                        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.424f, 0.478f, 0.573f, 0.9f)); // Hover (#6C7A92)
                        ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.524f, 0.578f, 0.673f, 1.0f));
                        if (ImGui.Button("Reset") && ImGui.IsItemHovered())
                        {
                            ImGui.SetTooltip("Reset to default name");
                        }
                        bool resetPressed = ImGui.IsItemClicked();
                        ImGui.PopStyleColor(3);
                        
                        if (resetPressed)
                        {
                            string oldName = sharedEstateNames.ContainsKey(idx) && !string.IsNullOrWhiteSpace(sharedEstateNames[idx])
                                ? sharedEstateNames[idx]
                                : $"Shared Estate {i + 1}";
                                
                            sharedEstateNames[idx] = defaultTitle;
                            renameBuffers[idx] = string.Empty;
                            SaveSharedEstateNames();
                            
                            // Update the favorite if it exists
                            UpdateSharedEstateFavorite(idx, oldName, defaultTitle, estates[i].id);
                        }
                        
                        ImGui.PopID();
                        
                        // Add spacing between rows using dummy
                        if (i < estates.Count - 1)
                        {
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            ImGui.Dummy(new Vector2(0, 8.0f)); // Vertical spacing
                        }
                    }
                    
                    ImGui.EndTable();
                }
                
                ImGui.PopStyleColor(4); // Pop the table style colors
                
                ImGui.Spacing();
                ImGui.Spacing();
        }
        // Local buffer for ImGui input per shared estate index
        private Dictionary<int, string> renameBuffers = new();

        private void DrawDisabledSharedEstateButton()
        {
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.35f, 0.35f, 0.35f, 0.6f));
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.75f, 0.75f, 0.75f, 0.8f));
            ImGui.Button("Shared Estates (Error)");
            ImGui.PopStyleColor(2);
        }

        private string GetEstateDisplayName(HouseId houseId)
        {
            try
            {
                if (houseId.IsApartment)
                {
                    return $"Apartment";
                }
                else
                {
                    return $"Plot {houseId.PlotIndex + 1} - Ward {houseId.WardIndex + 1}";
                }
            }
            catch
            {
                return "Shared Estate";
            }
        }

        private void ExecuteEstateCommand(string estateName)
        {
            try
            {
                // Cast to TeleporterService to access the ExecuteEstateCommand method
                var teleporterService = teleporter as TeleporterService;
                if (teleporterService != null)
                {
                    teleporterService.ExecuteEstateCommand(estateName);
                }
                else
                {
                    // Fallback: show the command info popup
                    ImGui.OpenPopup($"Estate Command Info###{estateName}");
                }
            }
            catch (System.Exception)
            {
                ImGui.OpenPopup($"Estate Command Info###{estateName}");
            }
        }

        private void DrawSharedEstateButton(string buttonText, string groupName, int estateIndex = -1)
        {
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(10, 6)); // Increased padding
            ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 3.0f);
            
            if (teleporter != null)
            {
                // Shared Estates Accent: Lavender (#B48EAD)
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.706f, 0.557f, 0.678f, 0.8f));
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.806f, 0.657f, 0.778f, 0.9f));
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.906f, 0.757f, 0.878f, 1.0f));
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.95f, 0.95f, 0.95f, 1.0f));

                bool pressed = ImGui.Button($"{buttonText}###{groupName.Replace(" ", "_")}");
                ImGui.PopStyleColor(4);

                if (pressed)
                {
                    ExecuteSharedEstateGroup(groupName, estateIndex);
                }
            }
            else
            {
                // Secondary Buttons: Soft Gray (#4C566A)
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.298f, 0.337f, 0.416f, 0.8f));
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.75f, 0.75f, 0.75f, 0.8f));
                ImGui.Button($"{buttonText}###disabled_{groupName.Replace(" ", "_")}");
                ImGui.PopStyleColor(2);
            }
            
            ImGui.PopStyleVar(2);
        }

        private void ExecuteSharedEstateGroup(string groupName, int estateIndex = -1)
        {
            var teleporterService = teleporter as TeleporterService;
            if (teleporterService == null)
            {
                ImGui.OpenPopup($"Shared Estate Info###{groupName}");
                return;
            }

            try
            {
                // Try specific estate teleport first if we have a valid index
                if (estateIndex >= 0 && TryTeleportToSpecificSharedEstate(estateIndex))
                {
                    return; // Success!
                }
                
                // Fallback to standard shared estate command
                teleporterService.ExecuteEstateCommand("Shared Estate");
            }
            catch
            {
                // Final fallback - try standard command or show popup
                try
                {
                    teleporterService.ExecuteEstateCommand("Shared Estate");
                }
                catch
                {
                    ImGui.OpenPopup($"Shared Estate Info###{groupName}");
                }
            }
        }

        private unsafe bool TryTeleportToSpecificSharedEstate(int estateIndex)
        {
            if (estateIndex < 0 || estateIndex >= MAX_SHARED_ESTATES) 
                return false;
            
            try
            {
                var telepo = Telepo.Instance();
                if (telepo == null || (nint)telepo == 0) 
                    return false;

                if (!TryUpdateAetheryteList(telepo))
                    return false;
                
                var teleportList = telepo->TeleportList;
                if (teleportList.LongCount == 0) 
                    return false;
                
                var sharedHouses = GetSharedHouseTeleports(teleportList);
                
                return TryTeleportToEstate(telepo, sharedHouses, estateIndex);
            }
            catch
            {
                return false;
            }
        }

        private unsafe bool TryUpdateAetheryteList(Telepo* telepo)
        {
            try
            {
                telepo->UpdateAetheryteList();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private unsafe System.Collections.Generic.List<(TeleportInfo info, int originalIndex)> GetSharedHouseTeleports(Span<TeleportInfo> teleportList)
        {
            var sharedHouses = new System.Collections.Generic.List<(TeleportInfo info, int originalIndex)>();
            long maxEntries = Math.Min(teleportList.Length, MAX_TELEPORT_ENTRIES);
            
            for (int i = 0; i < maxEntries; i++)
            {
                try
                {
                    var teleportInfo = teleportList[i];
                    if (teleportInfo.IsSharedHouse)
                    {
                        sharedHouses.Add((teleportInfo, i));
                    }
                }
                catch
                {
                    continue; // Skip problematic entries
                }
            }
            
            return sharedHouses;
        }

        private unsafe bool TryTeleportToEstate(Telepo* telepo, System.Collections.Generic.List<(TeleportInfo info, int originalIndex)> sharedHouses, int estateIndex)
        {
            if (estateIndex >= sharedHouses.Count) 
                return false;
                
            var (targetEstate, _) = sharedHouses[estateIndex];
            
            try
            {
                if (targetEstate.AetheryteId > 0 && targetEstate.AetheryteId < MAX_AETHERYTE_ID)
                {
                    return telepo->Teleport(targetEstate.AetheryteId, targetEstate.SubIndex);
                }
            }
            catch
            {
                // Teleport failed
            }
            
            return false;
        }

        private void DrawTeleportInfoPopup()
        {
            // Show estate command info popup
            if (ImGui.BeginPopup("Estate Command Info###Estate Hall") || 
                ImGui.BeginPopup("Estate Command Info###Estate Hall (Free Company)") ||
                ImGui.BeginPopup("Shared Estate Info###Shared Estate"))
            {
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.9f, 0.9f, 0.9f, 1.0f));
                ImGui.Text("To teleport to your estate:");
                ImGui.Spacing();
                ImGui.Text("Type in chat:");
                ImGui.Indent(10.0f);
                ImGui.Text("/tp Estate Hall (for private housing)");
                ImGui.Text("/tp Estate Hall (Free Company) (for FC housing)");
                ImGui.Text("/tp Shared Estate (for shared housing access)");
                ImGui.Spacing();
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.7f, 0.9f, 0.7f, 1.0f)); // Light green
                ImGui.Text("Multiple Shared Estates:");
                ImGui.Text("- Plugin tries to teleport to specific shared estates!");
                ImGui.Text("- Uses extra-safe Telepo system with crash prevention");
                ImGui.Text("- Falls back to standard '/tp Shared Estate' if direct fails");
                ImGui.Text("- Direct teleportation may take you to the correct estate");
                ImGui.Text("- If it fails, you'll get the standard shared estate teleport");
                ImGui.PopStyleColor();
                ImGui.Unindent(10.0f);
                ImGui.PopStyleColor();
                
                if (ImGui.Button("Close"))
                {
                    ImGui.CloseCurrentPopup();
                }
                
                ImGui.EndPopup();
            }
        }

        private void DrawHousingInformation()
        {
            ImGui.Spacing();
            ImGui.Separator();
            ImGui.Spacing();

            // Headers: Soft Gold (#EBCB8B)
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.922f, 0.796f, 0.545f, 1.0f));
            ImGui.Text("Housing Information");
            ImGui.PopStyleColor();

            ImGui.Spacing();

            // Subheaders: Light Gray (#D8DEE9)
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.847f, 0.871f, 0.914f, 1.0f));
            ImGui.TextWrapped("Housing Districts:");
            ImGui.PopStyleColor();
            
            // Description Text: Muted Gray (#B0B0B0)
            ImGui.Indent(10.0f);
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.690f, 0.690f, 0.690f, 1.0f));
            ImGui.Text("The Mist (Limsa)");
            ImGui.Text("Lavender Beds (Gridania)");
            ImGui.Text("The Goblet (Ul'dah)");
            ImGui.Text("Shirogane (Kugane)");
            ImGui.Text("Empyreum (Ishgard)");
            ImGui.PopStyleColor();
            ImGui.Unindent(10.0f);

            ImGui.Spacing();
            // Description Text: Muted Gray (#B0B0B0)
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.690f, 0.690f, 0.690f, 1.0f));
            ImGui.TextWrapped("- Estate teleportation requires ownership or access.\n- Shared estate buttons teleport directly to each available shared estate.");
            ImGui.PopStyleColor();
        }

        private void UpdateSharedEstateFavorite(int estateIndex, string oldName, string newName, HouseId houseId)
        {
            if (FavoritesManager == null) return;
            
            // Generate the old and new favorite keys
            string estateInfo = GetEstateDisplayName(houseId);
            string oldKey = $"Shared:{oldName}:{estateInfo}";
            string newKey = $"Shared:{newName}:{estateInfo}";
            
            // Check if the old favorite exists
            if (FavoritesManager.IsFavorite(oldKey))
            {
                // Remove the old favorite and add the new one
                FavoritesManager.RemoveFavorite(oldKey);
                
                // Add the new favorite with the updated name
                uint specialId = (uint)(10000 + estateIndex);
                FavoritesManager.AddFavorite(newKey, $"{newName}: {estateInfo}", ZoneName, $"SharedEstate_{estateIndex}", specialId, new Vector4(0.369f, 0.506f, 0.675f, 1.0f));
            }
        }
    }
}
