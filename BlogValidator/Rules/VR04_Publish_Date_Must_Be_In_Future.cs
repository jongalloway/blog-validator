using BlogValidator.Models;

namespace BlogValidator.Rules
{
    internal class VR04_Publish_Date_Must_Be_In_Future : IValidationRule
    {
        public bool Validate(ValidationContext context)
        {
            // If the publish date is null, return false. Otherwise, parse the date and compare it to the current date.

            if (context.FrontMatter.PublishDate == null)
            {
                context.ValidationMessage = "❌ Error: Publish date is missing";
                context.ValidationSuggestion = "Add a publish date to the front matter";
                return false;
            }
            else
            {
                DateTime publishDate = DateTime.Parse(context.FrontMatter.PublishDate);
                if (publishDate < DateTime.Now)
                {
                    context.ValidationMessage = "❌ Error: Publish date is in the past";
                    context.ValidationSuggestion = "Update the publish date to a future date";
                    return false;
                }
                else
                {
                    context.ValidationMessage = "✅ Publish date is in the future";
                    return true;
                }
            }
        }
    }
}
