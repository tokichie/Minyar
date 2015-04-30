using Minyar.Git;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minyar.Tests.Git {
    [TestFixture]
    class RepositoryTest {
        [Test]
        public void TestArchiveFiles() {
            GitRepository.ArchiveFiles();
        }

        [Test]
        public void TestUpdateRepositories() {
            GitRepository.UpdateRepositories(null);
        }
    }
}
