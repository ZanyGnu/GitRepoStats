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
        private static string trTemplate = @" <tr>
              <td valign=top style='width:50pt;border:solid #BDD6EE 1.0pt;
              border-top:none;padding:0in 5.4pt 0in 5.4pt'>
              <p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:
              normal'><b>{0}</b></p>
              </td>
              <td valign=top style='width:30pt;border-top:none;border-left:
              none;border-bottom:solid #BDD6EE 1.0pt;border-right:solid #BDD6EE 1.0pt;
              padding:0in 5.4pt 0in 5.4pt'>
              <p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:
              normal'>{1}</p>
              </td>
              <td valign=top style='width:100pt;border-top:none;border-left:
              none;border-bottom:solid #BDD6EE 1.0pt;border-right:solid #BDD6EE 1.0pt;
              padding:0in 5.4pt 0in 5.4pt'>
              <p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:
              normal'>{2}</p>
              </td>
              <td valign=top style='width:500pt;border-top:none;border-left:
              none;border-bottom:solid #BDD6EE 1.0pt;border-right:solid #BDD6EE 1.0pt;
              padding:0in 5.4pt 0in 5.4pt'>
              <p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:
              normal'>{3}</p>
              </td>
             </tr>";

        private static string htmlTemplate = @"<html>

          <html>

            <head>
            <style>
            <!--
             /* Font Definitions */
             @font-face
	            {font-family:'Cambria Math';
	            panose-1:2 4 5 3 5 4 6 3 2 4;}
            @font-face
	            {font-family:'Calibri Light';
	            panose-1:2 15 3 2 2 2 4 3 2 4;}
            @font-face
	            {font-family:Calibri;
	            panose-1:2 15 5 2 2 2 4 3 2 4;}
             /* Style Definitions */
             p.MsoNormal, li.MsoNormal, div.MsoNormal
	            {margin-top:0in;
	            margin-right:0in;
	            margin-bottom:8.0pt;
	            margin-left:0in;
	            line-height:107%;
	            font-size:9.0pt;
	            font-family:'Calibri',sans-serif;}
            h1
	            {mso-style-link:'Heading 1 Char';
	            margin-top:12.0pt;
	            margin-right:0in;
	            margin-bottom:0in;
	            margin-left:0in;
	            margin-bottom:.0001pt;
	            line-height:107%;
	            page-break-after:avoid;
	            font-size:16.0pt;
	            font-family:'Calibri Light',sans-serif;
	            color:#2E74B5;
	            font-weight:normal;}
            span.Heading1Char
	            {mso-style-name:'Heading 1 Char';
	            mso-style-link:'Heading 1';
	            font-family:'Calibri Light',sans-serif;
	            color:#2E74B5;}
            .MsoChpDefault
	            {font-family:'Calibri',sans-serif;}
            .MsoPapDefault
	            {margin-bottom:8.0pt;
	            line-height:107%;}
            @page WordSection1
	            {size:8.5in 11.0in;
	            margin:1.0in 1.0in 1.0in 1.0in;}
            div.WordSection1
	            {page:WordSection1;}
            -->
            </style>

            </head>

            <div class=WordSection1>

            <table class=MsoTable15Grid4Accent2 border=1 cellspacing=0 cellpadding=0
             style='border-collapse:collapse;border:none'>
             <tr style='height:12.75pt'>
              <td valign=top style='width:50pt;border:solid #ED7D31 1.0pt;
              border-right:none;background:#C00000;padding:0in 5.4pt 0in 5.4pt;height:12.75pt'>
              <p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:
              normal'><b><span style='color:white'>CL</span></b></p>
              </td>
              <td valign=top style='width:30pt;border-top:solid #ED7D31 1.0pt;
              border-left:none;border-bottom:solid #ED7D31 1.0pt;border-right:none;
              background:#C00000;padding:0in 5.4pt 0in 5.4pt;height:12.75pt'>
              <p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:
              normal'><b><span style='color:white'>User</span></b></p>
              </td>
              <td valign=top style='width:100pt;border-top:solid #ED7D31 1.0pt;
              border-left:none;border-bottom:solid #ED7D31 1.0pt;border-right:none;
              background:#C00000;padding:0in 5.4pt 0in 5.4pt;height:12.75pt'>
              <p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:
              normal'><b><span style='color:white'>Checkin Time</span></b></p>
              </td>
              <td valign=top style='width:500pt;border:solid #ED7D31 1.0pt;
              border-left:none;background:#C00000;padding:0in 5.4pt 0in 5.4pt;height:12.75pt'>
              <p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:
              normal'><b><span style='color:white'>Description</span></b></p>
              </td>
             </tr>
             {0}
            </table>
            </div>
            </body>
            </html>";

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
                        trTemplate,
                        commit.Id, commit.Author, 
                        commit.CommitTime.ToString("yy/MM/dd hh:mm:ss"), 
                        commit.Message);
                }
                //fileContents.AppendFormat("</div>{0}", HtmlTemplates.HtmlPostTemplate);
                File.WriteAllText(
                    fileName,
                    String.Format(htmlTemplate.Replace("{0}", "%0%").Replace("{1}", "%1%").Replace("{", "{{").Replace("}", "}}").Replace("%0%", "{0}").Replace("%1%", "{1}"),
                    fileContents.ToString()));
            }
        }

        public string GetFormattedString()
        {
            return string.Empty;
        }
    }
}
