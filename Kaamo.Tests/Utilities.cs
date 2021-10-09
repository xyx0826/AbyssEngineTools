using System.IO;

namespace Kaamo.Tests
{
    internal static class Utilities
    {
        private const string ResourceFolder = "./Resources";

        public static string GetResourcePath(string fileName)
            => Path.Combine(ResourceFolder, fileName);
    }
}
