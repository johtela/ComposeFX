namespace ComposeFX.SpirV
{
	using System;
	using System.Linq;

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

		public override string ToString ()
		{
			var oper = Enum.GetName (typeof (Op), Operation);
			var type = Type == null ? "" : $"${Type.ResultId} ";
			var operands = Operands.Select (o => o.ToString ())
				.Aggregate ((o1, o2) => $"{o1} {o2}");
			return $"{ResultId} = {oper} {type}{operands}";
		}

		public Opcode New (uint resultId, Op operation, Opcode type,
			params Operand[] operands) =>
			new Opcode
			{
				ResultId = resultId,
				Operation = operation,
				Type = type,
				Operands = operands
			};

		public Opcode New (Op operation, Opcode type, params Operand[] operands) =>
			new Opcode
			{
				Operation = operation,
				Type = type,
				Operands = operands
			};

		public Opcode New (uint resultId, Op operation, params Operand[] operands) =>
			new Opcode
			{
				ResultId = resultId,
				Operation = operation,
				Operands = operands
			};

		public Opcode New (Op operation, params Operand[] operands) =>
			new Opcode
			{
				Operation = operation,
				Operands = operands
			};
	}
}
