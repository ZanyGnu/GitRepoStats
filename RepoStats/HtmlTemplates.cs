using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoStats
{
    public static class HtmlTemplates
    {
        public static string HtmlPreTemplate = @"<html>

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

        public static string HtmlPostTemplate = @"
            <br><br><hr><br>
            </div>

            </body>

            </html>";

        public static string FillerTemplate = @"
        
            <p class=MsoNormal>&nbsp;</p>
            <br><br><br><hr><br><br>
            <p class=MsoNormal><i>Content</i></p>

            <p class=MsoNormal>&nbsp;</p>

            <p class=MsoNormal>&nbsp;</p>

            <p class=MsoNormal>&nbsp;</p>

            <p class=MsoNormal>&nbsp;</p>";

        public static class Table
        {
            public static string trTemplate = @" <tr>
              <td valign=top style='width:50pt;border:solid #BDD6EE 1.0pt;
              border-top:none;padding:0in 5.4pt 0in 5.4pt'>
              <p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:
              normal'>{0}</p>
              </td>
              <td valign=top style='width:50pt;border-top:none;border-left:
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

            public static string TableTemplate = @"
            <table class=MsoTable15Grid4Accent2 border=1 cellspacing=0 cellpadding=0
             style='border-collapse:collapse;border:none'>
             <tr style='height:12.75pt'>
              <td valign=top style='width:50pt;border:solid #ED7D31 1.0pt;
              border-right:none;background:#C00000;padding:0in 5.4pt 0in 5.4pt;height:12.75pt'>
              <p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:
              normal'><b><span style='color:white'>Lines Added</span></b></p>
              </td>
              <td valign=top style='width:50pt;border-top:solid #ED7D31 1.0pt;
              border-left:none;border-bottom:solid #ED7D31 1.0pt;border-right:none;
              background:#C00000;padding:0in 5.4pt 0in 5.4pt;height:12.75pt'>
              <p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:
              normal'><b><span style='color:white'>Lines Removed</span></b></p>
              </td>
              <td valign=top style='width:100pt;border-top:solid #ED7D31 1.0pt;
              border-left:none;border-bottom:solid #ED7D31 1.0pt;border-right:none;
              background:#C00000;padding:0in 5.4pt 0in 5.4pt;height:12.75pt'>
              <p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:
              normal'><b><span style='color:white'>Number of commits</span></b></p>
              </td>
              <td valign=top style='width:500pt;border:solid #ED7D31 1.0pt;
              border-left:none;background:#C00000;padding:0in 5.4pt 0in 5.4pt;height:12.75pt'>
              <p class=MsoNormal style='margin-bottom:0in;margin-bottom:.0001pt;line-height:
              normal'><b><span style='color:white'>File Path</span></b></p>
              </td>
             </tr>
             {0}
            </table>";
        }

        public static class SvgContribution
        {
            public static string SVGTemplatePre = @"
            <svg width = '721' height='110'>
                <g transform = 'translate(20, 20)' >";

            public static string CellEntryTemplate = @"
                    <g transform='translate({0}, 0)'>
                        <rect class='day' width='11' height='11' y='{1}' fill='{2}' data-count='{3}' data-date='{4}'></rect>
                    </g>";

            public static string SVGTemplatePost = @"
                    <text x='26' y='-5' class='month'>Aug</text>
                    <text x='91' y='-5' class='month'>Sep</text>
                    <text x='143' y='-5' class='month'>Oct</text>
                    <text x='195' y='-5' class='month'>Nov</text>
                    <text x='260' y='-5' class='month'>Dec</text>
                    <text x='312' y='-5' class='month'>Jan</text>
                    <text x='364' y='-5' class='month'>Feb</text>
                    <text x='416' y='-5' class='month'>Mar</text>
                    <text x='481' y='-5' class='month'>Apr</text>
                    <text x='533' y='-5' class='month'>May</text>
                    <text x='598' y='-5' class='month'>Jun</text>
                    <text x='650' y='-5' class='month'>Jul</text>
                    <text text-anchor='middle' class='wday' dx='-10' dy='9' style='display: none;'>S</text>
                    <text text-anchor='middle' class='wday' dx='-10' dy='22'>M</text>
                    <text text-anchor='middle' class='wday' dx='-10' dy='35' style='display: none;'>T</text>
                    <text text-anchor='middle' class='wday' dx='-10' dy='48'>W</text>
                    <text text-anchor='middle' class='wday' dx='-10' dy='61' style='display: none;'>T</text>
                    <text text-anchor='middle' class='wday' dx='-10' dy='74'>F</text>
                    <text text-anchor='middle' class='wday' dx='-10' dy='87' style='display: none;'>S</text>
                </g>
            </svg>";

        }
    }
}
