using System.IO;
using System.Reflection;

namespace Ocelot
{
    public class Storage
    {
        public static Process LoadProcess()
        {

            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\oct90001.json");
            return new Process(path);
        }
    }
}
