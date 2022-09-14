namespace Messerli.MetaGeneratorAbstractions;

public readonly struct Template
{
    public readonly string TemplateName;
    public readonly string Content;

    public Template(string templateName, string content)
    {
        TemplateName = templateName;
        Content = content;
    }
}
