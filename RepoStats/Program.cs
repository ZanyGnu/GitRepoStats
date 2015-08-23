
namespace RepoStats
{
    using Analyzers;
    using LibGit2Sharp;
    using Microsoft.Owin.Hosting;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Xml;

    public class Program
    {        
        static void Main(string[] args)
        {
            string url = "http://localhost:12345/";
            using (WebApp.Start<Startup>(url))
            {
                url = url + "report.html";
                Process.Start(url); // Launch the browser.
                Console.WriteLine("Repo stats launched at {0}", url);
                Console.WriteLine("Press Enter to exit...");
                Console.ReadLine();
            }

            string repoRoot = string.Empty;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            if (args != null && args.Length > 0)
            {
                repoRoot = args[0];
            }
            else
            {
                repoRoot = ConfigurationManager.AppSettings["RepoRoot"];
            }

            string repoName = GetRepoName(repoRoot);

            using (var repo = new Repository(repoRoot))
            {
                //repo.Info
                List<FileChangeAnalyzer> patchAnalyzers = new List<FileChangeAnalyzer>();

                patchAnalyzers.Add(new CommitTrendAnalyzer());
                patchAnalyzers.Add(new AuthorCommitTrendAnalyzer());
                patchAnalyzers.Add(new FileInfoAnalyzer(DateTime.Now.Subtract(TimeSpan.FromDays(30)), DateTime.Now));
                patchAnalyzers.Add(new CommitterInfoAnalyzer(DateTime.Now.Subtract(TimeSpan.FromDays(30)), DateTime.Now));
                patchAnalyzers.Add(new LinesOfCodeTrendAnalyzer());
                patchAnalyzers.Add(new CommitsByDayAnalyzer());
                patchAnalyzers.Add(new CommitsByAuthorAnalyzer());

                CommitIterator iterator = new CommitIterator(repoRoot, repoName, null, patchAnalyzers);
                iterator.Iterate();
                iterator.WriteOutput();

                stopwatch.Stop();
                Console.WriteLine("Time To execute: {0}", stopwatch.Elapsed);
            }
        }

        private static string GetRepoName(string repoRoot)
        {
            string repoConfigPath = Path.Combine(repoRoot, "repo.config");
            if (File.Exists(repoConfigPath))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(File.ReadAllText(repoConfigPath));
                XmlNodeList objNode = xmlDoc.SelectNodes("/Repository/Name");
                if (objNode != null && objNode.Count > 0)
                {
                    return objNode[0].Name;
                }
            }

            return Path.GetDirectoryName(repoRoot);            
        }
    }
}
