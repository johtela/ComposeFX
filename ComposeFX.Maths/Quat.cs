﻿namespace ComposeFX.Maths
{
	using ExtensionCord;

	public readonly struct Quat : IQuat<Quat, float>
	{
		private const float LERP_THRESHOLD = 0.99f;

		public readonly Vec3 Uvec;
		public readonly float W;
		public static readonly Quat Identity = new Quat (new Vec3 (0f), 1f);

		public Quat (in Vec3 vec, float w)
		{
			Uvec = vec;
			W = w;
		}

		public Quat (float x, float y, float z, float w)
		{
			Uvec = new Vec3 (x, y, z);
			W = w;
		}

		public Vec4 ToVec4 ()
		{
			return new Vec4 (Uvec.X, Uvec.Y, Uvec.Z, W);
		}

		public static Quat FromVec4 (in Vec4 vec)
		{
			return new Quat (vec.X, vec.Y, vec.Z, vec.W);
		}

		public static Quat FromAxisAngle (in Vec3 axis, float angle)
		{
			var lensqr = axis.LengthSquared;
			if (angle == 0f || lensqr == 0f)
				return Identity;

			var normaxis = lensqr == 1f ? axis : axis / lensqr.Sqrt ();
			var halfangle = angle / 2;
			return new Quat (normaxis * halfangle.Sin (), halfangle.Cos ());
		}

		public V ToVector<V> () where V : struct, IVec<V, float>
		{
			return Vec.FromArray<V, float> (Uvec.X, Uvec.Y, Uvec.Z, W);
		}

		public Mat3 ToMatrix ()
		{
			var xx = Uvec.X * Uvec.X;
			var xy = Uvec.X * Uvec.Y;
			var xz = Uvec.X * Uvec.Z;
			var xw = Uvec.X * W;
			var yy = Uvec.Y * Uvec.Y;
			var yz = Uvec.Y * Uvec.Z;
			var yw = Uvec.Y * W;
			var zz = Uvec.Z * Uvec.Z;
			var zw = Uvec.Z * W;

			return new Mat3 (
				1 - 2 * (yy + zz), 2 * (xy - zw), 2 * (xz + yw),
				2 * (xy + zw), 1 - 2 * (xx + zz), 2 * (yz - xw),
				2 * (xz - yw), 2 * (yz + xw), 1 - 2 * (xx + yy));
		}

		public Quat Invert ()
		{
			return new Quat (Uvec, -W);
		}

		public Quat Conjugate ()
		{
			return new Quat (-Uvec, W);
		}

		public Quat Multiply (in Quat other)
		{
			return new Quat (other.W * Uvec + W * other.Uvec + Vec.Cross (in Uvec, in other.Uvec),
				W * other.W - Uvec.Dot (in other.Uvec));
		}

		public V RotateVec<V> (in V vec) where V : struct, IVec<V, float>
		{
			return (this * new Quat (vec[0], vec[1], vec[2], 0f) * Conjugate ()).ToVector<V> ();
		}

		public Vec3 RotateVec3 (in Vec3 vec)
		{
			return (this * new Quat (vec, 0f) * Conjugate ()).Uvec;
		}

		public Quat Lerp (in Quat other, float interPos)
		{
			return FromVec4 (Vec.Mix (ToVec4 (), other.ToVec4 (), interPos).Normalized);
		}

		public Quat Slerp (in Quat other, float interPos)
		{
			var v1 = ToVec4 ();
			var v2 = other.ToVec4 ();
			var dot = v1.Dot (in v2);
			if (dot > LERP_THRESHOLD)
				return FromVec4 (Vec.Mix (in v1, in v2, interPos));

			var theta = dot.Acos () * interPos;
			var v3 = (v2 - v1 * dot).Normalized;
			return FromVec4 (v1 * theta.Cos () + v3 * theta.Sin ());
		}

		public float Length
		{
			get { return LengthSquared.Sqrt (); }
		}

		public float LengthSquared
		{
			get { return Uvec.LengthSquared + W * W; }
		}

		public bool IsNormalized
		{
			get { return LengthSquared.ApproxEquals (1f, 0.001f); }
		}

		public Quat Normalized
		{
			get 
			{ 
				var len = Length;
				return new Quat (Uvec / len, W / len);
			}
		}

		public static implicit operator Vec4 (in Quat quat)
		{
			return quat.ToVec4 ();
		}

		public static implicit operator Quat (in Vec4 vec)
		{
			return FromVec4 (vec);
		}

		public static Quat operator - (in Quat quat)
		{
			return quat.Invert ();
		}

		public static Quat operator * (in Quat left, in Quat right)
		{
			return left.Multiply (right);
		}

		public static bool operator == (in Quat left, in Quat right)
		{
			return left.Equals (right);
		}

		public static bool operator != (in Quat left, in Quat right)
		{
			return !left.Equals (right);
		}

		public override bool Equals (object obj)
		{
			return obj is Quat && Equals ((Quat)obj);
		}

		public override int GetHashCode ()
		{
			return Uvec.GetHashCode () ^ W.GetHashCode ();
		}

		public override string ToString ()
		{
			return string.Format ("[ {0} {1} ]", Uvec, W);
		}

		#region IEquatable implementation

		public bool Equals (Quat other)
		{
			return Uvec == other.Uvec && W == other.W;
		}

		#endregion

		#region IQuat<Quat, float> implementation

		Quat IQuat<Quat, float>.FromAxisAngle (float x, float y, float z, float angle)
		{
			return FromAxisAngle (new Vec3 (x, y, z), angle);
		}

		Quat IQuat<Quat, float>.Identity 
		{
			get { return Identity; }
		}

		#endregion
	}
}

