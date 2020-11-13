using System;
using System.Linq;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Octokit;


namespace GitHubAffectedFiles
{
    class Program
    {
        public static async Task<int> Main(string[] args)
            => await CommandLineApplication.ExecuteAsync<Program>(args);

        [Argument(0, Description = "The GitHub owner/organisation", Name = "owner")]
        public string Owner { get; }
        [Argument(1, Description = "The GitHub repository", Name = "repo")]
        public string Repo { get; }
        [Argument(2, Description = "The base commit or reference")]
        public string Base { get; }
        [Argument(3, Description = "The final commit to compare")]
        public string Head { get; }
        
        [Option(Description = "An authentication token for calling the API", LongName = "auth", ShortName = "a")]
        public string Token { get; }


        async Task OnExecuteAsync(CommandLineApplication app)
        {
            if (string.IsNullOrEmpty(Repo) || string.IsNullOrEmpty(Owner) || string.IsNullOrEmpty(Base) || string.IsNullOrEmpty(Head))
            {
                app.ShowHelp();
                return;
            }
            
            if (string.IsNullOrEmpty(Token))
            {
                app.ShowHelp();
                
                await app.Out.WriteLineAsync("Authentication token (--auth) is required");

                return;
            }
            
            var github = new GitHubClient(new ProductHeaderValue(AppName))
            {
                Credentials = new Credentials(Token)
            };

            var results = await github.Repository.Commit.Compare(Owner, Repo, Base, Head);

            foreach (var file in results.Files.Distinct())
            {
                Console.WriteLine(file.Filename);
            }
        }
        const string AppName = "GitHubAffectedFiles";
    }
}