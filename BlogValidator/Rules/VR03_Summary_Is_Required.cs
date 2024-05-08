using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogValidator.Models;

namespace BlogValidator.Rules
{
    internal class VR03_Summary_Is_Required: IValidationRule
    {
        public bool Validate(ValidationContext context)
        {
            // If the summary is null, return false
            if (context.FrontMatter.Summary == null)
            {
                context.ValidationMessage = "❌ Error: Summary is missing";
                context.ValidationSuggestion = "Add a summary to the front matter";
                return false;
            }
            else
            {
                context.ValidationMessage = "✅ Summary is present";
                return true;
            }
        }

    }
}
