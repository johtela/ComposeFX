namespace ComposeFX.Compiler.Tests
{
	using ComposeFX.Maths;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using System.Runtime.InteropServices;

	[StructLayout (LayoutKind.Sequential, Pack = 4)]
	public struct PositionalVertex
	{
		public Vec3 position;
		public Vec3 normal;

		public override string ToString ()
		{
			return string.Format ("[Vertex: Position={0}, Normal={1}]", position, normal);
		}
	}

	[TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
