using BlogValidator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogValidator.Rules
{
    internal class VR02_Title_Is_Required: IValidationRule
    {
        public bool Validate(ValidationContext context)
        {
            // If the title is null, return false
            if (context.FrontMatter.Title == null)
            {
                context.ValidationMessage = "❌ Error: Title is missing";
                context.ValidationSuggestion = "Add a title to the front matter";
                return false;
            }
            else
            {
                context.ValidationMessage = "✅ Title is present";
                return true;
            }
        }

    }
}
