using System;
using System.Globalization;

namespace Messerli.MetaGenerator.Test;

internal sealed class TemporaryCultureSwitch : IDisposable
{
    private readonly CultureInfo _lastCulture;

    public TemporaryCultureSwitch(string newCulture)
    {
        _lastCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo(newCulture);
    }

    public void Dispose()
        => CultureInfo.CurrentCulture = _lastCulture;
}
