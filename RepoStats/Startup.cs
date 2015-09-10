
namespace RepoStats
{
    using LibGit2Sharp;
    using Microsoft.Owin;
    using Microsoft.Owin.FileSystems;
    using Microsoft.Owin.StaticFiles;
    using Owin;
    using ProtoBuf;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Web.Http;

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

            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute("Default", "{controller}/{checkinID}", new { controller = "Checkin", checkinID = RouteParameter.Optional });

            config.Formatters.XmlFormatter.UseXmlSerializer = true;
            config.Formatters.Remove(config.Formatters.JsonFormatter);
            // config.Formatters.JsonFormatter.UseDataContractJsonSerializer = true;

            app.UseWebApi(config);
        }
    }

    public class CheckinController : ApiController
    {
        public string guid = Guid.NewGuid().ToString();

        public CheckinController()
        {
        }

        // Gets
        [HttpGet]
        public List<FileChanges> Get(string checkinId)
        {
            Repository repo = new Repository(ConfigurationManager.AppSettings["RepoRoot"]);
            //Commit commit = repo.Lookup<Commit>(checkinId);
            //return commit.Message;

            string patchDirectory = Path.Combine(repo.Info.Path, "patches");
            List<FileChanges> fileChanges = null;

            IEnumerable<string> files = Directory.EnumerateFiles(patchDirectory, checkinId + "*", SearchOption.TopDirectoryOnly);
            if (files != null && files.Any())
            {
                string patchFileName = files.First();
                fileChanges = Serializer.Deserialize<List<FileChanges>>(File.OpenRead(patchFileName));
            }

            return fileChanges;
        }
    }
}
