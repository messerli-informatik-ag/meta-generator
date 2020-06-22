using System;
using Funcky.Monads;

namespace Messerli.MetaGenerator.UserInput
{
    internal static class Utility
    {
        public static TResult Retry<TResult>(Func<Option<TResult>> producer)
            where TResult : notnull
            => producer().GetOrElse(() => Retry(producer));
    }
}
