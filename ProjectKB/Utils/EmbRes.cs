using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProjectKB.Utils
{
    public static class EmbRes
    {
        public static Assembly assembly = typeof(EmbRes).Assembly;

        public static void LogNames()
        {
            Debug.WriteLine("EmbRes Names:");
            foreach (var name in assembly.GetManifestResourceNames())
            {
                Debug.WriteLine(name);
            }
        }

        public static Stream GetResourceStream(string resourceName)
        {
            string frn = assembly.GetName().Name + "." + resourceName;
            return assembly.GetManifestResourceStream(frn);
        }
    }
}
