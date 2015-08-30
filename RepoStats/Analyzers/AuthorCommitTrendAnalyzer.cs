
namespace RepoStats.Analyzers
{
    using LibGit2Sharp;

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    class AuthorCommitTrendAnalyzer : FileChangeAnalyzer
    {
        class SignatureComparer : IEqualityComparer<Signature>
        {
            public bool Equals(Signature x, Signature y)
            {
                return (x.Email == y.Email && x.Name == y.Name);
            }

            public int GetHashCode(Signature obj)
            {
                return obj.Email.GetHashCode() + obj.Name.GetHashCode();
            }
        }
        Dictionary<Signature, Dictionary<DateTime, long>> commitCountByDate = new Dictionary<Signature, Dictionary<DateTime, long>>(new SignatureComparer());
        long totalCommitCount = 0;

        public void Visit(Commit commit)
        {
            DateTime commitDate = commit.Author.When.DateTime.Round(TimeSpan.FromDays(1));
            Dictionary<DateTime, long> dateEntry = null;
            if (!commitCountByDate.TryGetValue(commit.Author, out dateEntry))
            {
                dateEntry = new Dictionary<DateTime, long>();
                commitCountByDate.Add(commit.Author, dateEntry);
            }
            if (!dateEntry.ContainsKey(commitDate))
            {
                dateEntry[commitDate] = 0;
            }
            dateEntry[commitDate] += 1;
            totalCommitCount++;
        }

        public void Visit(Commit commit, FileChanges fileChanges)
        {
        }

        public void Write()
        {
        }

        public string GetFormattedString()
        {
            StringBuilder returnString = new StringBuilder();
            returnString.Append(@"
             <table>
                <tr class='contrib-title-row'>
                    <td class='contrib-title-box'>
                        <h1>Top committers</h1>
                    </td>
                </tr>
                <tr>
                    <td>");
            int count = 0;
            string contributionLinkTemplate = "<br><a href='{0}'>{1} ({2})</a>";
            foreach (KeyValuePair<Signature, Dictionary<DateTime, long>> entry in this.commitCountByDate.OrderByDescending(c => c.Value.Values.Aggregate((a,b) => a + b)))
            {
                string directoryName = "commitsByAuthor\\" + entry.Key.Email + "\\";
                string fileName = directoryName  + "Contributions.html";
                
                Directory.CreateDirectory(directoryName);
                File.WriteAllText(fileName,
                    String.Concat(
                        HtmlTemplates.HtmlPreTemplate,
                        GetContributionData(entry.Value, entry.Key.Email),
                        HtmlTemplates.HtmlPostTemplate));

                if (count++ < 20)
                {
                    long commitCount = entry.Value.Values.Aggregate((a, b) => a + b);
                    returnString.AppendFormat(contributionLinkTemplate, fileName, entry.Key.Name, commitCount);
                }

            }

            returnString.Append(@"
                    </td>
                </tr>
            </table>");
            return returnString.ToString();
        }

        private object GetContributionData(Dictionary<DateTime, long> entry, string author)
        {

            StringBuilder svgCells = new StringBuilder();
            DateTime oneYearAgo = DateTime.Now.Subtract(TimeSpan.FromDays(365));
            int currentDateOffset = 0;

            while (currentDateOffset++ < 365)
            {
                DateTime currentDate = oneYearAgo.Add(TimeSpan.FromDays(currentDateOffset)).Round(TimeSpan.FromDays(1));
                long currentCommitCount = 0;
                entry.TryGetValue(currentDate, out currentCommitCount);
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
                String.Format(HtmlTemplates.SvgContribution.SVGTemplatePre.EscapeForFormat(), "."),
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
