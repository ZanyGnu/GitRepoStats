
namespace RepoStats
{
    using LibGit2Sharp;
    using ProtoBuf;

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
    }
}
