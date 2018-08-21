namespace ComposeFX.Maths
{
	using Compute;
	using Graphics;

	public static class Convert
	{
		public static void TestIL ()
		{
			var pos = new Vec2 ();
			if (pos.X < -1 || pos.X > 1 || pos.Y < -1 || pos.Y > 1)
			{
				pos = Vec.Clamp (pos, new Vec2 (-1f, -1f), new Vec2 (1f, 1f));
			}
		}

		[GLFunction ("ivec2 ({0})")]
		[CLFunction ("convert_int2 ({0})")]
		public static Vec2i ToVeci (this Vec2 vec)
		{
			return new Vec2i ((int)vec.X, (int)vec.Y);
		}

		[GLFunction ("ivec3 ({0})")]
		[CLFunction ("convert_int3 ({0})")]
		public static Vec3i ToVeci (this Vec3 vec)
		{
			return new Vec3i ((int)vec.X, (int)vec.Y, (int)vec.Z);
		}

		[GLFunction ("ivec4 ({0})")]
		[CLFunction ("convert_int4 ({0})")]
		public static Vec4i ToVeci (this Vec4 vec)
		{
			return new Vec4i ((int)vec.X, (int)vec.Y, (int)vec.Z, (int)vec.W);
		}

		[GLFunction ("vec2 ({0})")]
		[CLFunction ("convert_float2 ({0})")]
		public static Vec2 ToVeci (this Vec2i vec)
		{
			return new Vec2 (vec.X, vec.Y);
		}

		[GLFunction ("vec3 ({0})")]
		[CLFunction ("convert_float3 ({0})")]
		public static Vec3 ToVec (this Vec3i vec)
		{
			return new Vec3 (vec.X, vec.Y, vec.Z);
		}

		[GLFunction ("vec4 ({0})")]
		[CLFunction ("convert_float4 ({0})")]
		public static Vec4 ToVec (this Vec4i vec)
		{
			return new Vec4 (vec.X, vec.Y, vec.Z, vec.W);
		}
	}
}
