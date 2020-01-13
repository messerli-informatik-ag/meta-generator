namespace Messerli.ProjectAbstractions.UserInput
{
    public static class Validations
    {
        public static IValidation ValuePresent => new SimpleValidation(value => value.Length > 0, "Value '{0}' cannot be empty.");
    }
}
