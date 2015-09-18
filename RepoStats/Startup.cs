
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
    using System.Web.Routing;
    using System.Threading.Tasks;
    using System.Net.Http;
    using RazorEngine;
    using RazorEngine.Templating;

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseErrorPage();

            // Remap '/' to '.\defaults\'.
            // Turns on static files and default files.
            app.UseFileServer(new FileServerOptions()
            {
                RequestPath = PathString.Empty,
                FileSystem = new PhysicalFileSystem(@".\"),
            });

            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute("Default", "{controller}/{checkinID}", new { controller = "Checkin", checkinID = RouteParameter.Optional });
            config.Routes.MapHttpRoute("Default3", "{controller}/route/{arg}", new { controller = "Checkin", checkinID = RouteParameter.Optional });
            config.Routes.MapHttpRoute("Default2", "{controller}/{checkinID}", new { controller = "Razor", checkinID = RouteParameter.Optional });

            //config.Formatters.XmlFormatter.UseXmlSerializer = true;
            //config.Formatters.Remove(config.Formatters.JsonFormatter);
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.JsonFormatter.UseDataContractJsonSerializer = true;

            app.UseWebApi(config);
            app.UseWelcomePage();
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
        public CommitDetails Get(string checkinId)
        {
            return GetCheckinDetails(checkinId);
        }

        [HttpGet]
        [ActionName("route")]
        public string Route(string arg)
        {
            return "route controller";
        }

        public static CommitDetails GetCheckinDetails(string checkinId)
        {
            Repository repo = new Repository(ConfigurationManager.AppSettings["RepoRoot"]);
            //Commit commit = repo.Lookup<Commit>(checkinId);
            //return commit.Message;

            string patchDirectory = Path.Combine(repo.Info.Path, "patches");
            CommitDetails commitDetails = new CommitDetails();

            IEnumerable<string> files = Directory.EnumerateFiles(patchDirectory, checkinId + "*", SearchOption.TopDirectoryOnly);
            if (files != null && files.Any())
            {
                string patchFileName = files.First();
                string commitId = Path.GetFileNameWithoutExtension(patchFileName);
                commitDetails.Commit = repo.Lookup<Commit>(commitId);
                commitDetails.FileChanges = Serializer.Deserialize<List<FileChanges>>(File.OpenRead(patchFileName));
            }

            return commitDetails;
        }
    }

    public class RazorController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Get(string checkinId)
        {
            string template = File.ReadAllText("views\\commit.cshtml");
            string result = Engine.Razor.RunCompile(template, "templateName", null, CheckinController.GetCheckinDetails(checkinId));
            return new HttpResponseMessage()
            {
                Content = new StringContent(result, System.Text.Encoding.UTF8, "text/html"), 
            };
        }
    }
}
