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
        // Constants for better maintainability
        private const int MAX_SHARED_ESTATES = 2;
        private const int MAX_TELEPORT_ENTRIES = 50;
        private const int MAX_AETHERYTE_ID = 10000;
        
        // Housing districts and their teleport IDs
        private static readonly Dictionary<string, uint> HousingDistricts = new()
        {
            { "The Mist", 8 },          // Limsa Lominsa Lower Decks
            { "Lavender Beds", 2 },     // New Gridania  
            { "The Goblet", 9 },        // Ul'dah - Steps of Nald
            { "Shirogane", 111 },       // Kugane
            { "Empyreum", 70 }          // Foundation
        };
        
        // Favorite district settings
        private string selectedDistrict = "The Mist";
        private string favoriteDistrict = "";
        
        public override string ZoneName => "Housing";
        public override string LevelRange => "Any Level";
        public override Vector4 Color => new Vector4(0.8f, 0.6f, 0.4f, 1.0f); // Warm brown/orange for housing

        public HousingModule(ITeleporterIpc? teleporter) : base(teleporter) 
        {
            LoadFavoriteWard();
        }

        public override void DrawContent()
        {
            DrawZoneHeader("Housing & Residences", "Any Level", Color);
            
            DrawZoneSection("Estate Teleportation", "Any Level",
                "Direct teleport to your housing (if you own any)",
                () => {
                    DrawEstateButtons();
                    ImGui.Spacing();
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.8f, 0.9f, 0.8f, 1.0f)); // Light green
                    ImGui.TextWrapped("These buttons use the game's estate teleportation system:");
                    ImGui.Indent(10.0f);
                    ImGui.Text("• Only work if you own housing or have FC housing access");
                    ImGui.Text("• Will show an error if you don't have housing");
                    ImGui.Unindent(10.0f);
                    ImGui.PopStyleColor();
                }, Color);
            
            DrawFavoriteWardSection();
            
            DrawHousingInformation();
            DrawTeleportInfoPopup();
        }

        private void DrawEstateButtons()
        {
            // Private Estate Button
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(10, 6));
            ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 3.0f);
            
            if (teleporter != null)
            {
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.4f, 0.6f, 0.4f, 0.8f)); // Green theme for estate
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.5f, 0.7f, 0.5f, 0.9f));
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.6f, 0.8f, 0.6f, 1.0f));
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.95f, 0.95f, 0.95f, 1.0f));

                bool pressedPrivate = ImGui.Button("🏠 Private Estate Hall###private_estate");
                ImGui.PopStyleColor(4);

                if (pressedPrivate)
                {
                    ExecuteEstateCommand("Estate Hall");
                }
            }
            else
            {
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.35f, 0.35f, 0.35f, 0.6f));
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.75f, 0.75f, 0.75f, 0.8f));
                ImGui.Button("🏠 Private Estate Hall###disabled_private");
                ImGui.PopStyleColor(2);
            }

            ImGui.SameLine();

            // FC Estate Button
            if (teleporter != null)
            {
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.4f, 0.4f, 0.6f, 0.8f)); // Blue theme for FC
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.5f, 0.5f, 0.7f, 0.9f));
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.6f, 0.6f, 0.8f, 1.0f));
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.95f, 0.95f, 0.95f, 1.0f));

                bool pressedFC = ImGui.Button("🏰 FC Estate Hall###fc_estate");
                ImGui.PopStyleColor(4);

                if (pressedFC)
                {
                    ExecuteEstateCommand("Estate Hall (Free Company)");
                }
            }
            else
            {
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.35f, 0.35f, 0.35f, 0.6f));
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.75f, 0.75f, 0.75f, 0.8f));
                ImGui.Button("🏰 FC Estate Hall###disabled_fc");
                ImGui.PopStyleColor(2);
            }

            // Add a new line for shared estate buttons
            ImGui.NewLine();
            DrawSharedEstateSection();

            ImGui.PopStyleVar(2);
        }

        private void DrawSharedEstateSection()
        {
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.8f, 0.8f, 0.6f, 1.0f)); // Light yellow
            ImGui.Text("Shared Estates:");
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
                    DrawSharedEstateButton("🏡 Shared Estate", "Shared Estate", -1);
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
                DrawSharedEstateButton("🏡 Shared Estate", "Shared Estate", -1);
                return;
            }

            // Draw a button for each detected shared estate
            for (int i = 0; i < estates.Count; i++)
            {
                if (i > 0) ImGui.SameLine();
                string estateInfo = GetEstateDisplayName(estates[i].id);
                DrawSharedEstateButton($"🏡 Shared Estate {i + 1} - {estateInfo}", $"Shared Estate {i + 1}", estates[i].index);
            }
        }

        private void DrawDisabledSharedEstateButton()
        {
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.35f, 0.35f, 0.35f, 0.6f));
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.75f, 0.75f, 0.75f, 0.8f));
            ImGui.Button("🏡 Shared Estates (Error)");
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
                    return $"Plot {houseId.PlotIndex + 1}, Ward {houseId.WardIndex + 1}";
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
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(8, 4));
            ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 3.0f);
            
            if (teleporter != null)
            {
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.6f, 0.4f, 0.6f, 0.8f)); // Purple theme for shared estates
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.7f, 0.5f, 0.7f, 0.9f));
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.8f, 0.6f, 0.8f, 1.0f));
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
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.35f, 0.35f, 0.35f, 0.6f));
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

        private void DrawFavoriteWardSection()
        {
            DrawZoneSection("Favorite District", "Any Level",
                "Set and teleport to your favorite housing district.\n" +
                "*This will not teleport you to a specific ward, only the district.*",
                () => {
                    // Show current favorite if set
                    if (!string.IsNullOrEmpty(favoriteDistrict))
                    {
                        DrawFavoriteWardButton();
                        ImGui.Spacing();
                    }
                    
                    DrawFavoriteWardSettings();
                }, Color);
        }

        private void DrawFavoriteWardButton()
        {
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(10, 6));
            ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 3.0f);
            
            // Gold/star theme for favorite
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.8f, 0.7f, 0.2f, 0.8f)); 
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.9f, 0.8f, 0.3f, 0.9f));
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(1.0f, 0.9f, 0.4f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.1f, 0.1f, 0.1f, 1.0f)); // Dark text

            string buttonText = $"⭐ {favoriteDistrict}";
            bool pressed = ImGui.Button($"{buttonText}###favorite_district");
            ImGui.PopStyleColor(4);
            ImGui.PopStyleVar(2);

            if (pressed && HousingDistricts.ContainsKey(favoriteDistrict) && teleporter != null)
            {
                try
                {
                    teleporter.Teleport(HousingDistricts[favoriteDistrict]);
                }
                catch (System.Exception)
                {
                    // Silently fail if teleport is not available
                }
            }
            
            ImGui.SameLine();
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.9f, 0.8f, 0.2f, 1.0f)); // Gold text
            ImGui.Text("Favorite District");
            ImGui.PopStyleColor();
        }

        private void DrawFavoriteWardSettings()
        {
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.9f, 0.9f, 0.9f, 1.0f));
            ImGui.Text("Set Favorite District:");
            ImGui.PopStyleColor();
            
            ImGui.Spacing();
            
            // District dropdown
            ImGui.PushItemWidth(200);
            if (ImGui.BeginCombo("##district_combo", selectedDistrict))
            {
                foreach (var district in HousingDistricts.Keys)
                {
                    bool isSelected = selectedDistrict == district;
                    if (ImGui.Selectable(district, isSelected))
                    {
                        selectedDistrict = district;
                    }
                    if (isSelected)
                    {
                        ImGui.SetItemDefaultFocus();
                    }
                }
                ImGui.EndCombo();
            }
            ImGui.PopItemWidth();
            
            ImGui.SameLine();
            
            // Set Favorite button
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.2f, 0.7f, 0.2f, 0.8f)); // Green
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.3f, 0.8f, 0.3f, 0.9f));
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.4f, 0.9f, 0.4f, 1.0f));
            
            if (ImGui.Button("Set Favorite"))
            {
                SetFavoriteWard(selectedDistrict, 0); // Ward is no longer relevant
            }
            ImGui.PopStyleColor(3);
            
            // Clear favorite button if one is set
            if (!string.IsNullOrEmpty(favoriteDistrict))
            {
                ImGui.SameLine();
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.7f, 0.3f, 0.3f, 0.8f)); // Red
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.8f, 0.4f, 0.4f, 0.9f));
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.9f, 0.5f, 0.5f, 1.0f));
                
                if (ImGui.Button("Clear"))
                {
                    ClearFavoriteWard();
                }
                ImGui.PopStyleColor(3);
            }
            
            ImGui.Spacing();
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.7f, 0.7f, 0.7f, 1.0f)); // Gray
            ImGui.TextWrapped("Note: This teleports to the main city near the housing district.");
            ImGui.PopStyleColor();
        }

        private void SetFavoriteWard(string district, int ward)
        {
            favoriteDistrict = district;
            SaveFavoriteWard();
        }

        private void ClearFavoriteWard()
        {
            favoriteDistrict = "";
            SaveFavoriteWard();
        }

        private void SaveFavoriteWard()
        {
            try
            {
                // For now, we'll use a simple file-based persistence
                // This ensures the feature works without complex configuration dependencies
                string configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ZoneLevelGuide");
                Directory.CreateDirectory(configPath);
                
                string configFile = Path.Combine(configPath, "favorite_ward.txt");
                File.WriteAllText(configFile, favoriteDistrict);
            }
            catch
            {
                // If file save fails, we'll just keep it in memory for this session
                // This is a fallback to prevent crashes
            }
        }

        private void LoadFavoriteWard()
        {
            try
            {
                string configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ZoneLevelGuide");
                string configFile = Path.Combine(configPath, "favorite_ward.txt");
                
                if (File.Exists(configFile))
                {
                    string content = File.ReadAllText(configFile).Trim();
                    
                    if (!string.IsNullOrEmpty(content) && HousingDistricts.ContainsKey(content))
                    {
                        favoriteDistrict = content;
                    }
                }
            }
            catch
            {
                // If loading fails, start with defaults
                favoriteDistrict = "";
            }
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
                ImGui.Text("✓ Plugin tries to teleport to specific shared estates!");
                ImGui.Text("✓ Uses extra-safe Telepo system with crash prevention");
                ImGui.Text("✓ Falls back to standard '/tp Shared Estate' if direct fails");
                ImGui.Text("• Direct teleportation may take you to the correct estate");
                ImGui.Text("• If it fails, you'll get the standard shared estate teleport");
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

            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.9f, 0.9f, 0.9f, 1.0f));
            ImGui.Text("🏠 Housing Information");
            ImGui.PopStyleColor();

            ImGui.Spacing();

            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.85f, 0.85f, 0.85f, 1.0f));
            ImGui.TextWrapped("Housing Districts:");
            ImGui.Indent(10.0f);
            ImGui.Text("The Mist (Limsa)");
            ImGui.Text("Lavender Beds (Gridania)");
            ImGui.Text("The Goblet (Ul'dah)");
            ImGui.Text("Shirogane (Kugane)");
            ImGui.Text("Empyreum (Ishgard)");
            ImGui.Unindent(10.0f);
            ImGui.PopStyleColor();

            ImGui.Spacing();
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.6f, 0.9f, 0.9f, 1.0f)); // Light cyan
            ImGui.TextWrapped("• Estate teleportation requires ownership or access.\n• Shared estate buttons teleport directly to each available shared estate.");
            ImGui.PopStyleColor();
        }
    }
}
