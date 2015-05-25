using Minyar.Git;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minyar.Tests.Git {
    [TestFixture]
    class GitRepositoryTest {
        [Test]
        public void TestArchiveFiles() {
//            GitRepository.ArchiveFiles();
        }

        [Test]
        public void TestUpdateRepositories() {
            GitRepository.UpdateRepositories(null);
        }

        [Test]
        public void TestDownload() {
            var list = new List<string[]> { new string[] {"tokichie", "pattern-detection"} };
            GitRepository.DownloadRepositories(list);
        }

        [Test]
        public void TestGetDiff() {
            //var res = gitrepository.getdiff();
            //console.writeline(res);
        }
    }
}
