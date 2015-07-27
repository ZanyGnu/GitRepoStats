
namespace RepoStats
{
    using Analyzers;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    class OutputWriter
    {
        public static void OutputCheckinDetails(
            Dictionary<string, FileInfoAnalyzer.GitFileInfo> gitFileInfos, 
            Dictionary<string, CommitterInfoAnalyzer.GitCommitterInfo> gitComitterInfos)
        {
            string fileInfoChangesTableString = HtmlTemplates.Table.TableTemplate;
            StringBuilder trFileInfosContent = new StringBuilder();

            foreach (FileInfoAnalyzer.GitFileInfo fileInfo in gitFileInfos.Values.OrderByDescending(c => (c.LinesDeleted + c.LinesAdded)).Take(20))
            {
                trFileInfosContent.AppendFormat(
                    HtmlTemplates.Table.trTemplate, 
                    fileInfo.LinesAdded, 
                    fileInfo.LinesDeleted, 
                    fileInfo.NumberOfCommits,
                    fileInfo.Path);
            }

            fileInfoChangesTableString = String.Format(
                fileInfoChangesTableString
                    .Replace("{0}", "%0%")
                    .Replace("{1}", "%1%")
                    .Replace("{", "{{")
                    .Replace("}", "}}")
                    .Replace("%0%", "{0}")
                    .Replace("%1%", "{1}"), trFileInfosContent);


            string committerInfoTableString = HtmlTemplates.Table.TableTemplate;
            StringBuilder trCommitterInfosContent = new StringBuilder();

            foreach (CommitterInfoAnalyzer.GitCommitterInfo committerInfo in gitComitterInfos.Values.OrderByDescending(c => (c.LinesDeleted + c.LinesAdded)).Take(20))
            {
                trCommitterInfosContent.AppendFormat(
                    HtmlTemplates.Table.trTemplate,
                    committerInfo.LinesAdded,
                    committerInfo.LinesDeleted,
                    committerInfo.NumberOfCommits,
                    committerInfo.Author);
            }

            committerInfoTableString = String.Format(
                committerInfoTableString
                    .Replace("{0}", "%0%")
                    .Replace("{1}", "%1%")
                    .Replace("{", "{{")
                    .Replace("}", "}}")
                    .Replace("%0%", "{0}")
                    .Replace("%1%", "{1}"), trCommitterInfosContent);
            
            File.WriteAllText("report.html",
                String.Concat(
                    htmlPreTemplate,
                    fileInfoChangesTableString,
                    tableFillerTemplate,
                    committerInfoTableString,
                    htmlPostTemplate));
        }


        private static string htmlPreTemplate = @"<html>

          <html>

            <head>
            <meta http-equiv=Content-Type content='text/html; charset=windows-1252'>
            <meta name=Generator content='Microsoft Word 15 (filtered)'>
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
	            font-size:11.0pt;
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

            <p class=MsoNormal>
            <h1>Repo report</h1>
            <br>
            </p>

            ";

        private static string htmlPostTemplate = @"
            <br><br><hr><br>
            </div>

            </body>

            </html>";

        public static string tableFillerTemplate = @"
        
            <p class=MsoNormal>&nbsp;</p>
            <br><br><br><hr><br><br>
            <p class=MsoNormal><i>Content</i></p>

            <p class=MsoNormal>&nbsp;</p>

            <p class=MsoNormal>&nbsp;</p>

            <p class=MsoNormal>&nbsp;</p>

            <p class=MsoNormal>&nbsp;</p>";
    }
}
