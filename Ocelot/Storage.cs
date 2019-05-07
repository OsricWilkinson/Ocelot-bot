using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ocelot
{
    public class Storage
    {
        public static Process LoadProcess()
        {
            return new Process(@"..\..\Resources\oct90001.json");
        }
    }
}
