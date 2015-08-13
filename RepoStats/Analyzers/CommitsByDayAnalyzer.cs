using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoStats.Analyzers
{
    public class CommitsByDayAnalyzer : FileChangeAnalyzer
    {
        public class CommitDetails
        {
            public string Id { get; set; }
            public string Author { get; set; }
            public string Message { get; set; }
            public DateTime CommitTime { get; set; }
        }

        Dictionary<DateTime, List<CommitDetails>> commitsByDate = new Dictionary<DateTime, List<CommitDetails>>();

        public void Visit(Commit commit)
        {
            DateTime commitDate = commit.Committer.When.DateTime.Round(TimeSpan.FromDays(1));
            if (!commitsByDate.ContainsKey(commitDate))
            {
                commitsByDate[commitDate] = new List<CommitDetails>();
            }
            commitsByDate[commitDate].Add(new CommitDetails()
            {
                Id = commit.Id.Sha.Substring(0, 7),
                Author = commit.Author.Email,
                Message = commit.Message,
                CommitTime = commit.Committer.When.DateTime
            });
        }

        public void Visit(Commit commit, FileChanges fileChanges)
        {
        }

        public void Write()
        {
            string directoryName = "commitsByDate";
            Directory.CreateDirectory(directoryName);

            foreach (KeyValuePair<DateTime, List<CommitDetails>> entry in this.commitsByDate)
            {
                string dateString = entry.Key.ToString("yyyy-MM-dd");
                string fileName = directoryName + "\\" + dateString + ".html";
                StringBuilder fileContents = new StringBuilder();
                //fileContents.AppendFormat("{0}<h1> Commits on {1} </h1><div width='500px'>", HtmlTemplates.HtmlPreTemplate, dateString);

                foreach (CommitDetails commit in entry.Value)
                {
                    /*fileContents.AppendFormat(
                        "<pre>{0} {1} {2} {3}</pre>",
                        commit.Id, commit.CommitTime, commit.Author, commit.Message);
                        */
                    fileContents.AppendFormat(
                        HtmlTemplates.CommitDetails.trTemplate,
                        commit.Id, commit.Author, 
                        commit.CommitTime.ToString("yy/MM/dd hh:mm:ss"), 
                        commit.Message);
                }
                //fileContents.AppendFormat("</div>{0}", HtmlTemplates.HtmlPostTemplate);
                File.WriteAllText(
                    fileName,
                    String.Format(HtmlTemplates.CommitDetails.htmlTemplate.Replace("{0}", "%0%").Replace("{1}", "%1%").Replace("{", "{{").Replace("}", "}}").Replace("%0%", "{0}").Replace("%1%", "{1}"),
                    fileContents.ToString()));
            }
        }

        public string GetFormattedString()
        {
            return string.Empty;
        }
    }
}
