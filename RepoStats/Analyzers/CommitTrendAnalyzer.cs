﻿
namespace RepoStats.Analyzers
{
    using System;
    using System.Collections.Generic;
    using LibGit2Sharp;
    using System.Linq;

    class CommitTrendAnalyzer : PatchAnalyzer
    {
        Dictionary<DateTime, long> lineCountByDate = new Dictionary<DateTime, long>();
        long totalLineCount = 0;

        public void Visit(Commit c)
        {
            
        }

        public void Visit(Commit commit, PatchEntryChanges patchEntryChanges)
        {
            DateTime commitDate = commit.Committer.When.DateTime.Round(TimeSpan.FromDays(1));
            if (!lineCountByDate.ContainsKey(commitDate))
            {
                lineCountByDate[commitDate] = 0;
            }

            lineCountByDate[commitDate] += patchEntryChanges.LinesAdded - patchEntryChanges.LinesDeleted;
            totalLineCount += patchEntryChanges.LinesAdded - patchEntryChanges.LinesDeleted;

        }

        public void Write()
        {
            Console.WriteLine("Line count by date");
            IOrderedEnumerable<KeyValuePair<DateTime,long>> orderedLineCounts = lineCountByDate.OrderBy(c => c.Key);
            long currentTotalCount = 0;
            foreach (KeyValuePair<DateTime, long> lineCountOnDate in orderedLineCounts)
            {
                currentTotalCount += lineCountOnDate.Value;
                Console.WriteLine("\t{0} |{1}", 
                    lineCountOnDate.Key.ToString("dd/MM/yy"), 
                    new String('*', (int)((currentTotalCount * 100)/ totalLineCount)));
            }
        }
    }
}
