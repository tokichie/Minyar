﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octokit;

namespace Minyar.Github {
    class DiffPatcher {
        private PullRequestReviewComment comment;

        public DiffPatcher(PullRequestReviewComment prComment) {
            comment = prComment;
        }

        public void Patch() {
            
        }
    }
}