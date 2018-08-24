namespace ComposeFX.Compiler
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using Mono.Cecil;
	using Mono.Cecil.Cil;

	public class BasicBlock
    {
		public List<BasicBlock> Predecessors { get; }
		public List<BasicBlock> Successors { get; }

		public BasicBlock ()
		{
			Predecessors = new List<BasicBlock> ();
			Successors = new List<BasicBlock> ();
		}
	}
}
