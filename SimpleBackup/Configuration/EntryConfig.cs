using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBackup.Configuration
{
    public class EntryConfig
    {
        public string SourceFolder { get; set; }

        public string SourceFile { get; set; }

        public string DestinationFolder { get; set; }

        public string DestinationFile { get; set; }
    }
}
