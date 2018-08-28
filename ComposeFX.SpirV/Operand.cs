namespace ComposeFX.SpirV
{
	using System;
	using System.Text;

	public abstract class Operand
	{
		public virtual ushort WordCount => 1;

		public class _Id : Operand
		{
			public uint Target { get; set; }

			public override string ToString () => $"${Target}";
		}

		public class _String : Operand
		{
			public string Value { get; set; }

			public override ushort WordCount =>
				(ushort)((Encoding.UTF8.GetByteCount (Value) + 3) / 4);

			public override string ToString () => $"\"{Value}\"";
		}

		public class _Number : Operand
		{
			public int Value { get; set; }

			public override string ToString () => Value.ToString ();
		}

		public class _Enum : Operand
		{
			public Type Type { get; set; }
			public uint Value { get; set; }

			public override string ToString () => 
				System.Enum.GetName (Type, Value);
		}

		public static Operand Id (uint id) => 
			new _Id { Target = id };

		public static Operand Literal (string value) => 
			new _String { Value = value };

		public static Operand Literal (int value) => 
			new _Number { Value = value };

		public static Operand Enum (Type type, uint value) => 
			new _Enum { Type = type, Value = value };
	}
}