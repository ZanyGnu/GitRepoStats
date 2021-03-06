﻿
namespace RepoStats.Analyzers
{
    using LibGit2Sharp;

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    class CommitTrendAnalyzer : FileChangeAnalyzer
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

        public void Visit(Commit commit, FileChanges fileChanges)
        {
        }

        public void Write()
        {
            File.WriteAllText("RepoContributionMap.html", GetFormattedString());
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
                    14 * (currentDateOffset / 7),
                    14 * (currentDateOffset % 7),
                    CommitCountToColor(currentCommitCount),
                    currentCommitCount,
                    currentDate.ToString("yyyy-MM-dd")
                    );
            }

            return String.Concat(
                String.Format(HtmlTemplates.SvgContribution.SVGTemplatePre.EscapeForFormat(), "commitsByDate/"),
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
