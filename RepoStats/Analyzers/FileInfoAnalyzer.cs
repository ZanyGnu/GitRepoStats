using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoStats.Analyzers
{
    public class FileInfoAnalyzer : PatchAnalyzer
    {
        public class GitFileInfo
        {
            public string Path { get; set; }
            public int NumberOfCommits { get; set; }
            public int LinesAdded { get; set; }
            public int LinesDeleted { get; set; }
        }

        public Dictionary<string, GitFileInfo> GitFileInfos = new Dictionary<string, GitFileInfo>();

        DateTime startDate;
        DateTime endDate;

        public FileInfoAnalyzer(DateTime startDate, DateTime endDate)
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
