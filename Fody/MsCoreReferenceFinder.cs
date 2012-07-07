using System.Linq;
using Mono.Cecil;

public class MsCoreReferenceFinder
{
    ModuleWeaver moduleWeaver;
    IAssemblyResolver assemblyResolver;
    public MethodReference EditorBrowsableConstructor;
    public TypeDefinition EditorBrowsableStateType;
    public int AdvancedStateConstant;

    public MsCoreReferenceFinder(ModuleWeaver moduleWeaver, IAssemblyResolver assemblyResolver)
    {
        this.moduleWeaver = moduleWeaver;
        this.assemblyResolver = assemblyResolver;
    }


    public void Execute()
    {
        var assemblyDefinition = assemblyResolver.Resolve("System");
        var msCoreTypes = assemblyDefinition.MainModule.Types;

        var attribyteType = msCoreTypes.First(x => x.Name == "EditorBrowsableAttribute");
        EditorBrowsableConstructor = moduleWeaver.ModuleDefinition.Import(attribyteType.Methods.First(IsDesiredConstructor));
        EditorBrowsableStateType = msCoreTypes.First(x => x.Name == "EditorBrowsableState");
        var fieldDefinition = EditorBrowsableStateType.Fields.First(x => x.Name == "Advanced");
        AdvancedStateConstant =(int) fieldDefinition.Constant;
    }

    static bool IsDesiredConstructor(MethodDefinition x)
    {
        if (!x.IsConstructor)
        {
            return false;
        }
        if (x.Parameters.Count != 1)
        {
            return false;
        }
        return x.Parameters[0].ParameterType.Name == "EditorBrowsableState";
    }

}