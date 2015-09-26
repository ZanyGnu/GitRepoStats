
namespace RepoStats.Controller
{
    using RazorEngine;
    using RazorEngine.Templating;
    using System.IO;
    using System.Net.Http;
    using System.Web.Http;

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
}
