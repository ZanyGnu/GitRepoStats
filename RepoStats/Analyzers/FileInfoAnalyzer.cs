
namespace RepoStats.Analyzers
{
    using LibGit2Sharp;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class FileInfoAnalyzer : FileChangeAnalyzer
    {
        public class GitFileInfo
        {
            public string Path { get; set; }
            public int NumberOfCommits { get; set; }
            public int LinesAdded { get; set; }
            public int LinesDeleted { get; set; }
        }

        public Dictionary<string, GitFileInfo> GitFileInfos = new Dictionary<string, GitFileInfo>();

        DateTime startDate;
        DateTime endDate;

        public FileInfoAnalyzer(DateTime startDate, DateTime endDate)
        {
            this.startDate = startDate;
            this.endDate = endDate;
        }

        public void Visit(Commit c)
        {

        }

        public void Visit(Commit commit, FileChanges fileChanges)
        {
            if (!GitFileInfos.ContainsKey(fileChanges.Path))
            {
                GitFileInfos.Add(
                    fileChanges.Path,
                    new GitFileInfo()
                    {
                        Path = fileChanges.Path,
                        LinesAdded = 0,
                        LinesDeleted = 0,
                        NumberOfCommits = 0
                    });
            }

            GitFileInfos[fileChanges.Path].LinesAdded += fileChanges.LinesAdded;
            GitFileInfos[fileChanges.Path].LinesDeleted += fileChanges.LinesDeleted;
            GitFileInfos[fileChanges.Path].NumberOfCommits++;
        }

        public void Write()
        {
            //Console.WriteLine("Files ordered by number of modifications");
            //IOrderedEnumerable<FileInfoAnalyzer.GitFileInfo> orderedChanges = GitFileInfos.Values.OrderByDescending(c => c.LinesDeleted + c.LinesAdded);
            //foreach (FileInfoAnalyzer.GitFileInfo fileInfo in orderedChanges.Take(20))
            //{
            //    Console.WriteLine("\t{0} {1} {2}", fileInfo.Path, fileInfo.LinesAdded, fileInfo.LinesDeleted);
            //}

            //Console.WriteLine("Files ordered by number of commit touches");
            //orderedChanges = GitFileInfos.Values.OrderByDescending(c => c.NumberOfCommits);
            //foreach (FileInfoAnalyzer.GitFileInfo fileInfo in orderedChanges.Take(20))
            //{
            //    Console.WriteLine("\t{0} {1}", fileInfo.Path, fileInfo.NumberOfCommits);
            //}
        }

        public string GetFormattedString()
        {
            string fileInfoChangesTableString = HtmlTemplates.Table.TableTemplate;
            StringBuilder trFileInfosContent = new StringBuilder();

            foreach (FileInfoAnalyzer.GitFileInfo fileInfo in GitFileInfos.Values.OrderByDescending(c => (c.NumberOfCommits)).Take(20))
            {
                trFileInfosContent.AppendFormat(
                    HtmlTemplates.Table.trTemplate,
                    fileInfo.LinesAdded,
                    fileInfo.LinesDeleted,
                    fileInfo.NumberOfCommits,
                    fileInfo.Path);
            }

            fileInfoChangesTableString = String.Format(
                fileInfoChangesTableString.EscapeForFormat(), 
                "Hot files",
                trFileInfosContent);

            return fileInfoChangesTableString;
        }
    }
}
