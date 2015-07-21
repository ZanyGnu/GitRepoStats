
namespace RepoStats
{
    using LibGit2Sharp;
    using System.Collections.Generic;
    using System.Linq;

    public class CommitIterator
    {
        List<CommitAnalyzer> commitAnalysis;
        List<PatchAnalyzer> patchAnalysis;
        string repoRoot;

        public CommitIterator(string repoRoot, List<CommitAnalyzer> commitAnalysis, List<PatchAnalyzer> patchAnalysis)
        {
            this.commitAnalysis = commitAnalysis;
            this.patchAnalysis = patchAnalysis;
            this.repoRoot = repoRoot;

        }

        public void Iterate()
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

        public void WriteOutput()
        {
            if (commitAnalysis != null)
            {
                foreach (CommitAnalyzer ca in commitAnalysis)
                {
                    ca.Write();
                }
            }

            if (patchAnalysis != null)
            {
                foreach (PatchAnalyzer pa in patchAnalysis)
                {
                    pa.Write();
                }
            }
        }

        private static void ExecuteCommitAnalysis(Commit c, List<CommitAnalyzer> commitAnalysis, List<PatchAnalyzer> patchAnalysis)
        {
            if (commitAnalysis != null)
            {
                foreach (CommitAnalyzer ca in commitAnalysis)
                {
                    ca.Visit(c);
                }
            }

            if (patchAnalysis != null)
            {
                foreach (PatchAnalyzer pa in patchAnalysis)
                {
                    pa.Visit(c);
                }
            }
        }

        private static void ExecutePatchAnalysis(List<PatchAnalyzer> patchAnalysis, Commit c, Patch changes)
        {
            if (patchAnalysis != null)
            {
                foreach (PatchEntryChanges patchEntryChanges in changes)
                {
                    foreach (PatchAnalyzer pa in patchAnalysis)
                    {
                        pa.Visit(c, patchEntryChanges);
                    }
                }
            }
        }
    }
}
