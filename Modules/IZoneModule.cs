using System;
using System.Numerics;

namespace ZoneLevelGuide.Modules
{
    public interface IZoneModule
    {
        void DrawContent();
        string ZoneName { get; }
        string LevelRange { get; }
        Vector4 Color { get; }
    }
}
