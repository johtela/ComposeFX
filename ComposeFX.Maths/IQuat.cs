namespace ComposeFX.Maths
{
	using System;

	public interface IQuat<Q, T> : IEquatable<Q>
		where Q : struct, IQuat<Q, T>
		where T : struct, IEquatable<T>
	{
		Q Invert ();
		Q Conjugate ();
		Q Multiply (in Q other);
		Q FromAxisAngle (T x, T y, T z, T angle);
		V ToVector<V> () where V : struct, IVec<V, T>;
		Mat3 ToMatrix ();
		V RotateVec<V> (in V vec) where V : struct, IVec<V, T>;
		Q Lerp (in Q other, T interPos);
		Q Slerp (in Q other, T interPos);

		Q Identity { get; }
		T Length { get; }
		T LengthSquared { get; }
		bool IsNormalized { get; }
		Q Normalized { get; }
	}
}