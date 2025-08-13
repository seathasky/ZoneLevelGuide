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
        
        public Dictionary<int, string> SharedEstateNames { get; set; } = new();
        
        public Dictionary<string, FavoriteTeleportData> FavoriteTeleports { get; set; } = new();
        
        [NonSerialized]
        private IDalamudPluginInterface? pluginInterface;
        
        public void Initialize(IDalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;
        }
        
        public void Save()
        {
            pluginInterface?.SavePluginConfig(this);
        }
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
