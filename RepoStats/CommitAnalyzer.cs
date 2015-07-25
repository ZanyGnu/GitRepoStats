
namespace RepoStats
{
    using LibGit2Sharp;

    public interface CommitAnalyzer: IOutputWriter
    {
        void Visit(Commit c);
    }

    public interface PatchAnalyzer : CommitAnalyzer
    {
        void Visit(Commit commit, PatchEntryChanges patch);
    }

    public interface IOutputWriter
    {
        void Write();

        string GetFormattedString();
    }
}
