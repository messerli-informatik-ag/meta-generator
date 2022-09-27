using System.Text;
using Messerli.Lexer;
using Messerli.VsSolution.Model;

namespace Messerli.VsSolution.Parser.ProjectSection;

internal interface IProjectSection
{
    void Parse(TokenWalker tokenWalker, Project project);

    void Serialize(Project project, StringBuilder result);

    bool Exists(Project project);
}
