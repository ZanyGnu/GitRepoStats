
namespace RepoStats
{
    using LibGit2Sharp;

    public interface CommitAnalyzer
    {
        void Visit(Commit c);
    }

    public interface PatchAnalyzer : CommitAnalyzer
    {
        void Visit(Commit commit, PatchEntryChanges patch);
    }
}
