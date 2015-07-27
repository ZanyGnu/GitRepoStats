
namespace RepoStats.Analyzers
{
    using LibGit2Sharp;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

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
            totalCommitCount++;
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
            StringBuilder svgCells = new StringBuilder();
            DateTime oneYearAgo = DateTime.Now.Subtract(TimeSpan.FromDays(365));
            int currentDateOffset = 0;

            while(currentDateOffset++ < 365)
            {
                DateTime currentDate = oneYearAgo.Add(TimeSpan.FromDays(currentDateOffset)).Round(TimeSpan.FromDays(1));
                long currentCommitCount = 0;
                commitCountByDate.TryGetValue(currentDate, out currentCommitCount);
                //<g transform='translate({0}, 0)'>< rect class='day' width='11' height='11' y='{1}' fill='{2}' data-count='{3}' data-date='{4}'></rect></g>";
                svgCells.AppendFormat(
                    HtmlTemplates.SvgContribution.CellEntryTemplate,
                    13 * (currentDateOffset / 7),
                    13 * (currentDateOffset % 7),
                    CommitCountToColor(currentCommitCount),
                    currentCommitCount,
                    currentDate.ToString("yyyy-MM-dd")
                    );
            }

            return String.Concat(
                HtmlTemplates.SvgContribution.SVGTemplatePre,
                svgCells,
                HtmlTemplates.SvgContribution.SVGTemplatePost);
        }

        private string CommitCountToColor(long commitCount)
        {
            if (commitCount < 1)
                return "#eee";
            else if (commitCount < 3)
                return "#d6e685";
            else if (commitCount < 5)
                return "#8cc665";
            else if (commitCount < 8)
                return "#44a340";
            else return "#1e6823";
        }
    }
}
