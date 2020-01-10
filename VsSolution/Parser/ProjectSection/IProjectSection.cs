using System.Text;
using apophis.Lexer;
using Messerli.VsSolution.Model;

namespace Messerli.VsSolution.Parser.ProjectSection
{
    public interface IProjectSection
    {
        void Parse(TokenWalker tokenWalker, Project project);

        void Serialize(Project project, StringBuilder result);

        bool Exists(Project project);
    }
}