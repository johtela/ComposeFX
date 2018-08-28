namespace ComposeFX.SpirV
{
	using System;
	using System.Linq;
	using System.Text;

	public abstract class Operand
	{
		public virtual ushort WordCount => 1;

		public class CapabilityOperand : Operand
		{
			public Capability Capability { get; set; }
		}

		public class IdOperand : Operand
		{
			public uint Id { get; set; }
		}

		public class StringOperand : Operand
		{
			public string Literal { get; set; }

			public override ushort WordCount =>
				(ushort)((Encoding.UTF8.GetByteCount (Literal) + 3) / 4);
		}

		public class AddressingModelOperand : Operand
		{
			public AddressingModel Model { get; set; }
		}

		public class MemoryModelOperand : Operand
		{
			public MemoryModel Model { get; set; }
		}

		public class ExecutionModelOperand : Operand
		{
			public ExecutionModel Model { get; set; }
		}
	}

	public class Opcode
	{
		public ushort WordCount
		{
			get
			{
				var type = Type == null ? 0 : 1;
				var result = ResultId == 0 ? 0 : 1;
				var opers = Operands.Sum (o => o.WordCount);
				return (ushort)(1 + type + result + opers);
			}
		}

		public Op Operation { get; private set; }
		public Opcode Type { get; private set; }
		public uint ResultId { get; private set; }
		public Operand[] Operands { get; private set; }
	}
}
