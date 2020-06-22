using System;
using System.Linq;
using System.Reflection;
using Funcky.Monads;
using Messerli.MetaGeneratorAbstractions.Json;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.MetaGenerator.UserInput
{
    internal static class UserInputDescriptionBuilderExtension
    {
        public static UserInputDescriptionBuilder RegisterVariableName(this UserInputDescriptionBuilder builder, string? variableName)
        {
            if (variableName == null)
            {
                throw new Exception("A variable must have a name.");
            }

            builder.SetVariableName(variableName);

            return builder;
        }

        public static UserInputDescriptionBuilder RegisterVariableQuestion(this UserInputDescriptionBuilder builder, string? variableQuestion)
        {
            if (variableQuestion != null)
            {
                builder.SetVariableQuestion(variableQuestion);
            }

            return builder;
        }

        public static UserInputDescriptionBuilder RegisterSelectionValues(this UserInputDescriptionBuilder builder, Variable variable)
        {
            if (variable.GetVariableType() == VariableType.Selection && variable.SelectionValues != null && variable.SelectionValues.Count > 0)
            {
                builder.SetSelectionValues(variable.SelectionValues);
            }

            return builder;
        }

        public static UserInputDescriptionBuilder RegisterVariableValidations(this UserInputDescriptionBuilder builder, Variable variable, Assembly pluginAssembly)
        {
            if (variable.Validations != null)
            {
                foreach (var validation in variable.Validations)
                {
                    builder.SetValidation(FindValidationFunction(new ValidationName(validation), pluginAssembly));
                }
            }

            return builder;
        }

        private static IValidation FindValidationFunction(ValidationName validationName, Assembly pluginAssembly)
        {
            return FindGlobalValidation(validationName)
                .OrElse(() => FindPluginValidations(validationName, pluginAssembly))
                .GetOrElse(() => throw new NotImplementedException($"Validation '{validationName}' not found"));
        }

        private static Option<IValidation> FindGlobalValidation(ValidationName validationName)
        {
            return validationName.Class == nameof(Validations) && FindValidationOnType(validationName, typeof(Validations)) is IValidation validation
                ? Option.Some(validation)
                : Option<IValidation>.None();
        }

        private static object? FindValidationOnType(ValidationName validationName, Type type)
        {
            return type
                .GetProperties()
                .Where(p => p.Name == validationName.Property)
                .Select(p => p.GetValue(null))
                .FirstOrDefault();
        }

        private static Option<IValidation> FindPluginValidations(ValidationName validationName, Assembly pluginAssembly)
        {
            return FindValidationOnType(validationName, FindPluginValidationType(validationName.Class, pluginAssembly)) is IValidation validation
                ? Option.Some(validation)
                : Option<IValidation>.None();
        }

        private static Type FindPluginValidationType(string className, Assembly pluginAssembly)
        {
            return pluginAssembly
                .GetTypes()
                .Where(t => t.Name == className)
                .FirstOrDefault()
                ?? throw new TypeLoadException($"A class with the given name '{className}' was not found in the plugin '{pluginAssembly.GetName()}'");
        }
    }
}
