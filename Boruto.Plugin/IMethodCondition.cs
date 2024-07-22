namespace Boruto
{
    public interface IMethodCondition
    {
        bool Execute(Attributes.IfAttribute ifAttr, Microsoft.Xrm.Sdk.IPluginExecutionContext ctx);
    }
}
