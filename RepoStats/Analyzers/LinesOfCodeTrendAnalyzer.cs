
namespace RepoStats.Analyzers
{
    using LibGit2Sharp;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    class LinesOfCodeTrendAnalyzer: FileChangeAnalyzer
    {
        Dictionary<DateTime, long> lineCountByDate = new Dictionary<DateTime, long>();
        long totalLineCount = 0;

        public void Visit(Commit commit)
        {
        }

        public void Visit(Commit commit, FileChanges fileChanges)
        {
            DateTime commitDate = commit.Committer.When.DateTime.Round(TimeSpan.FromDays(1));
            if (!lineCountByDate.ContainsKey(commitDate))
            {
                lineCountByDate[commitDate] = 0;
            }

            lineCountByDate[commitDate] += fileChanges.LinesAdded;
            totalLineCount += fileChanges.LinesAdded ;
        }

        public void Write()
        {
            Console.WriteLine("Line count by date");
            IOrderedEnumerable<KeyValuePair<DateTime, long>> orderedLineCounts = lineCountByDate.OrderBy(c => c.Key);
            long currentTotalCount = 0;
            foreach (KeyValuePair<DateTime, long> lineCountOnDate in orderedLineCounts)
            {
                currentTotalCount += lineCountOnDate.Value;
                Console.WriteLine("\t{0} |{1}",
                    lineCountOnDate.Key.ToString("dd/MM/yy"),
                    new String('*', (int)((currentTotalCount * 100) / totalLineCount)));
            }
        }

        public string GetFormattedString()
        {
            /*
            categories: [{ 0}]
                categories: [
                'Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun',
                'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'
            ]
            
            // Series:
            {
                name: 'London',
                data: [3.9, 4.2, 5.7, 8.5, 11.9, 15.2, 17.0, 16.6, 14.2, 10.3, 6.6, 4.8]
            },

    */

            StringBuilder categoryString = new StringBuilder();
            StringBuilder seriesDataString = new StringBuilder();
            IOrderedEnumerable<KeyValuePair<DateTime, long>> orderedLineCounts = lineCountByDate.OrderBy(c => c.Key);
            long currentTotalCount = 0;
            foreach (KeyValuePair<DateTime, long> lineCountOnDate in orderedLineCounts)
            {
                currentTotalCount += lineCountOnDate.Value;

                categoryString.AppendFormat("'{0}', ", lineCountOnDate.Key.ToString("dd/MM/yy"));
                seriesDataString.AppendFormat("{0}, ", currentTotalCount);
            }

            string seriesString = String.Format(
                HtmlTemplates.Graph.SeriesTemplate, 
                "Lines Of Code", 
                seriesDataString.ToString());

            return HtmlTemplates.Graph.ContainerTempalte + String.Format(
                HtmlTemplates.Graph.GraphTemplate, 
                categoryString.ToString(),
                seriesString);
        }
    }
}