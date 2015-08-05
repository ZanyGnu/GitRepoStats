﻿
namespace RepoStats
{
    using LibGit2Sharp;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;

    public class CommitIterator
    {
        public class FileChanges
        {
            public string Path;
            public int LinesAdded;
            public int LinesRemoved;
        }

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
                int commitCount = repo.Commits.Count();
                int currentCommitCount = 0;

                foreach (Commit c in repo.Commits)
                {
                    currentCommitCount++;
                    Console.Write("\rProcessing {0}/{1} ({2}%)    ", currentCommitCount, commitCount, currentCommitCount * 100 / commitCount);

                    ExecuteCommitAnalysis(c, commitAnalysis, patchAnalysis);

                    if (c.Parents.Count() == 0)
                    {
                        continue;
                    }

                    List<FileChanges> fileChanges = new List<FileChanges>();

                    // TODO: need to handle multiple parents.
                    Patch changes = null;
                    changes = repo.Diff.Compare<Patch>(c.Tree, c.Parents.First().Tree);

                    foreach(var patchChanges in changes)
                    {
                        FileChanges change = new FileChanges()
                        {
                            LinesAdded = patchChanges.LinesAdded,
                            LinesRemoved = patchChanges.LinesDeleted,
                            Path = patchChanges.Path
                        };

                        fileChanges.Add(change);
                    }

                    Directory.CreateDirectory("patches");
                    XmlSerializer s = new XmlSerializer(typeof(List<FileChanges>));
                    s.Serialize(new FileStream("patches/" + c.Id, FileMode.OpenOrCreate), fileChanges);

                    ExecutePatchAnalysis(patchAnalysis, c, changes);
                }
            }
        }

        public void WriteOutput()
        {
            string htmlData = "";
            if (commitAnalysis != null)
            {
                foreach (CommitAnalyzer ca in commitAnalysis)
                {
                    ca.Write();
                    htmlData += ca.GetFormattedString();
                }
            }

            if (patchAnalysis != null)
            {
                foreach (PatchAnalyzer pa in patchAnalysis)
                {
                    pa.Write();
                    htmlData += pa.GetFormattedString();
                }
            }

            File.WriteAllText("report.html",
                String.Concat(
                    HtmlTemplates.HtmlPreTemplate,
                    htmlData,
                    HtmlTemplates.HtmlPostTemplate));
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
            if (patchAnalysis != null && changes != null)
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
