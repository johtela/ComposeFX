namespace ComposeTester
{
	using System;
	using System.Linq;
	using ComposeFX.Maths;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using ExtensionCord;
	using LinqCheck;

	[TestClass]
	public class MatTests
    {
        static MatTests ()
        {
			VecTests.Use ();
            Arbitrary.Register (ArbitraryMat<Mat2, float> (2, 2));
            Arbitrary.Register (ArbitraryMat<Mat3, float> (3, 3));
            Arbitrary.Register (ArbitraryMat<Mat4, float> (4, 4));
        }

		// TODO: Move to ExtensionCord
		private static T[] Flatten<T> (T[,] array)
		{
			var dim1 = array.GetLength (0);
			var dim2 = array.GetLength (1);
			var len = dim1 * dim2;
			var res = new T[len];
			for (int d1 = 0, i = 0; d1 < dim1; d1++)
				for (int d2 = 0; d2 < dim2; d2++)
					res[i++] = array[d1, d2];
			return res;
		}

		private static T[,] Unflatten<T> (T[] array, int dim1, int dim2)
		{
			var len = array.Length;
			var res = new T[dim1, dim2];
			for (int d1 = 0, i = 0; d1 < dim1; d1++)
				for (int d2 = 0; d2 < dim2; d2++)
					res[d1, d2] = array[i++];
			return res;
		}

		private static M FixZerosInDiagonal<M> (M mat)
            where M : struct, ISquareMat<M, float>
		{
			var arr = mat.ToArray<M, float> ();
			for (int i = 0; i < mat.Columns; i++)
				if (arr[i, i] == 0f)
					arr[i, i] = 1f;
			return mat.FromArray (arr);
		}

		public static Arbitrary<M> ArbitraryMat<M, T> (int cols, int rows) 
            where M : struct, IMat<M, T>
            where T : struct, IEquatable<T>
        {
            var arb = Arbitrary.Get<T> (); 
            return new Arbitrary<M> ( 
                from a in arb.Generate.Fixed2DArrayOf (cols, rows)
                select Mat.FromArray<M, T> (a),
                m => from a in Flatten (m.ToArray<M, T> ())
						.Map (arb.Shrink).Combinations ()
                     select Mat.FromArray<M, T> (Unflatten (a, cols, rows)));
        }

        public void CheckAddSubtract<M> () where M : struct, IMat<M, float>
        {
			(from mat1 in Prop.ForAll<M> ()
			 from mat2 in Prop.ForAll<M> ()
			 let neg = mat2.Multiply (-1f)
			 select new { mat1, mat2, neg })
			.Check (p => p.mat1.Subtract (p.mat1).Equals (default),
				label: $"{typeof (M).Name}: mat1 - mat1 = [ 0 ... ]")
			.Check (p => p.mat1.Subtract (p.mat2).Equals (p.mat1.Add (p.neg)),
				label: $"{typeof (M).Name}: mat1 - mat2 = mat1 + (-mat2)");
        }

        public void CheckMultiplyScalar<M> () where M : struct, IMat<M, float>
        {
			(from mat in Prop.ForAll<M> ()
			 from scalar in Prop.ForAll<float> ()
			 let mult = mat.Multiply (scalar)
			 let multdiv = mult.Multiply (1 / scalar)
			 select new { mat, scalar, mult, multdiv })
			.Check (p => Mat.ApproxEquals (p.mat, p.multdiv),
				label: $"{typeof (M).Name}: (mat * scalar) * (1 / scalar) = mat");
		}

		public void CheckTranspose<M> () where M : struct, ISquareMat<M, float>
        {
			(from mat1 in Prop.ForAll<M> ()
			 from mat2 in Prop.ForAll<M> ()
			 let mat1t = mat1.Transposed
			 let mat1tt = mat1t.Transposed
			 let mat2t = mat2.Transposed
			 let add_mat1t_mat2t = mat1t.Add (mat2t)
			 let addt_mat1_mat2 = mat1.Add (mat2).Transposed
			 select new
			 {
				 mat1,
				 mat1t,
				 mat1tt,
				 mat2,
				 mat2t,
				 add_mat1t_mat2t,
				 addt_mat1_mat2
			 })
			.Check (p => p.mat1.Rows == p.mat1t.Columns && p.mat1.Columns == p.mat1t.Rows,
				label: $"{typeof (M).Name}: mat.Rows = mat^T.Columns and mat.Columns = mat^T.Rows")
			.Check (p => p.mat1.Equals (p.mat1tt),
				label: $"{typeof (M).Name}: mat^TT = mat")
			.Check (p => p.add_mat1t_mat2t.Equals (p.addt_mat1_mat2),
				label: $"{typeof (M).Name}: mat1^T + mat2^T = (mat1 + mat2)^T");
        }

        public void CheckMultiplyMatrices<M> () where M : struct, ISquareMat<M, float>
        {
			(from mat1 in Prop.ForAll<M> ()
			 from mat2 in Prop.ForAll<M> ()
			 from mat3 in Prop.ForAll<M> ()
			 let ident = Mat.Identity<M> ()
			 let mult_mat1_ident = mat1.Multiply (ident)
			 let mult_mat12 = mat1.Multiply (mat2)
			 let mult_mat12_3 = mult_mat12.Multiply (mat3)
			 let mult_mat23 = mat2.Multiply (mat3)
			 let mult_mat1_23 = mat1.Multiply (mult_mat23)
			 select new
			 {
				 mat1,
				 mat2,
				 ident,
				 mult_mat1_ident,
				 mult_mat12,
				 mult_mat12_3,
				 mult_mat23,
				 mult_mat1_23
			 })
			.Check (p => p.mult_mat1_ident.Equals (p.mat1),
				label: $"{typeof (M).Name}: mat * I = mat")
			.Check (p => Mat.ApproxEquals (p.mult_mat12_3, p.mult_mat1_23, 0.001f),
				 label: $"{typeof (M).Name}: (mat1 * mat2) * mat3 = mat1 * (mat2 * mat3)");
        }

        public void CheckTranslation<M, V> () 
            where M : struct, ISquareMat<M, float>
            where V : struct, IVec<V, float>
        {
			(from v in Prop.ForAll<V> ()
			 from o in Prop.ForAll<V> ()
			 let last = v.Dimensions - 1
			 let vec = v.With (last, 1f)
			 let offset = v.With (last, 0f)
			 let trans = Mat.Translation<M> (offset.ToArray ().Segment (0, last))
			 let transvec = trans.Multiply (vec)
			 select new { vec, offset, trans, transvec })
			.Check (p => p.transvec.Equals (p.vec.Add (p.offset)),
				label: $"{typeof (M).Name}, {typeof (V).Name}: trans * vec = vec + offset");
        }

        public void CheckScaling<M, V> ()
            where M : struct, ISquareMat<M, float>
            where V : struct, IVec<V, float>
        {
			(from vec in Prop.ForAll<V> ()
			 from scale in Prop.ForAll<V> ()
			 let trans = Mat.Scaling<M> (scale.ToArray ())
			 let transvec = trans.Multiply (vec)
			 select new { vec, scale, trans, transvec })
			.Check (p => p.transvec.Equals (p.vec.Multiply (p.scale)),
				label: $"{typeof (M).Name}, {typeof (V).Name}: trans * vec = vec * scale");
        }

        public void CheckRotationZ<M, V> ()
            where M : struct, ISquareMat<M, float>
            where V : struct, IVec<V, float>
        {
			(from vec in Prop.ForAll<V> ()
			 from rot in Prop.ForAll<float> ()
			 let trans = Mat.RotationZ<M> (rot)
			 let transvec = trans.Multiply (vec)
			 select new { vec, rot, trans, transvec })
			.Check (p => p.transvec.Length.ApproxEquals (p.vec.Length),
				label: $"{typeof (M).Name}, {typeof (V).Name}: | trans * vec | = | vec |");
        }

        public void CheckRotationXY<M, V> ()
            where M : struct, ISquareMat<M, float>
            where V : struct, IVec<V, float>
        {
			(from vec in Prop.ForAll<V> ()
			 from rotX in Prop.ForAll<float> ()
			 from rotY in Prop.ForAll<float> ()
			 let trans = Mat.RotationX<M> (rotX).Multiply (Mat.RotationY<M> (rotY))
			 let transvec = trans.Multiply (vec)
			 select new { vec, rotX, rotY, trans, transvec })
			.Check (p => p.transvec.Length.ApproxEquals (p.vec.Length),
				label: $"{typeof (M).Name}, {typeof (V).Name}: | trans * vec | = | vec |");
        }

        public void CheckInverse<M> ()
            where M : struct, ISquareMat<M, float>
        {
			(from omat in Prop.ForAll<M> ()
			 let mat = FixZerosInDiagonal (omat)
			 let inv = mat.Inverse ()
			 let mat_inv = mat.Multiply (inv)
			 let ident = Mat.Identity<M> ()
			 select new { mat, inv, mat_inv, ident })
			.Check (p => Mat.ApproxEquals (p.mat_inv, p.ident, 0.1f),
				label: $"{typeof (M).Name}: mat * mat^-1 = I");
        }

        [TestMethod]
        public void TestAddSubtract ()
        {
            CheckAddSubtract<Mat2> ();
            CheckAddSubtract<Mat3> ();
            CheckAddSubtract<Mat4> ();
        }

        [TestMethod]
        public void TestMultiplyScalar ()
        {
            CheckMultiplyScalar<Mat2> ();
            CheckMultiplyScalar<Mat3> ();
            CheckMultiplyScalar<Mat4> ();
        }

        [TestMethod]
        public void TestTranspose ()
        {
            CheckTranspose<Mat2> ();
            CheckTranspose<Mat3> ();
            CheckTranspose<Mat4> ();
        }

        [TestMethod]
        public void TestMultiplyMatrices ()
        {
            CheckMultiplyMatrices<Mat2> ();
            CheckMultiplyMatrices<Mat3> ();
            CheckMultiplyMatrices<Mat4> ();
        }

        [TestMethod]
        public void TestTranslation ()
        {
            CheckTranslation<Mat2, Vec2> ();
            CheckTranslation<Mat3, Vec3> ();
            CheckTranslation<Mat4, Vec4> ();
        }

        [TestMethod]
        public void TestScaling ()
        {
            CheckScaling<Mat2, Vec2> ();
            CheckScaling<Mat3, Vec3> ();
            CheckScaling<Mat4, Vec4> ();
        }

        [TestMethod]
        public void TestRotation ()
        {
            CheckRotationZ<Mat2, Vec2> ();
            CheckRotationZ<Mat3, Vec3> ();
            CheckRotationZ<Mat4, Vec4> ();
            CheckRotationXY<Mat3, Vec3> ();
            CheckRotationXY<Mat4, Vec4> ();
        }

        [TestMethod]
        public void TestInverse ()
        {
            CheckInverse<Mat2> ();
            CheckInverse<Mat3> ();
            CheckInverse<Mat4> ();
        }
    }
}
