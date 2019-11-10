using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

using Serilog;

using SimpleBackup.Configuration;

namespace SimpleBackup
{
    public class BackupRunner
    {
        private readonly Config config;

        public BackupRunner(Config config)
        {
            this.config = config;
        }

        public void Run()
        {
            Log.Information("Starting backup process ...");
            var startTime = DateTime.Now;

            var backupFolder = Path.Combine(config.RootFolder, config.BackupFolder);
            EnsureFolder(backupFolder);
            ProcessEntries();
            CreateZipArchive(backupFolder);

            Log.Information("Backup completed in {0:hh\\:mm\\:ss}.", DateTime.Now - startTime);
            Log.Information("Press any key to continue . . .");
#if !DEBUG
            Console.ReadKey();
#endif
        }

        private void EnsureFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                Log.Information($"Creating folder {path} ...");
                Directory.CreateDirectory(path);
            }
        }

        private void ProcessEntries()
        {
            foreach (var entry in config.Entries)
            {
                ProcessEntry(entry);
            }
        }

        private void ProcessEntry(EntryConfig entry)
        {
            if (!string.IsNullOrEmpty(entry.SourceFolder) && !string.IsNullOrEmpty(entry.DestinationFolder))
            {
                FolderToFolder(entry);
            }
            else if (!string.IsNullOrEmpty(entry.SourceFile) && !string.IsNullOrEmpty(entry.DestinationFile))
            {
                FileToFile(entry);
            }
            else if (!string.IsNullOrEmpty(entry.SourceFile) && !string.IsNullOrEmpty(entry.DestinationFolder))
            {
                FileToFolder(entry);
            }
        }

        private void FolderToFolder(EntryConfig entry)
        {
            // Copy files
            foreach (var source in Directory.GetFiles(entry.SourceFolder, "*.*", SearchOption.AllDirectories))
            {
                var destination = source.Replace(entry.SourceFolder, entry.DestinationFolder);
                EnsureFolder(Path.GetDirectoryName(destination));
                Log.Information($"Copying file {source} to {destination} ...");
                File.Copy(source, destination, true);
            }
        }

        private void FileToFolder(EntryConfig entry)
        {
            EnsureFolder(entry.DestinationFolder);
            var destination = Path.Combine(entry.DestinationFolder, Path.GetFileName(entry.SourceFile));
            Log.Information($"Copying file {entry.SourceFile} to {destination} ...");
            File.Copy(entry.SourceFile, destination, true);
        }

        private void FileToFile(EntryConfig entry)
        {
            Log.Information($"Copying file {entry.SourceFile} to {entry.DestinationFile} ...");
            File.Copy(entry.SourceFile, entry.DestinationFile, true);
        }

        private void CreateZipArchive(string backupFolder)
        {
            if (config.CreateZipArchive)
            {
                try
                {
                    var backupZipFile = Path.Combine(config.RootFolder, $"{config.BackupFolder}.zip");
                    Log.Information($"Creating archive {backupZipFile} ...");
                    ZipFile.CreateFromDirectory(backupFolder, backupZipFile);
                }
                catch (Exception ex)
                {
                    Log.Error($"Error creating ZIP archive. {ex}", ex);
                }

                try
                {
                    Log.Information($"Deleting backup folder {backupFolder}");
                    Directory.Delete(backupFolder, true);
                }
                catch (Exception ex)
                {
                    Log.Error($"Unable to delete backup folder. {ex}", ex);
                }
            }
        }        
    }
}
