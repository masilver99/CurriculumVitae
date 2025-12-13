using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CV.Models
{
    public class VersionInfo
    {
        public string BuildDate { get; }

        public VersionInfo()
        {
            BuildDate = System.IO.File.GetLastWriteTime(System.Reflection.Assembly.GetExecutingAssembly().Location).ToString("dd-MMM-yyyy HH:mm:ss");
        }
    }
}
