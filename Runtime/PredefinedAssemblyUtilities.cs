using System;
using System.Collections.Generic;
using System.Reflection;

namespace UnityEssentials
{
    /// <summary>
    /// Utility class for working with predefined Unity assemblies and extracting types based on interface implementation.
    /// </summary>
    public static class PredefinedAssemblyUtilities
    {
        /// <summary>
        /// Enum representing commonly known Unity assemblies.
        /// </summary>
        public enum AssemblyType
        {
            AssemblyCSharpFirstPass,
            AssemblyCSharpEditorFirstPass,
            AssemblyCSharp,
            AssemblyCSharpEditor,
        }

        /// <summary>
        /// Maps an assembly name string to a predefined AssemblyType enum value.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly.</param>
        /// <returns>The corresponding AssemblyType if known; otherwise, null.</returns>
        public static AssemblyType? GetAssemblyType(string assemblyName) =>
            assemblyName switch
            {
                "Assembly-CSharp-firstpass" => AssemblyType.AssemblyCSharpFirstPass,
                "Assembly-CSharp-Editor-firstpass" => AssemblyType.AssemblyCSharpEditorFirstPass,
                "Assembly-CSharp" => AssemblyType.AssemblyCSharp,
                "Assembly-CSharp-Editor" => AssemblyType.AssemblyCSharpEditor,
                _ => null
            };

        /// <summary>
        /// Searches all known Unity runtime assemblies for types implementing the given interface type.
        /// </summary>
        /// <param name="interfaceType">The interface type to search for.</param>
        /// <returns>A list of types that implement the given interface.</returns>
        public static List<Type> GetTypes(Type interfaceType)
        {
            // 1. Get all loaded assemblies
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // 2. Filter predefined Unity assemblies
            var assemblyTypes = FilterAssemblies(assemblies);

            // 3. Check type implementations
            List<Type> types = new();
            AddTypesFromAssembly(assemblyTypes[AssemblyType.AssemblyCSharp], types, interfaceType);
            AddTypesFromAssembly(assemblyTypes[AssemblyType.AssemblyCSharpFirstPass], types, interfaceType);

            return types;
        }

        /// <summary>
        /// Filters the provided assemblies and categorizes them by a custom AssemblyType enum.
        /// Only assemblies that can be classified with a known AssemblyType are included.
        /// </summary>
        /// <param name="assemblies">An array of assemblies to filter.</param>
        /// <returns>
        /// A dictionary mapping each recognized AssemblyType to the array of types defined in its corresponding assembly.
        /// </returns>
        private static Dictionary<AssemblyType, Type[]> FilterAssemblies(Assembly[] assemblies)
        {
            var result = new Dictionary<AssemblyType, Type[]>();

            foreach (var assembly in assemblies)
            {
                var type = GetAssemblyType(assembly.GetName().Name);
                if (type.HasValue)
                    result[type.Value] = assembly.GetTypes();
            }

            return result;
        }

        /// <summary>
        /// Adds types from the given assembly array to the result list if they implement the given interface.
        /// </summary>
        /// <param name="assembly">The array of types from an assembly.</param>
        /// <param name="types">The list to populate with matching types.</param>
        /// <param name="interfaceType">The interface type to match against.</param>
        private static void AddTypesFromAssembly(Type[] assembly, ICollection<Type> types, Type interfaceType)
        {
            foreach (var type in assembly)
                if (type != interfaceType && interfaceType.IsAssignableFrom(type))
                    types.Add(type);
        }
    }
}
