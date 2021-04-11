using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Mono.Cecil
{
    internal static class Utilities
    {
        public static bool IsType<T>(this TypeReference type) => type.IsType(typeof(T));

        public static bool IsType(this TypeReference type, Type targetType) =>
            type.GetAssemblyQualifiedName() == targetType.GetAssemblyQualifiedName() ||
            type.FullName == targetType.FullName &&
            type.Resolve().GetAssemblyQualifiedName() == targetType.GetAssemblyQualifiedName();

        public static bool IsType(this TypeReference type, TypeReference targetType) =>
            type.GetAssemblyQualifiedName() == targetType.GetAssemblyQualifiedName();

        public static bool IsType(this TypeReference type, string assemblyQualifiedName) =>
            assemblyQualifiedName == type.GetAssemblyQualifiedName();

        public static string GetAssemblyQualifiedName(this TypeReference type)
        {
            if (!type.IsGenericInstance) return $"{type.FullName}, {GetAssemblyName(type.Scope.MetadataScopeType == MetadataScopeType.AssemblyNameReference ? type.Scope.Name : type.Module.Assembly.Name.Name)}";

            var genericInstance = (GenericInstanceType)type;

            return $"{type.Namespace}.{type.Name}, {GetAssemblyName(type.Scope.MetadataScopeType == MetadataScopeType.AssemblyNameReference ? type.Scope.Name : type.Module.Assembly.Name.Name)}[[{string.Join("], [", genericInstance.GenericArguments.Select(GetAssemblyQualifiedName).ToArray())}]]";
        }

        private static string GetAssemblyName(string assemblyName) => assemblyName == "System.Private.CoreLib" ? "System.Runtime" : assemblyName;

        private static string GetAssemblyQualifiedName(this Type type) =>
            type.IsConstructedGenericType
                ? $"{type.Namespace}.{type.Name}[[{string.Join("], [", type.GetGenericArguments().Select(GetAssemblyQualifiedName).ToArray())}]]"
                : $"{type.FullName}, {GetAssemblyName(type.Assembly.GetName().Name)}";

        public static TypeReference? GetUnderlyingType(this TypeReference type)
        {
            if (!type.IsGenericInstance) return null;

            return type.GetElementType().IsType(typeof(Nullable<>))
                ? ((GenericInstanceType)type).GenericArguments[0]
                : null;
        }

        public static CustomAttribute? GetCustomAttribute(this ICustomAttributeProvider provider, string attributeType) =>
            provider.CustomAttributes.FirstOrDefault(attr => attr.AttributeType.IsType(attributeType));

        [return: NotNullIfNotNull("defaultValue")]
        public static T? FirstValue<T>(this Collection<CustomAttributeNamedArgument> args, string name, T? defaultValue)
        {
            var arg = args.FirstOrDefault(a => a.Name == name);

            return arg.Argument.Value == null ? defaultValue : (T?)arg.Argument.Value;
        }

        [return: NotNullIfNotNull("defaultValue")]
        public static T? FirstValue<T>(this Collection<CustomAttributeArgument> args, int index, T? defaultValue)
        {
            var arg = args.ElementAtOrDefault(index);

            return arg.Value == null ? defaultValue : (T?)arg.Value;
        }

        public static FieldDefinition DefineField(this TypeDefinition type, string fieldName, Type fieldType, FieldAttributes attributes)
        {
            var filed = new FieldDefinition(fieldName, attributes, type.Module.ImportReference(fieldType));

            type.Fields.Add(filed);

            return filed;
        }

        public static MethodReference? GetMethod(this TypeReference type, Func<MethodDefinition, bool> predicate)
        {
            var method = type.Resolve().Methods.FirstOrDefault(predicate);

            return method == null ? null : type.Module.ImportReference(method).CopyTo(type);
        }

        public static MethodReference? GetMethod(this TypeReference type, string methodName)
        {
            var method = type.Resolve().GetMethod(methodName);

            return method == null ? null : type.Module.ImportReference(method).CopyTo(type);
        }

        public static MethodReference CopyTo(this MethodReference method, TypeReference type)
        {
            var copy = new MethodReference(method.Name, method.ReturnType, type);

            foreach (var p in method.Parameters) copy.Parameters.Add(p);

            copy.CallingConvention = method.CallingConvention;

            copy.ExplicitThis = method.ExplicitThis;
            copy.HasThis = method.HasThis;

            foreach (var p in method.GenericParameters) copy.GenericParameters.Add(p);

            return copy;
        }

        public static MethodDefinition? GetMethod(this TypeDefinition type, string methodName) =>
            type.Methods.FirstOrDefault(m => m.Name == methodName);

        public static TypeDefinition DefineType(this ModuleDefinition module, string fullName, TypeAttributes attributes, TypeReference baseType)
        {
            var index = fullName.LastIndexOf('.');

            var type = new TypeDefinition(index < 0 ? "" : fullName.Substring(0, index), index < 0 ? fullName : fullName.Substring(index + 1), attributes, baseType);

            module.Types.Add(type);

            return type;
        }

        public static Collection<GenericParameter> DefineGenericParameters(this TypeReference type, params string[] names)
        {
            foreach (var name in names)
                type.GenericParameters.Add(new GenericParameter(name, type));

            return type.GenericParameters;
        }

        // ReSharper disable once InconsistentNaming
        public static ILProcessor GetILGenerator(this MethodDefinition method) => method.Body.GetILProcessor();

        public static FieldDefinition DefineField(this TypeDefinition type, string name, TypeReference fieldType, FieldAttributes attributes)
        {
            var field = new FieldDefinition(name, attributes, fieldType);

            type.Fields.Add(field);

            return field;
        }

        public static MethodDefinition DefineMethod(this TypeDefinition type, string name, MethodAttributes attributes, TypeReference returnType, TypeReference[] parameters)
        {
            var method = new MethodDefinition(name, attributes, returnType);

            type.Methods.Add(method);

            foreach (var parameter in parameters)
                method.Parameters.Add(new ParameterDefinition(parameter));

            return method;
        }

        public static TypeReference[] GetGenericArguments(this TypeReference type) => ((GenericInstanceType)type).GenericArguments.ToArray();

        public static ParameterDefinition DefineParameter(this MethodDefinition method, int position, ParameterAttributes attributes, string name)
        {
            var parameter = method.Parameters[position - 1];

            parameter.Attributes = attributes;
            parameter.Name = name;

            return parameter;
        }

        public static ParameterDefinition[] GetParameters(this MethodReference method) => method.Parameters.ToArray();

        public static void DeclareLocal(this ILProcessor il, Type type) =>
            il.Body.Variables.Add(new VariableDefinition(il.Body.Method.Module.ImportReference(type)));

        public static void DeclareLocal(this ILProcessor il, TypeReference type) =>
            il.Body.Variables.Add(new VariableDefinition(il.Body.Method.Module.ImportReference(type)));

        public static void MarkLabel(this ILProcessor il, Instruction instruction) => il.Append(instruction);

        public static Instruction DefineLabel(this ILProcessor il) => il.Create(OpCodes.Nop);

        public static MethodReference? GetGetMethod(this TypeReference type, string property) =>
            type.GetMethod("get_" + property);
    }
}
