
namespace RepoStats.Controller
{
    using RazorEngine;
    using RazorEngine.Templating;
    using System.IO;
    using System.Net.Http;
    using System.Web.Http;

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
}
