
namespace RepoStats
{
    using Analyzers;
    using LibGit2Sharp;

    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.Linq;

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

                patchAnalyzers.Add(new FileInfoAnalyzer(DateTime.Now.Subtract(TimeSpan.FromDays(30)), DateTime.Now));
                patchAnalyzers.Add(new CommitterInfoAnalyzer(DateTime.Now.Subtract(TimeSpan.FromDays(30)), DateTime.Now));
                patchAnalyzers.Add(new CommitTrendAnalyzer());
                patchAnalyzers.Add(new LinesOfCodeTrendAnalyzer());

                CommitIterator iterator = new CommitIterator(repoRoot, null, patchAnalyzers);
                iterator.Iterate();
                iterator.WriteOutput();
            }
        }
    }
}
