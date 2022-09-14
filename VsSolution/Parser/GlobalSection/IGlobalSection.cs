using System.Text;
using Messerli.Lexer;
using Messerli.VsSolution.Model;

namespace Messerli.VsSolution.Parser.GlobalSection;

internal interface IGlobalSection
{
    void Parse(TokenWalker tokenWalker, Solution solution);

    void Serialize(Solution solution, StringBuilder result);

    bool Exists(Solution solution);
}
