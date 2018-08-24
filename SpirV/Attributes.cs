namespace ComposeFX.SpirV
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	[AttributeUsage (AttributeTargets.Class)]
	public class ShaderAttribute : Attribute { }

	[AttributeUsage (AttributeTargets.Method)]
	public class ShaderFunctionAttribute : Attribute { }


}
