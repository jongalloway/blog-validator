using GitHub;
using GitHub.Octokit.Authentication;
using GitHub.Octokit.Client;
using BlogValidator.Models;
using BlogValidator.Rules;
using Markdig;

Console.OutputEncoding = System.Text.Encoding.Unicode;
Console.WriteLine("🔥Here we go!🔥");

// Populate the repoOwner and repoName from environment variables. These are set by GitHub Actions. The repoOwner and repoName need to be parsed from GITHUB_REPOSITORY, which is in the format owner/repo.

var repository = Environment.GetEnvironmentVariable("GITHUB_REPOSITORY")?.Split('/') ?? new string[] { "jongalloway", "fake-blog" };

var repoOwner = repository[0];
var repoName = repository[1];

// Get the pull request number from the environment variable. This is set by GitHub Actions. The environment variable is GITHUB_REF_NAME and is in the format of :pr_number/merge. Parse the pull request number from the string.

var pullRequestNumber = int.Parse(Environment.GetEnvironmentVariable("GITHUB_REF_NAME")?.Split('/')[0] ?? "1");

var token = Environment.GetEnvironmentVariable("GITHUB_TOKEN") ?? "";
var request = RequestAdapter.Create(new TokenAuthenticationProvider("Octokit.Gen", token));
var gitHubClient = new GitHubClient(request);

// Get the pull request by number
var pullRequest = await gitHubClient.Repos[repoOwner][repoName].Pulls[pullRequestNumber].GetAsync();

Console.WriteLine($"👀 #{pullRequest.Number} {pullRequest.Title}");
var files = await gitHubClient.Repos[repoOwner][repoName].Pulls[pullRequestNumber].Files.GetAsync();

// Get all the markdown files
var markdownFiles = files.Where(f => f.Filename.EndsWith(".md"));

// If there is more than one markdown file, return an error
if (markdownFiles.Count() > 1)
{
    Console.WriteLine("❌ Error: More than one markdown file in the PR");
    return 1;
}
else
{
    Console.WriteLine("✅ One markdown file in the PR");

    // Get the first markdown file
    var markdownFile = markdownFiles.FirstOrDefault();

    var markdownFileContent = await gitHubClient.Repos[repoOwner][repoName].Git.Blobs[markdownFile.Sha].GetAsync();
    var markdownFileText = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(markdownFileContent.Content));
    Console.WriteLine(markdownFileText);

    // Use markdig to parse the markdown file
    var pipeline = new Markdig.MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

    // Create a validation context
    var context = new ValidationContext(markdownFileText, markdownFile.Filename);

    // Get all the rules by looking up all the types that implement IValidationRule
    var rules = AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(s => s.GetTypes())
        .Where(p => typeof(IValidationRule).IsAssignableFrom(p) && p.IsClass)
        .Select(r => (IValidationRule)Activator.CreateInstance(r))
        .ToList();

    // Run all the rules. If any fail, return an error.
    int errorCount = 0;
    string validationSummary = "";
    const string validationHeader = "| Issue | File | Message |\n| --- | --- | --- |\n";

    foreach (var rule in rules)
    {
        var result = rule.Validate(context);
        if (!result)
        {
            errorCount++;
        }

        // Format the results in a table using GitHub Actions output as a table
        Console.WriteLine($"::set-output name=ValidationMessage::{context.ValidationMessage}");

        // Parse the issue number (e.g. VR01) from the rule name
        var ruleCode = rule.GetType().Name.Substring(2, 2);

        validationSummary += $"| {ruleCode} | {context.ValidationMessage} | {context.Filename} | {context.ValidationSuggestion} |\n";

    }

    // If there are any errors, return an error
    if (errorCount > 0)
    {
        var comment = await gitHubClient.Repos[repoOwner][repoName].Issues[pullRequestNumber].Comments.PostAsync(
        new GitHub.Repos.Item.Item.Issues.Item.Comments.CommentsPostRequestBody
        {
            Body = $"## Validation Results\n\n{validationHeader}{validationSummary}"
        });

        return 1;
    }
}

// Succeeded so exit with success code
return 0;