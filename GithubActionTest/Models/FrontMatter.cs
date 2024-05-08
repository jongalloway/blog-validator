using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Markdig.Extensions.Yaml;
using Markdig.Parsers;
using Markdig.Syntax;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;


namespace BlogValidator.Models
{
    internal class FrontMatter
    {
        // This class will represent the front matter of a markdown document
        // It has a title, date, and author, publishdate, summary, and tags
        public string Title { get; set; }
        public string Date { get; set; }
        public string Author { get; set; }
        public string PublishDate { get; set; }
        public string Summary { get; set; }
        public List<string> Tags { get; set; }

        // Create a parameterless constructor for the deserializer
        public FrontMatter()
        {
        }

        public bool TryParse(MarkdownDocument markdownDocument)
        {
            // Get the front matter block
            var frontMatterBlock = markdownDocument.FirstOrDefault() as YamlFrontMatterBlock;
            if (frontMatterBlock != null)
            {
                try {
                    var yaml = string.Join(Environment.NewLine, frontMatterBlock.Lines);
                    var deserializer = new DeserializerBuilder()
                        .WithNamingConvention(UnderscoredNamingConvention.Instance)
                        .IgnoreUnmatchedProperties()
                        .Build();

                    var frontMatter = deserializer.Deserialize<FrontMatter>(yaml);
                    if (frontMatter != null)
                    {
                        Title = frontMatter.Title;
                        Date = frontMatter.Date;
                        Author = frontMatter.Author;
                        PublishDate = frontMatter.PublishDate;
                        Summary = frontMatter.Summary;
                        Tags = frontMatter.Tags;
                        return true;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error parsing front matter: {e.Message}");
                    return false;
                }
            }
            return false;
        }
    }
}
