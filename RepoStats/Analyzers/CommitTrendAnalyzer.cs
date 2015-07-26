
namespace RepoStats.Analyzers
{
    using System;
    using System.Collections.Generic;
    using LibGit2Sharp;
    using System.Linq;

    class CommitTrendAnalyzer : PatchAnalyzer
    {
        Dictionary<DateTime, long> commitCountByDate = new Dictionary<DateTime, long>();
        long totalCommitCount = 0;

        public void Visit(Commit commit)
        {
            DateTime commitDate = commit.Committer.When.DateTime.Round(TimeSpan.FromDays(1));
            if (!commitCountByDate.ContainsKey(commitDate))
            {
                commitCountByDate[commitDate] = 0;
            }
            commitCountByDate[commitDate] += 1;
        }

        public void Visit(Commit commit, PatchEntryChanges patchEntryChanges)
        {
        }

        public void Write()
        {
            Console.WriteLine("Commit count by date");
            IOrderedEnumerable<KeyValuePair<DateTime,long>> orderedCommitCounts = commitCountByDate.OrderBy(c => c.Key);
            long currentTotalCount = 0;
            foreach (KeyValuePair<DateTime, long> commitCountOnDate in orderedCommitCounts)
            {
                currentTotalCount += commitCountOnDate.Value;
                Console.WriteLine("\t{0} |{1}", 
                    commitCountOnDate.Key.ToString("dd/MM/yy"), 
                    new String('*', (int)((currentTotalCount * 100)/ totalCommitCount)));
            }
        }

        public string GetFormattedString()
        {
            return HtmlTemplates.SvgContribution.SVGTemplatePre;
        }
    }
}
