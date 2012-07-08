﻿using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Collections.Generic;

public class TypeProcessor
{
    ModuleWeaver moduleWeaver;
    MsCoreReferenceFinder msCoreReferenceFinder;

    public TypeProcessor(ModuleWeaver moduleWeaver, MsCoreReferenceFinder msCoreReferenceFinder)
    {
        this.moduleWeaver = moduleWeaver;
        this.msCoreReferenceFinder = msCoreReferenceFinder;
    }

    public void Execute(TypeDefinition typeDefinition)
    {
        if (IsCompilerGenerated(typeDefinition.CustomAttributes))
        {
            return;
        }
        if (typeDefinition.IsNotPublic)
        {
            if (typeDefinition.IsNested)
            {
                typeDefinition.IsNestedPublic = true;
            }
            else
            {
                typeDefinition.IsPublic = true;
            }
            AddCEditorBrowsableAttribute(typeDefinition.CustomAttributes);
        }
        if (typeDefinition.IsInterface)
        {
            return;
        }

        foreach (var method in typeDefinition.Methods)
        {
            ProcessMethod(method);
        }
        foreach (var field in typeDefinition.Fields)
        {
            ProcessField(field);
        }
    }


    void ProcessField(FieldDefinition field)
    {
        if (IsCompilerGenerated(field.CustomAttributes))
        {
            return;
        }
        if (field.IsPublic)
        {
            return;
        }
        var requiresPublicize = false;
        if (field.IsAssembly)
        {
            field.IsAssembly = false;
            requiresPublicize = true;
        }
        if (field.IsPrivate)
        {
            field.IsPrivate = false;
            requiresPublicize = true;
        }
        if (requiresPublicize)
        {
            field.IsPublic = true;
            AddCEditorBrowsableAttribute(field.CustomAttributes);
        }
    }


    static bool IsCompilerGenerated(IEnumerable<CustomAttribute> customAttributes)
    {
        return customAttributes.Any(x=>x.AttributeType.Name == "CompilerGeneratedAttribute");
    }

    void ProcessMethod(MethodDefinition method)
    {
        var requiresPublicize = false;
        if (method.IsPublic)
        {
            return;
        }
        if (method.IsAssembly)
        {
            method.IsAssembly = false;
            requiresPublicize = true;
        }
        if (method.IsHideBySig)
        {
            method.IsHideBySig = false;
            requiresPublicize = true;
        }
        if (method.IsPrivate)
        {
            method.IsPrivate = false;
            requiresPublicize = true;
        }

        if (requiresPublicize)
        {
            method.IsPublic = true;
            AddCEditorBrowsableAttribute(method.CustomAttributes);
        }
    }

    void AddCEditorBrowsableAttribute(Collection<CustomAttribute> customAttributes)
    {
        var customAttribute = new CustomAttribute(msCoreReferenceFinder.EditorBrowsableConstructor);
        customAttribute.ConstructorArguments.Add(new CustomAttributeArgument(msCoreReferenceFinder.EditorBrowsableStateType, msCoreReferenceFinder.AdvancedStateConstant));
        customAttributes.Add(customAttribute);
    }
}