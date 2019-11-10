using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.RegularExpressions;

using Serilog;

namespace SimpleBackup.Configuration
{
    public class Config
    {
        private static Config instance;

        #region Configurable Properties

        public string RootFolder { get; set; }

        public string BackupFolder { get; set; }

        public IList<EntryConfig> Entries { get; set; }

        public bool CreateZipArchive { get; set; }

        #endregion

        public static string CurrentDirectory
        {
            get
            {
                var exeFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
#if DEBUG
                var projectFolder = Regex.Split(exeFolder, @"\\bin\\Debug\\")[0];
                return projectFolder;
#else
                return exeFolder;
#endif                
            }
        }

        public static Config Get(string fileName = "Config.json")
        {
            if (instance == null)
            {
                var path = Path.Combine(CurrentDirectory, fileName);
                var json = File.ReadAllText(path);

                instance = JsonSerializer.Deserialize<Config>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                instance.Validate();
            }

            return instance;
        }

        public void Validate()
        {
            if (string.IsNullOrEmpty(RootFolder))
            {
                RootFolder = CurrentDirectory;
            }

            if (string.IsNullOrEmpty(BackupFolder))
            {
                BackupFolder = string.Format(@"Backup-{0}-{1:00}-{2:00}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            }            

            foreach (var entry in Entries)
            {
                if (string.IsNullOrEmpty(entry.SourceFolder) && string.IsNullOrEmpty(entry.SourceFile))
                {
                    throw new ConfigException(@"Entry source must be specified.");
                }

                if (!string.IsNullOrEmpty(entry.SourceFolder) && !string.IsNullOrEmpty(entry.SourceFile))
                {
                    throw new ConfigException(@"Either entry source folder or entry source file must be specified, but not both.");
                }

                if (!string.IsNullOrEmpty(entry.SourceFolder) && !Path.IsPathFullyQualified(entry.SourceFolder))
                {
                    entry.SourceFolder = Path.Combine(CurrentDirectory, entry.SourceFolder);
                }
                else if (!string.IsNullOrEmpty(entry.SourceFile) && !Path.IsPathFullyQualified(entry.SourceFile))
                {
                    entry.SourceFile = Path.Combine(CurrentDirectory, entry.SourceFile);
                }

                if (string.IsNullOrEmpty(entry.DestinationFolder) && string.IsNullOrEmpty(entry.DestinationFile))
                {
                    if (!string.IsNullOrEmpty(entry.SourceFolder))
                    {
                        entry.DestinationFolder = entry.SourceFolder.Substring(entry.SourceFolder.LastIndexOf("\\") + 1);
                    }
                    else if (!string.IsNullOrEmpty(entry.SourceFile))
                    {
                        entry.DestinationFile = Path.GetFileName(entry.SourceFile);
                    }
                }

                if (!string.IsNullOrEmpty(entry.DestinationFolder) && !string.IsNullOrEmpty(entry.DestinationFile))
                {
                    throw new ConfigException(@"Either entry destination folder or entry destination file must be specified, but not both.");
                }

                if (!string.IsNullOrEmpty(entry.SourceFolder) && !string.IsNullOrEmpty(entry.DestinationFile))
                {
                    throw new ConfigException(@"Backing up a folder to a file is not supported.");
                }

                if (!string.IsNullOrEmpty(entry.DestinationFolder) && !Path.IsPathFullyQualified(entry.DestinationFolder))
                {
                    entry.DestinationFolder = Path.Combine(RootFolder, BackupFolder, entry.DestinationFolder);
                }
                else if (!string.IsNullOrEmpty(entry.DestinationFile) && !Path.IsPathFullyQualified(entry.DestinationFile))
                {
                    entry.DestinationFile = Path.Combine(RootFolder, BackupFolder, entry.DestinationFile);
                }
            }
        }
    }
}
