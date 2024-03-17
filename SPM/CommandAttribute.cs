namespace SPM;

[AttributeUsage(AttributeTargets.Method)]
public class CommandAttribute : Attribute
{
    public string CommandName { get; set; } = "";
    public string Usage { get; set; } = "";
}