
namespace RepoStats.Analyzers
{
    using LibGit2Sharp;

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    class LinesOfCodeTrendAnalyzer: FileChangeAnalyzer
    {
        Dictionary<string, Dictionary<string, long>> lineCountByExtension = new Dictionary<string, Dictionary<string, long>>();
        HashSet<string> categoryDates = new HashSet<string>();

        long totalLineCount = 0;

        public void Visit(Commit commit)
        {
        }

        public void Visit(Commit commit, FileChanges fileChanges)
        {
            string commitDate = commit.Committer.When.DateTime.ToString("dd/MM/yy");
            categoryDates.Add(commitDate);

            Dictionary<string, long> lineCountByDate = null;
            string extension = Path.GetExtension(fileChanges.Path);
            if (!lineCountByExtension.TryGetValue(extension, out lineCountByDate))
            {
                lineCountByDate = new Dictionary<string, long>();
                lineCountByExtension[extension] = lineCountByDate;
            }

            if (!lineCountByDate.ContainsKey(commitDate))
            {
                lineCountByDate[commitDate] = 0;
            }

            lineCountByDate[commitDate] += fileChanges.LinesAdded - fileChanges.LinesDeleted;
            totalLineCount += fileChanges.LinesAdded - fileChanges.LinesDeleted;
        }

        public void Write()
        {
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

            StringBuilder categoryString = new StringBuilder(categoryDates.Count);
            StringBuilder seriesString = new StringBuilder(categoryDates.Count);
            StringBuilder seriesDataString = new StringBuilder();
            var orderedLineCountByExtension = lineCountByExtension.OrderByDescending(c => c.Value.Sum(x => x.Value));

            foreach (KeyValuePair<string, Dictionary<string, long>> entry in orderedLineCountByExtension.Take(6))
            {
                PopulateSeriesData(seriesString, seriesDataString, entry.Key, entry.Value);
            }

            Dictionary<string, long> others = new Dictionary<string, long>();

            foreach (KeyValuePair<string, Dictionary<string, long>> entry in orderedLineCountByExtension.Skip(6))
            {
                foreach(KeyValuePair<string, long> x in entry.Value)
                {
                    if (!others.ContainsKey(x.Key))
                    {
                        others[x.Key] = 0;
                    }
                    others[x.Key] += x.Value;
                };
            }

            PopulateSeriesData(seriesString, seriesDataString, "Others", others);

            foreach (string date in categoryDates)
            {
                categoryString.AppendFormat("'{0}', ", date);
            }

            return HtmlTemplates.Graph.ContainerTempalte + 
                String.Format(
                    HtmlTemplates.Graph.GraphTemplate.EscapeForFormat(), 
                    categoryString.ToString(),
                    seriesString.ToString());
        }

        private void PopulateSeriesData(StringBuilder seriesString, StringBuilder seriesDataString, string seriesName, Dictionary<string, long> seriesValues)
        {
            seriesDataString.Clear();
            IOrderedEnumerable<KeyValuePair<string, long>> orderedLineCounts = seriesValues.OrderBy(c => c.Key);
            long currentTotalCount = 0;
            foreach (string date in categoryDates)
            {
                long lineCount = 0;
                if (seriesValues.ContainsKey(date))
                {
                    lineCount = seriesValues[date];
                }
                currentTotalCount += lineCount;

                seriesDataString.AppendFormat("{0}, ", currentTotalCount);
            }

            seriesString.AppendFormat(
                HtmlTemplates.Graph.SeriesTemplate.EscapeForFormat(),
                seriesName,
                seriesDataString.ToString());
        }
    }
}