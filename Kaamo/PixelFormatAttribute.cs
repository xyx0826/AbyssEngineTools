using System;
using Kaamo.Enums;

namespace Kaamo
{
    /// <summary>
    /// Indicates that a pixel format is in use by an <see cref="Engine"/>
    /// in the form of a binary enum value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    internal class PixelFormatAttribute : Attribute
    {
        public readonly Engine EngineVersion;

        public readonly byte Value;

        public PixelFormatAttribute(Engine engineVersion, byte value)
        {
            EngineVersion = engineVersion;
            Value = value;
        }
    }
}
