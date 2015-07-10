using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoStats
{
    class Program
    {
        class GitFileInfo
        {
            public string Path { get; set; }
            public int NumberOfCommits { get; set; }
            public int LinesAdded { get; set; }
            public int LinesDeleted { get; set; }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello world");
            string repoRoot = "C:\\rdnext\\Azure\\Compute";
            
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

                // build change vector by file. 
                // note: Not efficient at all.
                //foreach(string file in Directory.GetFiles(repoRoot, "*", SearchOption.AllDirectories))
                //{
                //    string fileEntry = file.Substring(repoRoot.Length + 1);
                //    Console.WriteLine(fileEntry);
                //    foreach (LogEntry log in repo.Commits.QueryBy(fileEntry))
                //    {
                //        Console.WriteLine("\t " + log.Commit.Sha);
                //    }
                //}

                foreach (IndexEntry e in repo.Index)
                {
                    Console.WriteLine("{0} {1} {2}       {3}",
                        Convert.ToString((int)e.Mode, 8),
                        e.Id.ToString(), (int)e.StageLevel, e.Path);
                }
            }
        }
    }
}
