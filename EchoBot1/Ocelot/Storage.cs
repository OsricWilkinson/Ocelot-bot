using System.IO;
using System.Reflection;

namespace Ocelot
{
    public class Storage
    {
        public static Process LoadProcess()
        {
            // Difference between dev and live.
            // sigh.
            // (Live doesn't need the relative bits)
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\oct90001.json");
            if (!File.Exists(path))
            {
                path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"..\..\..\Resources\oct90001.json");
            }
            return new Process(path);
        }
    }
}
