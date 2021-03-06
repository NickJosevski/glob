﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace Glob.Tests
{
    public class GlobTests
    {
        [Fact]
        public void CanParseSimpleFilename()
        {
            var glob = new Glob("*.txt");
            Assert.True(glob.IsMatch("file.txt"));
            Assert.False(glob.IsMatch("file.zip"));
            Assert.True(glob.IsMatch(@"c:\windows\file.txt"));
        }

        [Fact]
        public void CanParseDots()
        {
            var glob = new Glob("/some/dir/folder/foo.*");
            Assert.True(glob.IsMatch("/some/dir/folder/foo.txt"));
            Assert.True(glob.IsMatch("/some/dir/folder/foo.csv"));
        }

        [Fact]
        public void CanMatchSingleFile()
        {
            var glob = new Glob("*file.txt");
            Assert.True(glob.IsMatch("bigfile.txt"));
            Assert.True(glob.IsMatch("smallfile.txt"));
        }


        [Fact]
        public void CanMatchSingleFileOnExtension()
        {
            var glob = new Glob("folder/*.txt");
            Assert.True(glob.IsMatch("folder/bigfile.txt"));
            Assert.True(glob.IsMatch("folder/smallfile.txt"));
            Assert.False(glob.IsMatch("folder/smallfile.txt.min"));
        }

        [Fact]
        public void CanMatchSingleFileWithAnyNameOrExtension()
        {
            var glob = new Glob("folder/*.*");
            Assert.True(glob.IsMatch("folder/bigfile.txt"));
            Assert.True(glob.IsMatch("folder/smallfile.txt"));
            Assert.True(glob.IsMatch("folder/smallfile.txt.min"));
        }

        [Fact]
        public void CanMatchSingleFileUsingCharRange()
        {
            var glob = new Glob("*fil[e-z].txt");
            Assert.True(glob.IsMatch("bigfile.txt"));
            Assert.True(glob.IsMatch("smallfilf.txt"));
            Assert.False(glob.IsMatch("smallfila.txt"));
            Assert.False(glob.IsMatch("smallfilez.txt"));
        }

        [Fact]
        public void CanMatchSingleFileUsingNumberRange()
        {
            var glob = new Glob("*file[1-9].txt");
            Assert.True(glob.IsMatch("bigfile1.txt"));
            Assert.True(glob.IsMatch("smallfile8.txt"));
            Assert.False(glob.IsMatch("smallfile0.txt"));
            Assert.False(glob.IsMatch("smallfilea.txt"));
        }

        [Fact]
        public void CanMatchSingleFileUsingCharList()
        {
            var glob = new Glob("*file[abc].txt");
            Assert.True(glob.IsMatch("bigfilea.txt"));
            Assert.True(glob.IsMatch("smallfileb.txt"));
            Assert.False(glob.IsMatch("smallfiled.txt"));
            Assert.False(glob.IsMatch("smallfileaa.txt"));
        }

        [Fact]
        public void CanMatchSingleFileUsingInvertedCharList()
        {
            var glob = new Glob("*file[!abc].txt");
            Assert.False(glob.IsMatch("bigfilea.txt"));
            Assert.False(glob.IsMatch("smallfileb.txt"));
            Assert.True(glob.IsMatch("smallfiled.txt"));
            Assert.True(glob.IsMatch("smallfile-.txt"));
            Assert.False(glob.IsMatch("smallfileaa.txt"));
        }

        [Fact]
        public void CanMatchBinFolderGlob()
        {
            var root = new DirectoryInfo(@"C:\");
            var allBinFolders = root.GlobDirectories("**/bin");

            Assert.True(allBinFolders.Any(), "There should be some bin folders");
        }

        [Fact]
        public void CanMatchDllExtension()
        {

            var root = new DirectoryInfo(@"C:\");
            var allDllFiles = root.GlobFiles("**/*.dll");

            Assert.True(allDllFiles.Any(), "There should be some DLL files");
        }

        [Fact]
        public void CanMatchInfoInFileSystemInfo()
        {

            var root = new DirectoryInfo(@"C:\");
            var allInfoFilesAndFolders = root.GlobFileSystemInfos("**/*info");

            Assert.True(allInfoFilesAndFolders.Any(), "There should be some 'allInfoFilesAndFolders'");
        }

        [Fact]
        public void CanMatchConfigFilesInMsDirectory()
        {
            var globPattern = @"**/*.config";

            var root = new DirectoryInfo(@"C:\Windows\Microsoft.NET");
            var result = root.GlobFiles(globPattern).ToList();

            Assert.NotNull(result);
            Assert.True(result.Any(x => x.Name == "machine.config"), $"There should be some machine.config files in '{root.FullName}'");
        }

        [Fact]
        public void CanMatchConfigFilesOrFoldersInMsDirectory()
        {
            var globPattern = @"**/*[Cc]onfig";

            var root = new DirectoryInfo(@"C:\Windows\Microsoft.NET");
            var result = root.GlobFileSystemInfos(globPattern).ToList();

            Assert.NotNull(result);
            Assert.True(result.Any(x => x.Name == "security.config"), $"There should be some security.config files in '{root.FullName}'");
            Assert.True(result.Any(x => x.Name == "machine.config"), $"There should some folder with 'Config' in '{root.FullName}'");
        }

        [Fact]
        public void CanMatchDirectoriesInMsDirectory()
        {
            var globPattern = @"**/*v*.0*";

            var root = new DirectoryInfo(@"C:\Windows\Microsoft.NET");
            var result = root.GlobDirectories(globPattern).ToList();

            Assert.NotNull(result);
            Assert.True(result.Any(), $"There should be some directories that match glob: {globPattern} in '{root.FullName}'");
        }
    }
}
