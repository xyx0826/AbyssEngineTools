namespace Kaamo.Texture.Enums
{
    /// <summary>
    /// Abyss Engine versions.
    /// </summary>
    internal enum Engine
    {
        /// <summary>
        /// Used by Galaxy on Fire 2.
        /// </summary>
        AbyssEngine2,

        /// <summary>
        /// Used by Galaxy on Fire: Alliances.
        /// </summary>
        AbyssEngine3,

        /// <summary>
        /// Used by Galaxy on Fire: Manticore.
        /// </summary>
        AbyssEngine4
    }

    /// <summary>
    /// Products built on Abyss Engine.
    /// </summary>
    internal enum Product
    {
        GalaxyOnFire2,
        GalaxyOnFireAlliances,
        GalaxyOnFireManticore
    }

    /// <summary>
    /// Platforms supported by Abyss Engine products.
    /// </summary>
    internal enum Platform
    {
        Android,
        Ios,
        Windows,
        Mac,
        AppleTv,
        Switch
    }
}
