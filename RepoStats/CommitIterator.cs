using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoStats
{
    public class CommitIterator
    {
        List<CommitAnalysis> commitAnalysis;
        List<PatchAnalysis> patchAnalysis;
        string repoRoot;

        public CommitIterator(string repoRoot, List<CommitAnalysis> commitAnalysis, List<PatchAnalysis> patchAnalysis)
        {
            this.commitAnalysis = commitAnalysis;
            this.patchAnalysis = patchAnalysis;
            this.repoRoot = repoRoot;

        }

        void RunThroughCommits()
        {
            using (var repo = new Repository(repoRoot))
            {
                foreach (Commit c in repo.Commits)
                {
                    ExecuteCommitAnalysis(c, commitAnalysis, patchAnalysis);

                    if (c.Parents.Count() == 0)
                    {
                        continue;
                    }

                    // TODO: need to handle multiple parents.
                    Patch changes = repo.Diff.Compare<Patch>(c.Tree, c.Parents.First().Tree);

                    ExecutePatchAnalysis(patchAnalysis, c, changes);
                }
            }
        }

        private static void ExecuteCommitAnalysis(Commit c, List<CommitAnalysis> commitAnalysis, List<PatchAnalysis> patchAnalysis)
        {
            foreach (CommitAnalysis ca in commitAnalysis)
            {
                ca.Visit(c);
            }

            foreach (PatchAnalysis pa in patchAnalysis)
            {
                pa.Visit(c);
            }
        }

        private static void ExecutePatchAnalysis(List<PatchAnalysis> patchAnalysis, Commit c, Patch changes)
        {
            foreach (PatchEntryChanges patchEntryChanges in changes)
            {
                foreach (PatchAnalysis pa in patchAnalysis)
                {
                    pa.Visit(c, patchEntryChanges);
                }
            }
        }

        public interface CommitAnalysis
        {
            void Visit(Commit c);
        }
        public interface PatchAnalysis : CommitAnalysis
        {
            void Visit(Commit commit, PatchEntryChanges patch);
        }

        public class FileInfoAnalysis : PatchAnalysis
        {
            public void Visit(Commit c)
            {

            }

            public void Visit(Commit commit, PatchEntryChanges patch)
            {

            }
        }
    }
}
