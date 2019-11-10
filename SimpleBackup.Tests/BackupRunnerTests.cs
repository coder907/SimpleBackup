using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Xunit;

using SimpleBackup.Configuration;

namespace SimpleBackup.Tests
{
    public class BackupRunnerTests
    {
        [Fact]
        public void TestFileToFile()
        {
            var folder = Path.Combine(Config.CurrentDirectory, "TestBackup");
            DeleteFolder(folder);

            try
            {
                var config = Config.Get("FileToFile.Config.json");

                var runner = new BackupRunner(config);
                runner.Run();

                Assert.True(Directory.Exists(folder));
                Assert.True(File.Exists(Path.Combine(folder, "TestFile.txt")));
            }
            finally
            {
                DeleteFolder(folder);
            }
        }

        [Fact]
        public void TestFileToFolder()
        {
            var folder = Path.Combine(Config.CurrentDirectory, "TestBackup");
            DeleteFolder(folder);

            try
            {
                var config = Config.Get("FileToFolder.Config.json");

                var runner = new BackupRunner(config);
                runner.Run();

                Assert.True(Directory.Exists(folder));
                Assert.True(File.Exists(Path.Combine(folder, "TestDestinationFolder", "TestFile.txt")));
            }
            finally
            {
                DeleteFolder(folder);
            }
        }

        [Fact]
        public void TestFolderToFolder()
        {
            var folder = Path.Combine(Config.CurrentDirectory, "TestBackup");
            DeleteFolder(folder);

            try
            {
                var config = Config.Get("FolderToFolder.Config.json");

                var runner = new BackupRunner(config);
                runner.Run();

                Assert.True(Directory.Exists(folder));
                Assert.True(Directory.Exists(Path.Combine(folder, "TestFolder")));
                Assert.True(Directory.Exists(Path.Combine(folder, "TestFolder", "TestSubFolder")));

                Assert.True(File.Exists(Path.Combine(folder, "TestFolder", "TestFolderFile.txt")));
                Assert.True(File.Exists(Path.Combine(folder, "TestFolder", "TestSubFolder", "TestSubFolderFile.txt")));
            }
            finally
            {
                DeleteFolder(folder);
            }
        }

        [Fact]
        public void TestBackup()
        {
            var folder = Path.Combine(Config.CurrentDirectory, "TestBackup");
            DeleteFolder(folder);

            try
            {
                var config = Config.Get("Backup.Config.json");

                var runner = new BackupRunner(config);
                runner.Run();

                Assert.True(Directory.Exists(folder));
                Assert.True(Directory.Exists(Path.Combine(folder, "TestDestinationFolder1")));
                Assert.True(Directory.Exists(Path.Combine(folder, "TestDestinationFolder2")));
                Assert.True(Directory.Exists(Path.Combine(folder, "TestDestinationFolder2", "TestSubFolder")));

                Assert.True(File.Exists(Path.Combine(folder, "TestDestinationFolder1", "TestFile.txt")));
                Assert.True(File.Exists(Path.Combine(folder, "TestDestinationFolder1", "TestFolderFile.txt")));
                Assert.True(File.Exists(Path.Combine(folder, "TestDestinationFolder1", "TestSubFolderFile.txt")));
                Assert.True(File.Exists(Path.Combine(folder, "TestDestinationFolder2", "TestFolderFile.txt")));
                Assert.True(File.Exists(Path.Combine(folder, "TestDestinationFolder2", "TestSubFolderFile.txt")));
                Assert.True(File.Exists(Path.Combine(folder, "TestDestinationFolder2", "TestSubFolder", "TestSubFolderFile.txt")));
            }
            finally
            {
                DeleteFolder(folder);
            }
        }        

        private void DeleteFolder(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }
    }
}
