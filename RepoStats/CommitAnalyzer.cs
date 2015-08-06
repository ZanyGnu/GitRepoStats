
namespace RepoStats
{
    using LibGit2Sharp;

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

    public class FileChanges
    {
        public string Path;
        public int LinesAdded;
        public int LinesDeleted;
    }
}
