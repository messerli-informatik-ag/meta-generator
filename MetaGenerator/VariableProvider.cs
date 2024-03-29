﻿using System;
using System.Collections.Generic;
using System.Linq;
using Messerli.MetaGeneratorAbstractions;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.MetaGenerator;

internal class VariableProvider : IVariableProvider
{
    private readonly IUserInputProvider _userInputProvider;

    private readonly Dictionary<string, string> _variables = new();

    public VariableProvider(IUserInputProvider userInputProvider)
    {
        _userInputProvider = userInputProvider;
    }

    public void RegisterValue(string variable, string value)
    {
        if (_variables.ContainsKey(variable))
        {
            throw new Exception("Variable is already registered");
        }

        _variables[variable] = value;
    }

    public Dictionary<string, string> GetVariableValues()
        => _userInputProvider
            .GetVariableValues()
            .Concat(_variables)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
}
