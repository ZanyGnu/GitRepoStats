
namespace RepoStats
{
    using CommandLine;
    using CommandLine.Text;

    class Options
    {
        [Option('s', "server", Required = false, DefaultValue = "true", HelpText = "Run server to host pages.")]
        public string RunServer { get; set; }

        [Option('a', "analyze", Required = false, DefaultValue = "true", HelpText = "Analyze the configured repositories.")]
        public string AnalyzeRepositories { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(
                this,
                (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
