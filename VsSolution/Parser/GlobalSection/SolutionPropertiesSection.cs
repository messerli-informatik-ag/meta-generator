using System;
using System.Linq;
using System.Text;
using apophis.Lexer;
using Messerli.VsSolution.Model;
using Messerli.VsSolution.Token;

namespace Messerli.VsSolution.Parser.GlobalSection
{
    internal class SolutionPropertiesSection : IGlobalSection
    {
        public void Parse(TokenWalker tokenWalker, Solution solution)
        {
            throw new NotImplementedException();
        }

        public void Serialize(Solution solution, StringBuilder result)
        {
            throw new NotImplementedException();
        }

        public bool Exists(Solution solution)
        {
            return false;
        }
    }
}
