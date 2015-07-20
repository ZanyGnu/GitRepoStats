using LibGit2Sharp;
using RepoStats.Analyzers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoStats
{
    public class GitFileInfo
    {
        public string Path { get; set; }
        public int NumberOfCommits { get; set; }
        public int LinesAdded { get; set; }
        public int LinesDeleted { get; set; }
    }

    public class GitCommitterInfo
    {
        public string Author { get; set; }
        public int NumberOfCommits { get; set; }
        public int LinesAdded { get; set; }
        public int LinesDeleted { get; set; }
    }

    public class Program
    {
        
        static void Main(string[] args)
        {
            string repoRoot = string.Empty;

            if (args != null && args.Length > 0)
            {
                repoRoot = args[0];
            }
            else
            {
                repoRoot = ConfigurationManager.AppSettings["RepoRoot"];
            }
            
            using (var repo = new Repository(repoRoot))
            {
                var RFC2822Format = "ddd dd MMM HH:mm:ss yyyy K";

                int commitCount = repo.Commits.Count();
                foreach (Commit c in repo.Commits.Take(15))
                {
                    Console.WriteLine(string.Format("commit {0}", c.Id));

                    if (c.Parents.Count() > 1)
                    {
                        Console.WriteLine("Merge: {0}",
                            string.Join(" ", c.Parents.Select(p => p.Id.Sha.Substring(0, 7)).ToArray()));
                    }

                    Console.WriteLine(string.Format("Author: {0} <{1}>", c.Author.Name, c.Author.Email));
                    Console.WriteLine("Date:   {0}", c.Author.When.ToString(RFC2822Format, CultureInfo.InvariantCulture));
                    Console.WriteLine();
                    Console.WriteLine(c.Message);
                    Console.WriteLine();
                }

                //FindHotFilesAndCommitters(repoRoot, DateTime.Now.Subtract(TimeSpan.FromDays(30)), DateTime.Now);

                List<PatchAnalyzer> patchAnalyzers = new List<PatchAnalyzer>();
                FileInfoAnalyzer fileInfoAnalyzer = new FileInfoAnalyzer(DateTime.Now.Subtract(TimeSpan.FromDays(30)), DateTime.Now);
                patchAnalyzers.Add(fileInfoAnalyzer);
                CommitIterator iterator = new CommitIterator(repoRoot, null, patchAnalyzers);
                iterator.Iterate();

                Console.WriteLine("Files ordered by number of modifications");
                IOrderedEnumerable<GitFileInfo> orderedChanges = fileInfoAnalyzer.GitFileInfos.Values.OrderByDescending(c => c.LinesDeleted + c.LinesAdded);
                foreach (GitFileInfo fileInfo in orderedChanges.Take(20))
                {
                    Console.WriteLine("\t{0} {1} {2}", fileInfo.Path, fileInfo.LinesAdded, fileInfo.LinesDeleted);
                }

                Console.WriteLine("Files ordered by number of commit touches");
                orderedChanges = fileInfoAnalyzer.GitFileInfos.Values.OrderByDescending(c => c.NumberOfCommits);
                foreach (GitFileInfo fileInfo in orderedChanges.Take(20))
                {
                    Console.WriteLine("\t{0} {1}", fileInfo.Path, fileInfo.NumberOfCommits);
                }
            }
        }

        static void FindHotFilesAndCommitters(string repoRoot, DateTime startDate, DateTime endDate)
        {
            Dictionary<string, GitFileInfo> gitFileInfos = new Dictionary<string, GitFileInfo>();
            Dictionary<string, GitCommitterInfo> gitComitterInfos = new Dictionary<string, GitCommitterInfo>();

            using (var repo = new Repository(repoRoot))
            {
                int commitCount = repo.Commits.Count();
                int currentCommitCount = 0;
                foreach (Commit c in repo.Commits)
                {
                    currentCommitCount++;
                    Console.Write("\rProcessing {0}/{1} ({2}%)    ", currentCommitCount, commitCount, currentCommitCount * 100 / commitCount);
                    
                    if (!c.Author.When.IsWithin(startDate, endDate))
                    {
                        continue;
                    }

                    // for each commit, look at the files modified.
                    if (c.Parents.Count() == 0)
                    {
                        continue;
                    }

                    if (!gitComitterInfos.ContainsKey(c.Committer.Name))
                    {
                        gitComitterInfos.Add(c.Committer.Name, new GitCommitterInfo()
                        {
                            Author = c.Committer.Name,
                            LinesAdded = 0,
                            LinesDeleted = 0,
                            NumberOfCommits = 0
                        });
                    }

                    gitComitterInfos[c.Committer.Name].NumberOfCommits++;

                    // TODO: need to handle multiple parents.
                    Patch changes = repo.Diff.Compare<Patch>(c.Tree, c.Parents.First().Tree);

                    foreach (PatchEntryChanges patchEntryChanges in changes)
                    {
                        //Console.WriteLine("Path:{0} +{1} -{2} ",
                        //        patchEntryChanges.Path,
                        //        patchEntryChanges.LinesAdded,
                        //        patchEntryChanges.LinesDeleted
                        //    );

                        // Update File details
                        if (!gitFileInfos.ContainsKey(patchEntryChanges.Path))
                        {
                            gitFileInfos.Add(
                                patchEntryChanges.Path,
                                new GitFileInfo()
                                {
                                    Path = patchEntryChanges.Path,
                                    LinesAdded = 0,
                                    LinesDeleted = 0,
                                    NumberOfCommits = 0
                                });
                        }

                        gitFileInfos[patchEntryChanges.Path].LinesAdded += patchEntryChanges.LinesAdded;
                        gitFileInfos[patchEntryChanges.Path].LinesDeleted += patchEntryChanges.LinesDeleted;
                        gitFileInfos[patchEntryChanges.Path].NumberOfCommits ++;

                        // Update committer data

                        gitComitterInfos[c.Committer.Name].LinesAdded += patchEntryChanges.LinesAdded;
                        gitComitterInfos[c.Committer.Name].LinesDeleted += patchEntryChanges.LinesDeleted;
                    }
                }
            }

            Console.WriteLine("Files ordered by number of modifications");
            IOrderedEnumerable<GitFileInfo> orderedChanges = gitFileInfos.Values.OrderByDescending(c => c.LinesDeleted + c.LinesAdded);
            foreach(GitFileInfo fileInfo in orderedChanges.Take(20))
            {
                Console.WriteLine("\t{0} {1} {2}", fileInfo.Path, fileInfo.LinesAdded, fileInfo.LinesDeleted);
            }

            Console.WriteLine("Files ordered by number of commit touches");
            orderedChanges = gitFileInfos.Values.OrderByDescending(c => c.NumberOfCommits);
            foreach (GitFileInfo fileInfo in orderedChanges.Take(20))
            {
                Console.WriteLine("\t{0} {1}", fileInfo.Path, fileInfo.NumberOfCommits);
            }

            Console.WriteLine("Committers ordered by number of modifications");
            IOrderedEnumerable<GitCommitterInfo> orderedChangesByCommitters = gitComitterInfos.Values.OrderByDescending(c => c.LinesDeleted + c.LinesAdded);
            foreach (GitCommitterInfo committerInfo in orderedChangesByCommitters.Take(20))
            {
                Console.WriteLine("\t{0} {1} {2}", committerInfo.Author, committerInfo.LinesAdded, committerInfo.LinesDeleted);
            }

            Console.WriteLine("Committers ordered by number of commit touches");
            orderedChangesByCommitters = gitComitterInfos.Values.OrderByDescending(c => c.NumberOfCommits);
            foreach (GitCommitterInfo committerInfo in orderedChangesByCommitters.Take(20))
            {
                Console.WriteLine("\t{0} {1}", committerInfo.Author, committerInfo.NumberOfCommits);
            }

            OutputWriter.OutputCheckinDetails(gitFileInfos, gitComitterInfos);
        }
    }

    public static class Extensions
    {
        public static bool IsWithin(this DateTimeOffset obj, DateTime startDate, DateTime endDate)
        {
            return startDate <= obj && obj <= endDate;
        }
    }
}
