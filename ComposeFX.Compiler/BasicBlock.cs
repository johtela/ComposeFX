namespace ComposeFX.Compiler
{
	using ComposeFX.SpirV;
	using System;
	using System.Collections.Generic;
	using System.Text;

	public class BasicBlock
    {
		public List<BasicBlock> Predecessors { get; }
		public List<BasicBlock> Successors { get; }
		public List<Opcode> Opcodes { get; }

		public BasicBlock ()
		{
			Predecessors = new List<BasicBlock> ();
			Successors = new List<BasicBlock> ();
		}
	}
}
