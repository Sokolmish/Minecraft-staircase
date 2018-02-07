using System;

namespace Minecraft_staircase
{
    public struct PixelData
    {
        public int ID { get; set; }
        public int[] NormalColor { get; set; }
        public int[] DarkColor { get; set; }
        public int[] LightColor { get; set; }
    }

    /// <summary>
    /// Block defined by a set
    /// </summary>
    public struct UnsettedBlock
    {
        public int ID { get; set; }
        public ColorType Set { get; set; }
    }

    /// <summary>
    /// Block defined by a height
    /// </summary>
    public struct SettedBlock
    {
        public int ID { get; set; }
        public int Height { get; set; }
    }

    public enum ColorType
    {
        /// <summary>
        /// Downward block
        /// </summary>
        Dark = 0,
        /// <summary>
        /// On level block
        /// </summary>
        Normal = 1,
        /// <summary>
        /// Upward block
        /// </summary>
        Light = 2
    }

    public enum ArtType
    {
        Flat = 0,
        Lite = 1,
        Full = 2
    }
}