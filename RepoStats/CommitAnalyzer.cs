
namespace RepoStats
{
    using LibGit2Sharp;
    using ProtoBuf;
    using System.Collections.Generic;

    public interface CommitAnalyzer: IOutputWriter
    {
        void Visit(Commit c);
    }

    public interface FileChangeAnalyzer : CommitAnalyzer
    {
        void Visit(Commit commit, FileChanges fileChanges);
    }

    public interface IOutputWriter
    {
        void Write();

        string GetFormattedString();
    }

    [ProtoContract]
    public class FileChanges
    {
        [ProtoMember(1)]
        public string Path;

        [ProtoMember(2)]
        public int LinesAdded;

        [ProtoMember(3)]
        public int LinesDeleted;

        [ProtoMember(4)]
        public string Diff;
    }

    public class CommitDetails
    {
        public Commit Commit;
        public List<FileChanges> FileChanges;        
    }
}
