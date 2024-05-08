using BlogValidator.Models;

namespace BlogValidator.Rules;

interface IValidationRule
{
    bool Validate(ValidationContext context);
}
