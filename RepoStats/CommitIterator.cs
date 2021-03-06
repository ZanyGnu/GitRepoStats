﻿
namespace RepoStats
{
    using LibGit2Sharp;
    using ProtoBuf;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;

    public class CommitIterator
    {
        List<CommitAnalyzer> commitAnalysis;
        List<FileChangeAnalyzer> patchAnalysis;
        string repoRoot;
        string repoName;

        enum SerializerType
        {
            XmlSerializer,
            ProtoBuf,
        }

        private static SerializerType currentSeerializer = CommitIterator.SerializerType.ProtoBuf;

        public CommitIterator(string repoRoot, string repoName, List<CommitAnalyzer> commitAnalysis, List<FileChangeAnalyzer> patchAnalysis)
        {
            this.commitAnalysis = commitAnalysis;
            this.patchAnalysis = patchAnalysis;
            this.repoRoot = repoRoot;
            this.repoName = repoName;
        }

        public void Iterate()
        {
            
            XmlSerializer serializer = new XmlSerializer(typeof(List<FileChanges>));

            using (var repo = new Repository(repoRoot))
            {
                string patchDirectory = Path.Combine(repo.Info.Path, "patches");
                                
                if (!Directory.Exists(patchDirectory))
                {
                    Directory.CreateDirectory(patchDirectory);
                }

                int commitCount = repo.Commits.Count();
                int currentCommitCount = 0;

                foreach (Commit c in repo.Commits)
                {
                    currentCommitCount++;
                    Console.Write("\rProcessing {0}/{1} ({2}%)    ", currentCommitCount, commitCount, currentCommitCount * 100 / commitCount);

                    ExecuteCommitAnalysis(c, commitAnalysis, patchAnalysis);

                    if (c.Parents.Count() == 0)
                    {
                        continue;
                    }

                    List<FileChanges> fileChanges = null;

                    string ext = GetExtension();
                    string patchFileName = patchDirectory + "/" + c.Id + ext;

                    if (File.Exists(patchFileName))
                    {
                        //Console.WriteLine("Reloading {0} from cache.", c.Id);
                        try
                        {
                            switch (currentSeerializer)
                            {
                                case SerializerType.XmlSerializer:
                                    fileChanges = (List<FileChanges>)serializer.Deserialize(File.OpenRead(patchFileName));
                                    break;
                                case SerializerType.ProtoBuf:
                                    fileChanges = Serializer.Deserialize<List<FileChanges>>(File.OpenRead(patchFileName));
                                    break;
                                default:
                                    break;
                            }

                        }
                        catch
                        {
                            // any exception to deserialize, we attempt to find the patch info again and serialize.
                        }
                    }

                    if (fileChanges == null)
                    {
                        fileChanges = new List<FileChanges>();
                        // TODO: need to handle multiple parents.
                        if (c.Parents.Count() > 1)
                        {
                            // ignore all merge commits
                            continue;
                        }

                        Patch changes = null;
                        changes = repo.Diff.Compare<Patch>(c.Parents.First().Tree, c.Tree);

                        foreach (var patchChanges in changes)
                        {
                            FileChanges change = new FileChanges()
                            {
                                LinesAdded = patchChanges.LinesAdded,
                                LinesDeleted = patchChanges.LinesDeleted,
                                Diff = patchChanges.Patch,
                                Path = patchChanges.Path
                            };

                            fileChanges.Add(change);
                        }

                        switch (currentSeerializer)
                        {
                            case SerializerType.XmlSerializer:
                                serializer.Serialize(new FileStream(patchFileName, FileMode.OpenOrCreate), fileChanges);
                                break;
                            case SerializerType.ProtoBuf:
                                Serializer.Serialize(new FileStream(patchFileName, FileMode.OpenOrCreate), fileChanges);
                                break;
                            default:
                                break;
                        }
                    }

                    ExecutePatchAnalysis(patchAnalysis, c, fileChanges);
                }
            }
        }

        private static string GetExtension()
        {
            switch (currentSeerializer)
            {
                case SerializerType.XmlSerializer:
                    return ".xml";
                case SerializerType.ProtoBuf:
                    return ".bin";
                default:
                    return "";
            }
        }

        public void WriteOutput()
        {
            string htmlData = "";
            if (commitAnalysis != null)
            {
                foreach (CommitAnalyzer ca in commitAnalysis)
                {
                    ca.Write();
                    htmlData += ca.GetFormattedString();
                }
            }

            if (patchAnalysis != null)
            {
                foreach (FileChangeAnalyzer pa in patchAnalysis)
                {
                    pa.Write();
                    htmlData += pa.GetFormattedString();
                }
            }

            File.WriteAllText("report.html",
                String.Concat(
                    HtmlTemplates.HtmlPreTemplate,
                    htmlData,
                    HtmlTemplates.HtmlPostTemplate));
        }

        private static void ExecuteCommitAnalysis(Commit c, List<CommitAnalyzer> commitAnalysis, List<FileChangeAnalyzer> patchAnalysis)
        {
            if (commitAnalysis != null)
            {
                foreach (CommitAnalyzer ca in commitAnalysis)
                {
                    ca.Visit(c);
                }
            }

            if (patchAnalysis != null)
            {
                foreach (FileChangeAnalyzer pa in patchAnalysis)
                {
                    pa.Visit(c);
                }
            }
        }

        private static void ExecutePatchAnalysis(List<FileChangeAnalyzer> patchAnalysis, Commit c, List<FileChanges> changes)
        {
            if (patchAnalysis != null && changes != null)
            {
                foreach (FileChanges fileChanges in changes)
                {
                    foreach (FileChangeAnalyzer pa in patchAnalysis)
                    {
                        pa.Visit(c, fileChanges);
                    }
                }
            }
        }
    }
}
