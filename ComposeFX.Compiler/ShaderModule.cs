namespace ComposeFX.Compiler
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Mono.Cecil;
	using Mono.Cecil.Cil;
	using ComposeFX.SpirV;

	public class ShaderModule
    {


		public ShaderModule (TypeDefinition typedef)
		{
			foreach (var md in typedef.Methods)
				if (HasAttribute (md.CustomAttributes, 
					typeof (ShaderFunctionAttribute)))
				{
				}
		}

		public static IEnumerable<ShaderModule> ModulesInAssembly (
			string assemblyPath)
		{
			var assy = AssemblyDefinition.ReadAssembly (assemblyPath);
			return from t in assy.MainModule.Types
				   where HasAttribute (t.CustomAttributes, typeof (ShaderAttribute))
				   select new ShaderModule (t);
		}

		private static bool HasAttribute (ICollection<CustomAttribute> attributes,
			Type type) => 
			attributes.Any (ca => ca.AttributeType.Name == type.Name);
	}
}
 