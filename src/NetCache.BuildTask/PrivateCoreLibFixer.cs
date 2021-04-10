using Mono.Cecil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NetCache
{
    internal struct PrivateCoreLibFixer
    {
        private static readonly AssemblyNameReference SystemRuntimeRef;

        static PrivateCoreLibFixer()
        {
            var systemRuntime = AppDomain.CurrentDomain.GetAssemblies().Single(mr => mr.GetName().Name == "System.Runtime");

            // in most platforms, referencing System.Object and other types ends up adding a reference to System.Private.CoreLib (note that in these platforms, System.Runtime has type forwarders for these types).
            // To avoid this reference to System.Private.CoreLib we update these types to pretend they come from System.Runtime instead.
            SystemRuntimeRef = new AssemblyNameReference(systemRuntime.GetName().Name, systemRuntime.GetName().Version)
            {
                PublicKeyToken = new byte[] { 0xb0, 0x3f, 0x5f, 0x7f, 0x11, 0xd5, 0x0a, 0x3a }
            };
        }

        /// <summary>Changes types referencing mscorlib so they appear to be defined in System.Runtime.dll</summary>
        /// <param name="mainModule">module which assembly references will be added to/removed from</param>
        public static void FixReferences(ModuleDefinition mainModule)
        {
            foreach (var t in mainModule.GetAllTypes())
                FixType(t, mainModule);

            foreach (var tbr in mainModule.AssemblyReferences.Where(a => a.Name == "mscorlib" || a.Name == "System.Private.CoreLib").ToArray())
                mainModule.AssemblyReferences.Remove(tbr);
        }

        private static void FixType(TypeDefinition type, ModuleDefinition mainModule)
        {
            FixTypeReferences(type.BaseType, mainModule);

            FixAttributes(type.CustomAttributes, mainModule);

            foreach (var field in type.Fields)
                FixTypeReferences(field.FieldType.GetElementType(), mainModule);

            foreach (var property in type.Properties)
            {
                FixTypeReferences(property.PropertyType.GetElementType(), mainModule);

                FixParameters(property.Parameters, mainModule);
            }

            foreach (var method in type.Methods)
                FixTypeReferences(method, mainModule);

            foreach (var @event in type.Events)
                FixTypeReferences(@event.EventType.GetElementType(), mainModule);
        }

        private static void FixTypeReferences(MethodReference method, ModuleDefinition mainModule)
        {
            FixTypeReferences(method.ReturnType.GetElementType(), mainModule);
            //FixTypeReference(method.MethodReturnType., mainModule);
            FixParameters(method.Parameters, mainModule);

            TryFixTypeReferencesInGenericInstance(method, mainModule);
        }

        private static void FixAttributes(Collection<CustomAttribute> customAttributes, ModuleDefinition mainModule)
        {
            foreach (var attribute in customAttributes)
            {
                FixTypeReferences(attribute.AttributeType.GetElementType(), mainModule);
                FixTypeReferences(attribute.Constructor, mainModule);
                FixTypeReferences(attribute.Fields, mainModule);
                FixTypeReferences(attribute.Properties, mainModule);
                FixTypeReferences(attribute.ConstructorArguments, mainModule);
            }
        }

        private static void FixTypeReferences(Collection<CustomAttributeArgument> attributeConstructorArguments, ModuleDefinition mainModule)
        {
            foreach (var constructorArgument in attributeConstructorArguments)
                FixTypeReferences(constructorArgument, mainModule);
        }

        private static void FixTypeReferences(Collection<CustomAttributeNamedArgument> attributeFields, ModuleDefinition mainModule)
        {
            foreach (var attributeField in attributeFields)
                FixTypeReferences(attributeField.Argument, mainModule);
        }

        private static void FixTypeReferences(CustomAttributeArgument customAttributeArgument, ModuleDefinition mainModule)
        {
            FixTypeReferences(customAttributeArgument.Type.GetElementType(), mainModule);

            if (customAttributeArgument.Value is TypeReference t) FixTypeReferences(t, mainModule);
        }

        private static void FixParameters(Collection<ParameterDefinition> parameters, ModuleDefinition mainModule)
        {
            foreach (var parameter in parameters)
                FixTypeReferences(parameter.ParameterType.GetElementType(), mainModule);
        }

        private static void FixTypeReferences(TypeReference? t, ModuleDefinition mainModule)
        {
            if (t == null) return;

            if (t.Scope.Name == "mscorlib" || t.Scope.Name == "System.Private.CoreLib")
            {
                if (mainModule.AssemblyReferences.All(a => a.Name != SystemRuntimeRef.Name)) mainModule.AssemblyReferences.Add(SystemRuntimeRef);

                if (t is not GenericInstanceType) t.Scope = SystemRuntimeRef;
            }


            if (t is ICustomAttributeProvider customAttributeProvider)
                FixAttributes(customAttributeProvider.CustomAttributes, mainModule);

            TryFixTypeReferencesInGenericInstance(t, mainModule);
        }

        private static void TryFixTypeReferencesInGenericInstance(MemberReference memberReference, ModuleDefinition mainModule)
        {
            if (memberReference is not IGenericInstance gi) return;

            foreach (var genericArgument in gi.GenericArguments)
                FixTypeReferences(genericArgument.GetElementType(), mainModule);

            if (gi is not GenericInstanceType git) return;

            foreach (var genericParameter in git.GenericParameters)
            {
                FixTypeReferences(genericParameter.GetElementType(), mainModule);

                FixTypeReferences(genericParameter.Constraints, mainModule);
            }

            FixTypeReferences(git.ElementType, mainModule);
        }

        private static void FixTypeReferences(IEnumerable<GenericParameterConstraint> genericConstraints, ModuleDefinition mainModule)
        {
            foreach (var genericConstraint in genericConstraints)
            {
                FixTypeReferences(genericConstraint.ConstraintType.GetElementType(), mainModule);

                FixAttributes(genericConstraint.CustomAttributes, mainModule);
            }
        }
    }
}
