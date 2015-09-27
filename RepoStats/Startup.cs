
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

            config.MapHttpAttributeRoutes();

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
}
