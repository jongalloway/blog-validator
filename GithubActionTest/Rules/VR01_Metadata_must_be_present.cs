using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogValidator.Models;

namespace BlogValidator.Rules
{
    internal class VR01_FrontMatterMustExist: IValidationRule
    {
        public bool Validate(ValidationContext context)
        {
            // If the front matter is null, return false
            if (context.FrontMatter == null)
            {
                context.ValidationMessage = "❌ Error: Front matter is missing";
                context.ValidationSuggestion = "Add front matter to the markdown file";
                return false;
            }
            else
            {
                context.ValidationMessage = "✅ Front matter is present";
                return true;
            }
        }

    }
}
