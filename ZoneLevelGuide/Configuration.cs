using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace ZoneLevelGuide
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;
        
        // Global shared estate names (kept for backward compatibility but will be character-specific)
        public Dictionary<int, string> SharedEstateNames { get; set; } = new();
        
        // Legacy favorites storage (for migration purposes)
        public Dictionary<string, FavoriteTeleportData> FavoriteTeleports { get; set; } = new();
        
        // New character-specific storage
        public Dictionary<string, CharacterProfile> CharacterProfiles { get; set; } = new();
        
        [NonSerialized]
        private IDalamudPluginInterface? pluginInterface;
        
        public void Initialize(IDalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;
            MigrateLegacyData();
        }
        
        private void MigrateLegacyData()
        {
            // Migrate legacy favorites to a default character profile if they exist
            if (FavoriteTeleports.Count > 0 && CharacterProfiles.Count == 0)
            {
                var defaultProfile = new CharacterProfile
                {
                    FavoriteTeleports = new Dictionary<string, FavoriteTeleportData>(FavoriteTeleports),
                    SharedEstateNames = new Dictionary<int, string>(SharedEstateNames)
                };
                CharacterProfiles["__legacy__"] = defaultProfile;
                
                // Clear legacy data after migration
                FavoriteTeleports.Clear();
            }
        }
        
        public CharacterProfile GetCharacterProfile(string characterKey)
        {
            if (!CharacterProfiles.ContainsKey(characterKey))
            {
                CharacterProfiles[characterKey] = new CharacterProfile();
            }
            return CharacterProfiles[characterKey];
        }
        
        public void Save()
        {
            pluginInterface?.SavePluginConfig(this);
        }
    }
    
    [Serializable]
    public class CharacterProfile
    {
        public Dictionary<string, FavoriteTeleportData> FavoriteTeleports { get; set; } = new();
        public Dictionary<int, string> SharedEstateNames { get; set; } = new();
    }
    
    [Serializable]
    public class FavoriteTeleportData
    {
        public string Name { get; set; } = "";
        public string Zone { get; set; } = "";
        public string Command { get; set; } = "";
        public uint AetheryteId { get; set; }
        public Vector4 ButtonColor { get; set; }
        public DateTime AddedDate { get; set; }
        public int Order { get; set; }
    }
}
