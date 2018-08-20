namespace ComposeFX.Maths.Tests
{
	using System;
    using System.Linq;
    using ComposeFX.Maths;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
    using LinqCheck;
	using ExtensionCord;

	[TestClass]
	public class QuatTests
    {
		static QuatTests ()
        {
			Arbitrary.Register (ArbitraryQuat<Quat, float> ());
        }

		public static Arbitrary<Q> ArbitraryQuat<Q, T> () 
			where Q : struct, IQuat<Q, T>
            where T : struct, IEquatable<T>
        {
            var arb = Arbitrary.Get<T> (); 
			var quat = default (Q);

			return new Arbitrary<Q> ( 
				from a in arb.Generate.FixedArrayOf (4)
				select quat.FromAxisAngle (a[0], a[1], a[2], a[3]));
        }

		public void CheckMultWithIdentity<Q, T> () 
			where Q : struct, IQuat<Q, T>
			where T : struct, IEquatable<T>
        {
			(from quat in Prop.ForAll<Q> ()
			 let ident = quat.Identity
			 select new { quat, ident })
			.Check (p => p.quat.Multiply (p.ident).Equals (p.quat),
				label: $"{typeof (Q).Name}: quat * ident = quat")
			.Check (p => p.quat.IsNormalized,
				label: $"{typeof (Q).Name}: | quat | = 1");
        }

		public void CheckMultiplication<Q, T> () 
			where Q : struct, IQuat<Q, T>
			where T : struct, IEquatable<T>
		{
			(from quat1 in Prop.ForAll<Q> ()
			 from quat2 in Prop.ForAll<Q> ()
			 let prod = quat1.Multiply (quat2).Normalized
			 select new { quat1, quat2, prod })
			.Check (p => p.prod.IsNormalized,
				label: $"{typeof (Q).Name}: | quat1 * quat2 | / len = 1");
		}

		public void CheckRotatingVec<Q, T, V> ()
			where Q : struct, IQuat<Q, T>
			where T : struct, IEquatable<T>
			where V : struct, IVec<V, T>
		{
			(from quat in Prop.ForAll<Q> ()
			 from vec in Prop.ForAll<V> ()
			 let vecLen = vec.Length
			 let rotVec = quat.RotateVec (vec)
			 let rotVecLen = rotVec.Length
			 select new { quat, vec, vecLen, rotVec, rotVecLen })
			.Check (p => p.vecLen.ApproxEquals (p.rotVecLen),
				label: $"{typeof (Q).Name}: | quat * vec | = | vec |");
		}

		public void CheckMatrixConversion<Q> ()
			where Q : struct, IQuat<Q, float>
		{
			(from quat in Prop.ForAll<Q> ()
			 from vec in Prop.ForAll<Vec3> ()
			 let vecLen = vec.Length
			 let mat = quat.ToMatrix ()
			 let transVec = mat.Multiply (vec)
			 let transVecLen = transVec.Length
			 select new { quat, vec, vecLen, mat, transVec, transVecLen })
			.Check (p => p.vecLen.ApproxEquals (p.transVecLen),
				label: $"{typeof (Q).Name}: quat = mat => | mat * vec | = | vec |");
		}

		public void CheckLerping<Q> (Func<Q, Q, float, Q> lerpFunc)
			where Q : struct, IQuat<Q, float>
		{
			(from quat1 in Prop.ForAll<Q> ()
			 from quat2 in Prop.ForAll<Q> ()
			 from alpha in Prop.Any (Gen.ChooseDouble (0.0, 1.0).ToFloat ())
			 let lerp = lerpFunc (quat1, quat2, alpha)
			 let len = lerp.Length
			 select new { quat1, quat2, alpha, lerp, len })
			.Check (p => p.lerp.IsNormalized,
				label: $"{typeof (Q).Name}: | lerp (quat1, quat2) | = 1");
		}

		[TestMethod]
		public void TestMultiplication ()
        {
			CheckMultWithIdentity<Quat, float> ();
			CheckMultiplication<Quat, float> ();
        }

		[TestMethod]
		public void TestMatrixConversion ()
		{
			CheckMatrixConversion<Quat> ();
		}

		[TestMethod]
		public void TestVecRotation ()
		{
			CheckRotatingVec<Quat, float, Vec3> ();
		}

		[TestMethod]
		public void TestLerping ()
		{
			CheckLerping<Quat> ((q1, q2, a) => q1.Lerp (q2, a));
			CheckLerping<Quat> ((q1, q2, a) => q1.Slerp (q2, a));
		}
	}
}
