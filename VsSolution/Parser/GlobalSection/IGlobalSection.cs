using System.Text;
using apophis.Lexer;
using Messerli.VsSolution.Model;

namespace Messerli.VsSolution.Parser.GlobalSection
{
    public interface IGlobalSection
    {
        void Parse(TokenWalker tokenWalker, Solution solution);

        void Serialize(Solution solution, StringBuilder result);

        bool Exists(Solution solution);
    }
}