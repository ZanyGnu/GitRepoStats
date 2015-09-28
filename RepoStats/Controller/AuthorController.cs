
namespace RepoStats.Controller
{
    using RazorEngine;
    using RazorEngine.Templating;
    using System.IO;
    using System.Net.Http;
    using System.Web.Http;

    public class AuthorController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Get(string user)
        {
            string template = File.ReadAllText("views\\user.cshtml");
            string bodyContent = File.ReadAllText("CommitsByAuthor/" + user + "Contributions.html");
            var model = new { Title = "Commit Details for " + user, BodyContent = bodyContent };
            string result = Engine.Razor.RunCompile(template, "templateNameAuthor", null, model);
            return new HttpResponseMessage()
            {
                Content = new StringContent(result, System.Text.Encoding.UTF8, "text/html"),
            };
        }
    }
}
