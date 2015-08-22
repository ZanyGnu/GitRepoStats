
namespace RepoStats
{
    using Microsoft.Owin;
    using Microsoft.Owin.FileSystems;
    using Microsoft.Owin.StaticFiles;
    using Owin;

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Remap '/' to '.\defaults\'.
            // Turns on static files and default files.
            app.UseFileServer(new FileServerOptions()
            {
                RequestPath = PathString.Empty,
                FileSystem = new PhysicalFileSystem(@".\"),
            });

            // Only serve files requested by name.
            app.UseStaticFiles("/commitsByAuthor");

            // Turns on static files, directory browsing, and default files.
            app.UseFileServer(new FileServerOptions()
            {
                RequestPath = new PathString("/commitsByAuthor"),
                EnableDirectoryBrowsing = true,
            });

            // Browse the root of your application (but do not serve the files).
            // NOTE: Avoid serving static files from the root of your application or bin folder,
            // it allows people to download your application binaries, config files, etc..
            app.UseDirectoryBrowser(new DirectoryBrowserOptions()
            {
                RequestPath = new PathString("/src"),
                FileSystem = new PhysicalFileSystem(@""),
            });
        }
    }
}
