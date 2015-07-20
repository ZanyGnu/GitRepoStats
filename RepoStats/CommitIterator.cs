
namespace RepoStats
{
    using LibGit2Sharp;
    using System;
    using System.Collections.Generic;
    using System.Linq;

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

        private static void ExecuteCommitAnalysis(Commit c, List<CommitAnalysis> commitAnalysis, List<PatchAnalysis> patchAnalysis)
        {
            if (commitAnalysis != null)
            {
                foreach (CommitAnalysis ca in commitAnalysis)
                {
                    ca.Visit(c);
                }
            }

            if (patchAnalysis != null)
            {
                foreach (PatchAnalysis pa in patchAnalysis)
                {
                    pa.Visit(c);
                }
            }
        }

        private static void ExecutePatchAnalysis(List<PatchAnalysis> patchAnalysis, Commit c, Patch changes)
        {
            if (patchAnalysis != null)
            {
                foreach (PatchEntryChanges patchEntryChanges in changes)
                {
                    foreach (PatchAnalysis pa in patchAnalysis)
                    {
                        pa.Visit(c, patchEntryChanges);
                    }
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
            public Dictionary<string, GitFileInfo> GitFileInfos = new Dictionary<string, GitFileInfo>();

            DateTime startDate;
            DateTime endDate;

            public FileInfoAnalysis (DateTime startDate, DateTime endDate)
            {
                this.startDate = startDate;
                this.endDate = endDate;
            }

            public void Visit(Commit c)
            {
                
            }

            public void Visit(Commit commit, PatchEntryChanges patchEntryChanges)
            {
                if (!GitFileInfos.ContainsKey(patchEntryChanges.Path))
                {
                    GitFileInfos.Add(
                        patchEntryChanges.Path,
                        new GitFileInfo()
                        {
                            Path = patchEntryChanges.Path,
                            LinesAdded = 0,
                            LinesDeleted = 0,
                            NumberOfCommits = 0
                        });
                }

                GitFileInfos[patchEntryChanges.Path].LinesAdded += patchEntryChanges.LinesAdded;
                GitFileInfos[patchEntryChanges.Path].LinesDeleted += patchEntryChanges.LinesDeleted;
                GitFileInfos[patchEntryChanges.Path].NumberOfCommits++;
            }
        }
    }
}
