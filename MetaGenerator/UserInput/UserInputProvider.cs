﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using Autofac.Features.Indexed;
using Funcky.Extensions;
using Funcky.Monads;
using Messerli.MetaGeneratorAbstractions;
using Messerli.MetaGeneratorAbstractions.Json;
using Messerli.MetaGeneratorAbstractions.UserInput;
using static Funcky.Functional;

namespace Messerli.MetaGenerator.UserInput;

internal class UserInputProvider : IUserInputProvider
{
    private readonly ITemplateLoader _templateLoader;
    private readonly IExecutingPluginAssemblyProvider _executingPluginAssemblyProvider;
    private readonly IIndex<VariableType, AbstractVariableRequester> _variableRequesters;
    private readonly Dictionary<string, IUserInputDescription> _knownUserInputs = new();
    private readonly Func<UserInputDescriptionBuilder> _newInputDescriptionBuilder;
    private readonly DataContractJsonSerializer _jsonSerializer;

    public UserInputProvider(
        ITemplateLoader templateLoader,
        Func<UserInputDescriptionBuilder> newInputDescriptionBuilder,
        IExecutingPluginAssemblyProvider executingPluginAssemblyProvider,
        IIndex<VariableType, AbstractVariableRequester> variableRequesters)
    {
        _templateLoader = templateLoader;
        _executingPluginAssemblyProvider = executingPluginAssemblyProvider;
        _variableRequesters = variableRequesters;
        _newInputDescriptionBuilder = newInputDescriptionBuilder;
        _jsonSerializer = new DataContractJsonSerializer(typeof(List<Variable>));
    }

    public IUserInputDescription this[string key] => GetUserInputDescription(key);

    public void RegisterVariable(UserInputDescription description)
        => _knownUserInputs.Add(description.VariableName, description);

    public void RegisterVariablesFromTemplate(string templateName)
        => GetVariablesFromTemplate(templateName)
            .ForEach(RegisterVariablesFromJson);

    public void AskUser(Dictionary<string, string> userArguments)
    {
        VerifyUserInputs();

        foreach (var variable in _knownUserInputs.Select(v => v.Value).Where(v => v.IsNeeded.Value))
        {
            var variableRequester = _variableRequesters[variable.VariableType];

            variable.Value = Option.Some(variableRequester.RequestValue(variable, userArguments.GetValueOrNone(key: variable.VariableName)));
        }
    }

    public IEnumerable<IUserInputDescription> GetUserInputDescriptions()
        => _knownUserInputs.Values;

    public Dictionary<string, string> GetVariableValues()
        => _knownUserInputs
            .Select(kv => kv.Value)
            .ToDictionary(v => v.VariableName, v => v.Value.GetOrElse("BAD VALUE!"));

    public string Value(string variableName)
        => _knownUserInputs
            .GetValueOrNone(key: variableName)
            .SelectMany(userInput => userInput.Value)
            .GetOrElse(() => throw new Exception($"Variable '{variableName}' is not a registered user input."));

    private List<Variable> GetVariablesFromTemplate(string templateName)
    {
        using var stream = _templateLoader.GetTemplateStream(templateName);
        var list = Option
            .FromNullable(stream)
            .Match(
                none: () => throw new Exception("no stream"),
                some: s => Option.FromNullable((List<Variable>?)_jsonSerializer.ReadObject(s)));

        return list.GetOrElse(() => throw new Exception("read object failed"));
    }

    private void RegisterVariablesFromJson(Variable variable)
        => RegisterVariable(BuildUserInput(variable));

    private UserInputDescription BuildUserInput(Variable variable)
        => _newInputDescriptionBuilder()
            .RegisterVariableName(variable.Name ?? throw new Exception("Variable name cannot be empty!"))
            .RegisterVariableQuestion(Option.FromNullable(variable.Question))
            .SetVariableType(variable.GetVariableType())
            .RegisterSelectionValues(variable)
            .RegisterVariableValidations(variable, _executingPluginAssemblyProvider.PluginAssembly)
            .Build();

    private IUserInputDescription GetUserInputDescription(string variableName)
        => _knownUserInputs
            .GetValueOrNone(key: variableName)
            .GetOrElse(() => NoValue(variableName));

    private static IUserInputDescription NoValue(in string variableName)
        => throw new Exception($"No value known for '{variableName}'");

    private void VerifyUserInputs()
        => _knownUserInputs
            .Select(v => v.Value)
            .ForEach(VerifyVariable);

    private static void VerifyVariable(IUserInputDescription variable)
    {
        if (variable.VariableType == VariableType.Selection && (variable.VariableSelectionValues is null || variable.VariableSelectionValues.Count == 0))
        {
            throw new Exception("If the variable type is selection, there must be at least one selection value.");
        }

        if (variable.VariableType != VariableType.Selection && variable.VariableSelectionValues is not null && variable.VariableSelectionValues.Count > 0)
        {
            throw new Exception("You have specified values for a selection, but the type is not a selection.");
        }

        if (variable.VariableSelectionValues is not null && variable.VariableSelectionValues.Any(selectionValue => string.IsNullOrEmpty(selectionValue.Value)))
        {
            throw new Exception("All selections value must have a valid string value!");
        }
    }
}
