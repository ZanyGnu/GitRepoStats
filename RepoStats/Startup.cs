
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
            //config.Routes.MapHttpRoute("Default", "{controller}/{checkinID}", new { controller = "Checkin", checkinID = RouteParameter.Optional });
            //config.Routes.MapHttpRoute("Default3", "{controller}/route/{arg}", new { controller = "Checkin", checkinID = RouteParameter.Optional });
            config.Routes.MapHttpRoute("Default", "{controller}/{commitID}", new { controller = "Commit", commitID = RouteParameter.Optional });
            config.Routes.MapHttpRoute("Default2", "{controller}", new { controller = "Repository" });
            config.Routes.MapHttpRoute("Default3", "{controller}/{user}", new { controller = "User", user = RouteParameter.Optional });

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

    public class CommitController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Get(string commitID)
        {
            string template = File.ReadAllText("views\\commit.cshtml");
            string result = Engine.Razor.RunCompile(template, "templateName", null, CheckinController.GetCheckinDetails(commitID));
            return new HttpResponseMessage()
            {
                Content = new StringContent(result, System.Text.Encoding.UTF8, "text/html"), 
            };
        }
    }

    public class RepositoryController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Get()
        {
            string template = File.ReadAllText("views\\repository.cshtml");
            string bodyContent = File.ReadAllText("RepoContributionMap.html");
            var model = new { Title = "Repository Details", BodyContent = bodyContent };
            string result = Engine.Razor.RunCompile(template, "templateName", null, model);
            return new HttpResponseMessage()
            {
                Content = new StringContent(result, System.Text.Encoding.UTF8, "text/html"),
            };
        }
    }

    public class UserController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Get(string user)
        {
            string template = File.ReadAllText("views\\user.cshtml");
            string bodyContent = File.ReadAllText("CommitsByAuthor/" + user + "Contributions.html");
            var model = new { Title = "Commit Details for " + user, BodyContent = bodyContent };
            string result = Engine.Razor.RunCompile(template, "templateName", null, model);
            return new HttpResponseMessage()
            {
                Content = new StringContent(result, System.Text.Encoding.UTF8, "text/html"),
            };
        }
    }
}
