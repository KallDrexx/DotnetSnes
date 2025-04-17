using System;
using System.Collections.Generic;
using System.Linq;
using Dntc.Attributes;
using Dntc.Common;
using Dntc.Common.Definitions;
using Dntc.Common.Definitions.Definers;
using Dntc.Common.Syntax.Statements;
using Mono.Cecil;

namespace DotnetSnes.Core.TranspilerSupport;

/// <summary>
/// Designates a static field as a label to an assembly location.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class AssemblyLabelAttribute(string labelName) : Attribute
{
    public class Definer : IDotNetFieldDefiner
    {
        public DefinedField? Define(FieldDefinition field)
        {
            // If the field has the AssemblyLabelFieldAttribute, then convert it
            // to a custom declaration field attribute instead so the built-in
            // definers can handle it.
            var attribute = field.CustomAttributes
                .SingleOrDefault(x => x.AttributeType.FullName == typeof(AssemblyLabelAttribute).FullName);

            if (attribute == null)
            {
                return null;
            }

            var label = attribute.ConstructorArguments[0].Value.ToString()!;
            var declaration = $"extern char {label}";
            var declaringNamespace = Dntc.Common.Utils.GetNamespace(field.DeclaringType);

            return new CustomDeclaredFieldDefinition(
                field,
                declaration,
                Dntc.Common.Utils.GetHeaderName(declaringNamespace),
                Dntc.Common.Utils.GetSourceFileName(declaringNamespace),
                new CFieldName(label),
                new IlFieldId(field.FullName),
                new IlTypeName(field.FieldType.FullName),
                field.IsStatic,
                []);
        }
    }

    private class CustomDeclaredFieldDefinition : CustomDefinedField
    {
        private readonly string _declaration;

        public CustomDeclaredFieldDefinition(
            FieldDefinition originalField,
            string declaration,
            HeaderName? declaredInHeader,
            CSourceFileName? declaredInSourceFileName,
            CFieldName nativeName,
            IlFieldId name,
            IlTypeName type,
            bool isGlobal,
            IReadOnlyList<HeaderName>? referencedHeaders = null)
            : base(originalField, declaredInHeader, declaredInSourceFileName, nativeName, name, type, isGlobal,
                referencedHeaders)
        {
            _declaration = declaration;
        }

        public override CustomCodeStatementSet GetCustomDeclaration()
        {
            return new CustomCodeStatementSet(_declaration);
        }
    }
}