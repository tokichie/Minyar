using NUnit.Framework;
using Minyar.Github;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minyar.Database;

namespace Minyar.Github.Tests {
    [TestFixture()]
    public class FileCacheTest {
        [Test()]
        public void FilePathTest() {
            Assert.Fail();
        }

        [Test()]
        public void FileExistsTest() {
            Assert.Fail();
        }

        [Test()]
        public void FileExistsInDatabaseTest() {
            Assert.That(FileCache.FileExistsInDatabase("facebook", "rebound", "6a9407d87512aaf8bb8d93c46cbddb27fa76873a",
                "src/com/facebook/rebound/SpringSystem.java"));
        }

        [Test()]
        public void LoadFileTest() {
            Assert.Fail();
        }

        [Test()]
        public void LoadContentTest() {
            Assert.Fail();
        }

        [Test()]
        public void LoadContentFromDatabaseTest() {
            Assert.Fail();
        }

        [Test()]
        public void SaveFileTest() {
            Assert.Fail();
        }
    }
}