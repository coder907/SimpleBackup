using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBackup.Configuration
{
    public class ConfigException : Exception
    {
        public ConfigException(string message) : base(message)
        {
        }
    }
}