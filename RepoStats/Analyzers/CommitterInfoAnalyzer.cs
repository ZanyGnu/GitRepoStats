using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoStats.Analyzers
{
    public class CommitterInfoAnalyzer : PatchAnalyzer
    {
        public class GitCommitterInfo
        {
            public string Author { get; set; }
            public int NumberOfCommits { get; set; }
            public int LinesAdded { get; set; }
            public int LinesDeleted { get; set; }
        }

        public Dictionary<string, GitCommitterInfo> GitComitterInfos = new Dictionary<string, GitCommitterInfo>();

        DateTime startDate;
        DateTime endDate;

        public CommitterInfoAnalyzer(DateTime startDate, DateTime endDate)
        {
            this.startDate = startDate;
            this.endDate = endDate;
        }

        public void Visit(Commit c)
        {
            if (!GitComitterInfos.ContainsKey(c.Committer.Name))
            {
                GitComitterInfos.Add(c.Committer.Name, new GitCommitterInfo()
                {
                    Author = c.Committer.Name,
                    LinesAdded = 0,
                    LinesDeleted = 0,
                    NumberOfCommits = 0
                });
            }

            GitComitterInfos[c.Committer.Name].NumberOfCommits++;
        }

        public void Visit(Commit commit, PatchEntryChanges patchEntryChanges)
        {
            GitComitterInfos[commit.Committer.Name].LinesAdded += patchEntryChanges.LinesAdded;
            GitComitterInfos[commit.Committer.Name].LinesDeleted += patchEntryChanges.LinesDeleted;
        }

        public void Write()
        {

            Console.WriteLine("Committers ordered by number of modifications");
            IOrderedEnumerable<CommitterInfoAnalyzer.GitCommitterInfo> orderedChangesByCommitters = GitComitterInfos.Values.OrderByDescending(c => c.LinesDeleted + c.LinesAdded);
            foreach (CommitterInfoAnalyzer.GitCommitterInfo committerInfo in orderedChangesByCommitters.Take(20))
            {
                Console.WriteLine("\t{0} {1} {2}", committerInfo.Author, committerInfo.LinesAdded, committerInfo.LinesDeleted);
            }

            Console.WriteLine("Committers ordered by number of commit touches");
            orderedChangesByCommitters = GitComitterInfos.Values.OrderByDescending(c => c.NumberOfCommits);
            foreach (CommitterInfoAnalyzer.GitCommitterInfo committerInfo in orderedChangesByCommitters.Take(20))
            {
                Console.WriteLine("\t{0} {1}", committerInfo.Author, committerInfo.NumberOfCommits);
            }
        }
    }
}
