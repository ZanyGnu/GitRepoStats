
namespace RepoStats.Controller
{
    using LibGit2Sharp;
    using ProtoBuf;
    using RazorEngine;
    using RazorEngine.Templating;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Web.Http;

    public class CommitController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Get(string commitID)
        {
            string template = File.ReadAllText("views\\commit.cshtml");
            string result = Engine.Razor.RunCompile(template, "templateNameCommit", null, GetCheckinDetails(commitID));
            return new HttpResponseMessage()
            {
                Content = new StringContent(result, System.Text.Encoding.UTF8, "text/html"),
            };
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
}
