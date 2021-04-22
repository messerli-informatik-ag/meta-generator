using System;
using System.Linq;
using System.Reflection;
using Funcky.Extensions;
using Funcky.Monads;
using Messerli.MetaGeneratorAbstractions.Json;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.MetaGenerator.UserInput
{
    internal static class UserInputDescriptionBuilderExtension
    {
        public static UserInputDescriptionBuilder RegisterVariableName(this UserInputDescriptionBuilder builder, string variableName)
            => builder.SetVariableName(variableName);

        public static UserInputDescriptionBuilder RegisterVariableQuestion(this UserInputDescriptionBuilder builder, Option<string> variableQuestion)
        {
            _ = variableQuestion.AndThen(question => builder.SetVariableQuestion(question));

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
            foreach (var validation in variable.Validations)
            {
                builder.SetValidation(FindValidationFunction(new ValidationName(validation), pluginAssembly));
            }

            return builder;
        }

        private static IValidation FindValidationFunction(ValidationName validationName, Assembly pluginAssembly)
            => FindGlobalValidation(validationName)
                .OrElse(() => FindPluginValidations(validationName, pluginAssembly))
                .GetOrElse(() => throw new NotImplementedException($"Validation '{validationName}' not found"));

        private static Option<IValidation> FindGlobalValidation(ValidationName validationName)
            => validationName.Class == nameof(Validations)
                ? FindValidationOnType(validationName, typeof(Validations)).AndThen(ToValidation)
                : Option<IValidation>.None();

        private static Option<object> FindValidationOnType(ValidationName validationName, Type type)
            => type
                .GetProperties()
                .Where(p => p.Name == validationName.Property)
                .Select(p => p.GetValue(null))
                .WhereNotNull()
                .FirstOrNone();

        private static Option<IValidation> FindPluginValidations(ValidationName validationName, Assembly pluginAssembly)
            => FindValidationOnType(validationName, FindPluginValidationType(validationName.Class, pluginAssembly))
                .AndThen(ToValidation);

        private static IValidation ToValidation(object v)
            => (IValidation)v;

        private static Type FindPluginValidationType(string className, Assembly pluginAssembly)
            => pluginAssembly
                .GetTypes()
                .Where(t => t.Name == className)
                .FirstOrNone()
                .GetOrElse(() => throw new TypeLoadException($"A class with the given name '{className}' was not found in the plugin '{pluginAssembly.GetName()}'"));
    }
}
