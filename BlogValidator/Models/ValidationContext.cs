using BlogValidator.Rules;
using Markdig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogValidator.Models
{
    internal class ValidationContext
    {
        private string MarkdownText { get; set; }
        private Markdig.Syntax.MarkdownDocument MarkdownDocument { get; set; }
        public string Filename { get; set; }
        public string ValidationMessage { get; set; }
        public string ValidationSuggestion { get; set; }
        public bool ValidationResult { get; set; }
        public FrontMatter FrontMatter { get;  }

        public ValidationContext(string markdownText, string filename)
        {
            MarkdownText = markdownText;
            Filename = filename;

            // create a markdig documennt parser with yaml front matter extension

            var parser = new Markdig.MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseYamlFrontMatter()
                .Build();

            MarkdownDocument = Markdown.Parse(markdownText, parser);

            var frontMatter = new FrontMatter();
            if (frontMatter.TryParse(MarkdownDocument))
            {
                FrontMatter = frontMatter;
            }
        }
    }
}
