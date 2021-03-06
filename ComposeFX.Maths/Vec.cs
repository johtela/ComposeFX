﻿namespace ComposeFX.Maths
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using ExtensionCord;
	using Compute;
	using Graphics;
	
	/// <summary>
	/// An enumeration representing the coordinate axes. This is used in the vector classes
	/// to do swizzling of vector components.
	/// </summary>
	public enum Coord : int
	{
		x = 0,
		y = 1,
		z = 2,
		w = 3,
	}

	/// <summary>
	/// IVec{V, T} is a generic interface for vector types. It allows one to create generic 
	/// functions that are usable with all or some of the vector types. It also removes a lot of 
	/// repetitive code that would be otherwise needed to implement a same operation for different 
	/// `Vec*` structures.
	/// </summary>
	/// <typeparam name="V">The type of the vector. I.e. the type of the structure that implements
	/// this interface.</typeparam>
	/// <typeparam name="T">The type of the components in the vector.</typeparam>
	public interface IVec<V, T> : IEquatable<V>
		where V : struct, IVec<V, T>
		where T : struct, IEquatable<T>
	{
		/// <summary>
		// Create vector from an array.
		/// </summary>
		V FromArray (params T[] components);

		/// <summary>
		/// Copy vector components to an array.
		/// </summary>
		T[] ToArray ();

		/// <summary>
		/// Negate all of the components of the vector.
		/// </summary>
		V Invert ();

		/// <summary>
		/// Add another vector this one componentwise.
		/// </summary>
		V Add (in V other);

		/// <summary>
		/// Subtract the given vector from this one componentwise.
		/// </summary>
		V Subtract (in V other);

		/// <summary>
		/// Multiply the components of this vector with a same scalar value.
		/// </summary>
		V Multiply (T scalar);

		/// <summary>
		/// Multiply with another vector componentwise.
		/// </summary>
		V Multiply (in V scale);

		/// <summary>
		/// Divide the components of this vector by a same scalar value.
		/// </summary>
		V Divide (T scalar);

		/// <summary>
		/// Divide by another vector componentwise.
		/// </summary>
		V Divide (in V scale);

		/// <summary>
		/// Calculate the dot product with another vector.
		/// </summary>
		T Dot (in V other);

		/// <summary>
		/// Number of dimensions/components in the vector.
		/// </summary>
		int Dimensions { get; }

		/// <summary>
		/// The value of the index'th component of the vector.
		/// </summary>
		T this[int index] { get; }

		/// <summary>
		/// The lengh of the vector.
		/// </summary>
		T Length { get; }

		/// <summary>
		/// The lengh of the vector squared. This is bit faster to calculate than the actual length
		/// because the square root operation is omitted.
		/// </summary>
		T LengthSquared { get; }

		/// <summary>
		/// The normalized vector. I.e. vector with same direction, but with lenght of 1.
		/// </summary>
		V Normalized { get; }
	}

	/// <summary>
	/// Collection of operations that operate on `Vec*` structures.
	/// </summary>
	/// Many of the GLSL functions that operate on vectors are contained in the `Vec` class.
	/// Most of these functions are implemented as extension methods, so that the operations
	/// are easy to discover from an IDE. They are of course transformed to regular functions
	/// in GLSL.
	/// 
	/// Some of the functions are only available in C#, in which case the method is *not*
	/// decorated by the <see cref="GLFunction"/> attribute. The method signatures are defined
	/// in terms of the most generic component type that can be used in the specific context. 
	/// Because there is no way in C# to treat numeric types like `float` or `int` generically, 
	/// there might be overloads for different primitive types. The only unifying property of 
	/// these types is that they all implement <see cref="IEquatable{T}"/> interface. This
	/// is used as the lowest common denominator when there is no need to refer to a particular
	/// arithmetic operation.
	public static class Vec
    {
		/// <summary>
		/// Create a vectory structure from an array.
		/// </summary>
		/// If you have an array of numbers, you can create a vector out of them generically
		/// using this function. This is handy in many cases when you want to convert vectors
		/// from one type to another or create them generically.
        public static V FromArray<V, T> (params T[] items)
            where V : struct, IVec<V, T>
            where T : struct, IEquatable<T>
        {
            return default (V).FromArray (items);
        }

		public static V New<V, T> (T value)
			where V : struct, IVec<V, T>
			where T : struct, IEquatable<T>
		{
			var v = default (V);
			return v.FromArray (value.Duplicate (v.Dimensions));
		}

		public static V Parse<V> (string text)
            where V : struct, IVec<V, float>
		{
			var parts = text.Split (' ');
			var last = parts.Length - 1;
			if (parts[0] != "[" || parts[last] != "]")
				throw new FormatException ("The string is not in the canonical vector format.");
			var values = new float[last - 1];
			for (int i = 1; i < last; i++)
				values[i - 1] = float.Parse (parts[i], CultureInfo.InvariantCulture);
			return FromArray<V, float> (values);
		} 

		/// <summary>
		/// Convert a vector type to another vector type that has the same component type.
		/// </summary>
		/// This function is useful for generically transforming a vector to another vector
		/// type that has the same component type but different number of components.
		public static U Convert<V, U, T> (in V vec)
			where V : struct, IVec<V, T>
			where U : struct, IVec<U, T>
			where T : struct, IEquatable<T>
		{
			return FromArray<U, T> (vec.ToArray ());
		}

		public static U Convert<V, U> (in V vec)
			where V : struct, IVec<V, int>
			where U : struct, IVec<U, float>
		{
			return FromArray<U, float> (vec.ToArray ().Map (x => (float)x));
		}

		/// <summary>
		/// Returns true, when two vectors are approximetely same.
		/// </summary>
		/// Since rounding errors are quite common with `float`s that are used heavily in OpenGL,
		/// comparing for equality can be quite tricky. To alleviate the problem, this function
		/// compares the vectors approximately. The maximum allowed error is defined by the `epsilon`
		/// parameter. If you want to allow for error in 4th decimal, for example, you should pass
		/// in `epsilon` value of 0.001.
		public static bool ApproxEquals<V> (in V vec, in V other, float epsilon)
            where V : struct, IVec<V, float>
        {
			var dim = vec.Dimensions;
            for (int i = 0; i < dim; i++)
				if (!vec[i].ApproxEquals (other[i], epsilon)) 
					return false;
            return true;
        }

		/// <summary>
		/// Calls <see cref="ApproxEquals{V}(V, V, float)"/> with an epsilon value of 0.000001.
		/// </summary>
		public static bool ApproxEquals<V> (V vec, V other)
			where V : struct, IVec<V, float>
		{
			return ApproxEquals (vec, other, 0.000001f);
		}

		/// <summary>
		/// Return a copy of a vector with one component changed.
		/// </summary>
		/// Many times it is necessary to change a single component of a vector. However, if you assign a
		/// value to the component directly, you will mutate the original vector. To quickly return a copy 
		/// of a vector with one component changed, this extension method is provided.
        public static V ChangeComp<V, T> (in V vec, int i, T value)
            where V : struct, IVec<V, T>
            where T : struct, IEquatable<T>
        {
            var a = vec.ToArray ();
            a[i] = value;
            return vec.FromArray (a);
        }
		
		/// <summary>
		/// Return the sum of all the components in the vector.
		/// </summary>
		public static float Sum<V> (in V vec)
			where V : struct, IVec<V, float>
		{
			var dim = vec.Dimensions;
			var res = vec [0];
			for (int i = 1; i < dim; i++)
				res += vec [i];
			return res;
		}

		/// <summary>
		/// Return the sum of all the components in the vector.
		/// </summary>
		public static int Sumi<V> (in V vec)
			where V : struct, IVec<V, int>
		{
			var dim = vec.Dimensions;
			var res = vec[0];
			for (int i = 1; i < dim; i++)
				res += vec[i];
			return res;
		}

		/// <summary>
		/// Return the product of all the components in the vector.
		/// </summary>
		public static float Product<V> (in V vec)
			where V : struct, IVec<V, float>
		{
			var dim = vec.Dimensions;
			var res = vec[0];
			for (int i = 1; i < dim; i++)
				res *= vec[i];
			return res;
		}

		/// <summary>
		/// Return the product of all the components in the vector.
		/// </summary>
		public static int Producti<V> (in V vec)
			where V : struct, IVec<V, int>
		{
			var dim = vec.Dimensions;
			var res = vec[0];
			for (int i = 1; i < dim; i++)
				res *= vec[i];
			return res;
		}

		/// <summary>
		/// Interpolate between two vectors. 
		/// </summary>
		/// Returns a sequence of vectors that contain the interpolated values. 
		/// The number of elemens in the sequence is given in the `step` parameter.
		public static IEnumerable<V> Interpolate<V> (V from, V to, int steps)
			where V : struct, IVec<V, float>
		{
			var step = 1f / steps;
			var f = 0f;
			for (int i = 0; i < steps; i++, f += step)
				yield return Mix (in from, in to, f);
		}

		public static float DistanceTo<V> (in V from, in V to)
			where V : struct, IVec<V, float>
		{
			return to.Subtract (from).Length;
		}

		public static float SquaredDistanceTo<V> (in V from, in V to)
			where V : struct, IVec<V, float>
		{
			return to.Subtract (from).LengthSquared;
		}

		public static float ManhattanDistanceTo<V> (in V from, in V to)
			where V : struct, IVec<V, float>
		{
			return Sum (Abs (to.Subtract (in from)));
		}

		/// <summary>
		/// Map the components of the vector to another vector of the same type.
		/// </summary>
		/// This function maps the components of a vector to another vector
		/// using a lambda exprerssion or function to do the transformation.
		public static V Map<V, T> (in V vec, Func<T, T> map)
			where V : struct, IVec<V, T>
			where T : struct, IEquatable<T>
		{
			var arr = vec.ToArray ();
			for (int i = 0; i < arr.Length; i++)
				arr[i] = map (arr[i]);
			return vec.FromArray (arr);
		}

		public static U Map<V, U, T, S> (in V vec, Func<T, S> map)
			where V : struct, IVec<V, T>
			where U : struct, IVec<U, S>
			where T : struct, IEquatable<T>
			where S : struct, IEquatable<S>
		{
			var arr = vec.ToArray ();
			var len = arr.Length;
			var res = new S[len];
			for (int i = 0; i < len; i++)
				res[i] = map (arr[i]);
			return default (U).FromArray (res);
		}

		/// <summary>
		/// Map the components of two vectors to another vector of the same type.
		/// </summary>
		/// This function maps the components of two vectors given as an argument to a result 
		/// vector using a lambda exprerssion or function to do the transformation.
		public static V Map2<V, T> (in V vec, in V other, Func<T, T, T> map)
			where V : struct, IVec<V, T>
			where T : struct, IEquatable<T>
		{
			var arr = vec.ToArray ();
			for (int i = 0; i < arr.Length; i++)
				arr[i] = map (arr[i], other[i]);
			return vec.FromArray (arr);
		}

		/// <summary>
		/// Map the components of three vectors to another vector of the same type.
		/// </summary>
		/// This function maps the components of three vectors given as an argument to a result 
		/// vector using a lambda exprerssion or function to do the transformation.
		public static V Map3<V, T> (in V vec1, in V vec2, in V vec3, Func<T, T, T, T> map)
			where V : struct, IVec<V, T>
			where T : struct, IEquatable<T>
		{
			var arr = vec1.ToArray ();
			for (int i = 0; i < arr.Length; i++)
				arr[i] = map (arr[i], vec2[i], vec3[i]);
			return vec1.FromArray (arr);
		}

		public static bool All<V, T> (in V vec, Func<T, bool> predicate)
			where V : struct, IVec<V, T>
			where T : struct, IEquatable<T>
		{
			var dim = vec.Dimensions;
			for (int i = 0; i < dim; i++)
				if (!predicate (vec[i]))
					return false;
			return true;
		}

		public static bool Any<V, T> (in V vec, Func<T, bool> predicate)
			where V : struct, IVec<V, T>
			where T : struct, IEquatable<T>
		{
			var dim = vec.Dimensions;
			for (int i = 0; i < dim; i++)
				if (predicate (vec[i]))
					return true;
			return false;
		}

		/// <summary>
		/// Calculate the normal vector given three points in a plane using the vector cross product.
		/// </summary>
		/// All of the points need to be unique, otherwise the calculation does not work. The direction
		/// of the normal depends on the order in which the positions are given. If you find that the 
		/// normal is pointing to an opposite direction, switch the order of `adjecentPos1` and 
		/// `adjacentPos2` parameters.
		public static Vec3 CalculateNormal (in Vec3 position, in Vec3 adjacentPos1, in Vec3 adjacentPos2)
		{
			return Cross (adjacentPos1 - position, adjacentPos2 - position).Normalized;
		}

		public static bool AreCollinear (in Vec3 pos1, in Vec3 pos2, in Vec3 pos3)
		{
			var vec1 = (pos2 - pos1).Normalized;
			var vec2 = (pos3 - pos1).Normalized;
			return vec1 == vec2 || vec1 == -vec2;
		}

		/// <summary>
		/// The cross product of two vectors.
		/// </summary>
		/// Cross returns a vector perpendicular to the two vectors given as arguments. This operation
		/// only makes sense in 3D, so function is only defined for <see cref="Vec3"/>.
		[GLFunction ("cross ({0})")]
		[CLFunction ("cross ({0})")]
		public static Vec3 Cross (in Vec3 v1, in Vec3 v2)
		{
			return new Vec3 (
				v1.Y * v2.Z - v1.Z * v2.Y,
				v1.Z * v2.X - v1.X * v2.Z,
				v1.X * v2.Y - v1.Y * v2.X);			
		}

		/// <summary>
		/// The angle of the vector in YZ-plane. I.e. the angle of rotation around X-axis.
		/// </summary>
		public static float XRotation (in Vec3 vec)
		{
			return FMath.Atan2 (-vec.Y, vec.Z);
		}

		/// <summary>
		/// The angle of the vector in XZ-plane. I.e. the angle of rotation around Y-axis.
		/// </summary>
		public static float YRotation (in Vec3 vec)
		{
			return FMath.Atan2 (vec.X, vec.Z);
		}

		/// <summary>
		/// Convert a vector of degree values to radians.
		/// </summary>
		[GLFunction ("radians ({0})")]
		[CLFunction ("radians ({0})")]
		public static V Radians<V> (in V vec)
			where V : struct, IVec<V, float>
		{
			return Map<V, float> (in vec, FMath.Radians);
		}

		/// <summary>
		/// Convert a vector of radian values to degrees.
		/// </summary>
		[GLFunction ("degrees ({0})")]
		[CLFunction ("degrees ({0})")]
		public static V Degrees<V> (in V vec)
			where V : struct, IVec<V, float>
		{
			return Map<V, float> (in vec, FMath.Degrees);
		}

		/// <summary>
		/// Applies the <see cref="Math.Abs(float)"/> function to the vector componentwise.
		/// </summary>
		[GLFunction ("abs ({0})")]
		[CLFunction ("fabs ({0})")]
		public static V Abs<V> (in V vec)
			where V : struct, IVec<V, float>
		{
			return Map<V, float> (in vec, Math.Abs);
		}

		/// <summary>
		/// Applies the <see cref="FMath.Floor(float)"/> function to the vector componentwise.
		/// </summary>
		[GLFunction ("floor ({0})")]
		[CLFunction ("floor ({0})")]
		public static V Floor<V> (in V vec)
			where V : struct, IVec<V, float>
		{
			return Map<V, float> (in vec, FMath.Floor);
		}

		/// <summary>
		/// Applies the <see cref="FMath.Ceiling(float)"/> function to the vector componentwise.
		/// </summary>
		[GLFunction ("ceil ({0})")]
		[CLFunction ("ceil ({0})")]
		public static V Ceiling<V> (in V vec)
			where V : struct, IVec<V, float>
		{
			return Map<V, float> (in vec, FMath.Ceiling);
		}

		/// <summary>
		/// Applies the <see cref="FMath.Truncate(float)"/> function to the vector componentwise.
		/// </summary>
		[GLFunction ("trunc ({0})")]
		[CLFunction ("trunc ({0})")]
		public static V Truncate<V> (in V vec)
			where V : struct, IVec<V, float>
		{
			return Map<V, float> (in vec, FMath.Truncate);
		}

		/// <summary>
		/// Applies the <see cref="FMath.Fraction(float)"/> function to the vector componentwise.
		/// </summary>
		[GLFunction ("fract ({0})")]
		[CLFunction ("({0} - floor ({0}))")]
		public static V Fraction<V> (in V vec)
			where V : struct, IVec<V, float>
		{
			return Map<V, float> (in vec, FMath.Fraction);
		}

		/// <summary>
		/// Applies the <see cref="Math.Min(float, float)"/> function to the vector componentwise.
		/// </summary>
		[GLFunction ("min ({0})")]
		[CLFunction ("min ({0})")]
		public static V Min<V> (in V vec1, in V vec2)
			where V : struct, IVec<V, float>
		{
			return Map2<V, float> (in vec1, in vec2, Math.Min);
		}

		/// <summary>
		/// Applies the <see cref="Math.Min(int, int)"/> function to the vector componentwise.
		/// </summary>
		[GLFunction ("min ({0})")]
		[CLFunction ("min ({0})")]
		public static V Mini<V> (in V vec1, in V vec2)
			where V : struct, IVec<V, int>
		{
			return Map2<V, int> (in vec1, in vec2, Math.Min);
		}

		/// <summary>
		/// Applies the <see cref="Math.Max(float, float)"/> function to the vector componentwise.
		/// </summary>
		[GLFunction ("max ({0})")]
		[CLFunction ("max ({0})")]
		public static V Max<V> (in V vec1, in V vec2)
			where V : struct, IVec<V, float>
		{
			return Map2<V, float> (in vec1, in vec2, Math.Max);
		}

		/// <summary>
		/// Applies the <see cref="Math.Max(int, int)"/> function to the vector componentwise.
		/// </summary>
		[GLFunction ("max ({0})")]
		[CLFunction ("max ({0})")]
		public static V Maxi<V> (in V vec1, in V vec2)
			where V : struct, IVec<V, int>
		{
			return Map2<V, int> (in vec1, in vec2, Math.Max);
		}

		/// <summary>
		/// Applies the <see cref="FMath.Clamp(float, float, float)"/> function to the vector componentwise.
		/// </summary>
		[GLFunction ("clamp ({0})")]
		[CLFunction ("clamp ({0})")]
		public static V Clamp<V> (in V vec, float min, float max)
			where V : struct, IVec<V, float>
		{
			return Map<V, float> (in vec, a => FMath.Clamp (a, min, max));
		}

		/// <summary>
		/// Applies the <see cref="FMath.Clamp(int, int, int)"/> function to the vector componentwise.
		/// </summary>
		[GLFunction ("clamp ({0})")]
		[CLFunction ("clamp ({0})")]
		public static V Clamp<V> (in V vec, int min, int max)
			where V : struct, IVec<V, int>
		{
			return Map<V, int> (in vec, a => FMath.Clamp (a, min, max));
		}

		/// <summary>
		/// Applies the <see cref="FMath.Clamp(float, float, float)"/> function to the vector componentwise.
		/// The minimum and maximum values are also given as vectors.
		/// </summary>
		[GLFunction ("clamp ({0})")]
		[CLFunction ("clamp ({0})")]
		public static V Clamp<V> (in V vec, in V min, in V max)
			where V : struct, IVec<V, float>
		{
			return Map3<V, float> (in vec, in min, in max, FMath.Clamp);
		}

		/// <summary>
		/// Applies the <see cref="FMath.Clamp(int, int, int)"/> function to the vector componentwise.
		/// The minimum and maximum values are also given as vectors.
		/// </summary>
		[GLFunction ("clamp ({0})")]
		[CLFunction ("clamp ({0})")]
		public static V Clampi<V> (in V vec, in V min, in V max)
			where V : struct, IVec<V, int>
		{
			return Map3<V, int> (in vec, in min, in max, FMath.Clamp);
		}

		/// <summary>
		/// Applies the <see cref="FMath.Mix(float, float, float)"/> function to the vector componentwise.
		/// The interPos parameter is also given as vector.
		/// </summary>
		[GLFunction ("mix ({0})")]
		[CLFunction ("mix ({0})")]
		public static V Mix<V> (in V vec, in V other, in V interPos)
			where V : struct, IVec<V, float>
		{
			return Map3<V, float> (in vec, in other, in interPos, FMath.Mix);
		}

		/// <summary>
		/// Applies the <see cref="FMath.Mix(float, float, float)"/> function to the vector componentwise.
		/// </summary>
		[GLFunction ("mix ({0})")]
		[CLFunction ("mix ({0})")]
		public static V Mix<V> (in V vec1, in V vec2, float interPos)
			where V : struct, IVec<V, float>
		{
			return Map2<V, float> (in vec1, in vec2, (x, y) => FMath.Mix (x, y, interPos));
		}

		/// <summary>
		/// Applies the <see cref="FMath.Step(float, float)"/> function to the vector componentwise.
		/// </summary>
		[GLFunction ("step ({0})")]
		[CLFunction ("step ({0})")]
		public static V Step<V> (float edge, in V vec)
			where V : struct, IVec<V, float>
		{
			return Map<V, float> (in vec, a => FMath.Step (edge, a));
		}

		/// <summary>
		/// Applies the <see cref="FMath.Step(float, float)"/> function to the vector componentwise.
		/// The edge values are also given as vector.
		/// </summary>
		[GLFunction ("step ({0})")]
		[CLFunction ("step ({0})")]
		public static V Step<V> (in V edge, in V vec)
			where V : struct, IVec<V, float>
		{
			return Map2<V, float> (in edge, in vec, FMath.Step);
		}

		/// <summary>
		/// Applies the <see cref="FMath.SmoothStep(float, float, float)"/> function to the vector componentwise.
		/// </summary>
		[GLFunction ("smoothstep ({0})")]
		[CLFunction ("smoothstep ({0})")]
		public static V SmoothStep<V> (float edgeLower, float edgeUpper, in V vec)
			where V : struct, IVec<V, float>
		{
			return Map<V, float> (in vec, a => FMath.SmoothStep (edgeLower, edgeUpper, a));
		}

		/// <summary>
		/// Applies the <see cref="FMath.SmoothStep(float, float, float)"/> function to the vector componentwise.
		/// The edge values are also given as vector.
		/// </summary>
		[GLFunction ("smoothstep ({0})")]
		[CLFunction ("smoothstep ({0})")]
		public static V SmoothStep<V> (in V edgeLower, in V edgeUpper, in V vec)
			where V : struct, IVec<V, float>
		{
			return Map3<V, float> (in edgeLower, in edgeUpper, in vec, FMath.SmoothStep);
		}

		/// <summary>
		/// Applies the <see cref="FMath.Pow(float, float)"/> function to the vector componentwise.
		/// The exp values are also given as vector.
		/// </summary>
		[GLFunction ("pow ({0})")]
		[CLFunction ("pow ({0})")]
		public static V Pow<V> (in V vec, in V exp)
			where V : struct, IVec<V, float>
		{
			return Map2<V, float> (in vec, in exp, FMath.Pow);
		}

		/// <summary>
		/// Applies the <see cref="FMath.Exp(float)"/> function to the vector componentwise.
		/// </summary>
		[GLFunction ("exp ({0})")]
		[CLFunction ("exp ({0})")]
		public static V Exp<V> (in V vec)
			where V : struct, IVec<V, float>
		{
			return Map<V, float> (in vec, FMath.Exp);
		}

		/// <summary>
		/// Applies the <see cref="FMath.Log(float)"/> function to the vector componentwise.
		/// </summary>
		[GLFunction ("log ({0})")]
		[CLFunction ("log ({0})")]
		public static V Log<V> (in V vec)
			where V : struct, IVec<V, float>
		{
			return Map<V, float> (in vec, FMath.Log);
		}

		/// <summary>
		/// Applies the <see cref="FMath.Sqrt(float)"/> function to the vector componentwise.
		/// </summary>
		[GLFunction ("sqrt ({0})")]
		[CLFunction ("sqrt ({0})")]
		public static V Sqrt<V> (in V vec)
			where V : struct, IVec<V, float>
		{
			return Map<V, float> (in vec, FMath.Sqrt);
		}

		/// <summary>
		/// Applies the <see cref="FMath.InverseSqrt(float)"/> function to the vector componentwise.
		/// </summary>
		[GLFunction ("inversesqrt ({0})")]
		[CLFunction ("inversesqrt ({0})")]
		public static V InverseSqrt<V> (in V vec)
			where V : struct, IVec<V, float>
		{
			return Map<V, float> (in vec, FMath.InverseSqrt);
		}

		[GLFunction ("mod ({0})")]
		[CLFunction ("fmod ({0})")]
		public static V Mod<V> (in V vec, float modulo)
			where V : struct, IVec<V, float>
		{
			return Map<V, float> (in vec, x => x % modulo);
		}

		[GLFunction ("mod ({0})")]
		[CLFunction ("fmod ({0})")]
		public static V Mod<V> (in V vec, in V modulo)
			where V : struct, IVec<V, float>
		{
			return Map2<V, float> (in vec, in modulo, (x, m) => x % m);
		}

		/// <summary>
		/// Calculates the reflection vector along given normal.
		/// </summary>
		/// The reflect function returns a vector that points in the direction of reflection.
		/// The function has two input parameters: the incident vector, and the normal vector 
		/// of the reflecting surface.
		/// 
		/// Note: To obtain the desired result the `along` vector has to be normalized. The reflection 
		/// vector always has the same length as the incident vector. From this it follows that the 
		/// reflection vector is normalized if `vec` and `along` vectors are both normalized.		
		[GLFunction ("reflect ({0})")]
		public static V Reflect<V, T> (in V vec, in V along)
			where V : struct, IVec<V, float>
			where T : struct, IEquatable<T>
		{
			return vec.Subtract (along.Multiply (2 * vec.Dot (in along)));
		}

		/// <summary>
		/// Applies the <see cref="FMath.Sin(float)"/> function to the vector componentwise.
		/// </summary>
		[GLFunction ("sin ({0})")]
		[CLFunction ("sin ({0})")]
		public static V Sin<V> (in V angles)
			where V : struct, IVec<V, float>
		{
			return Map<V, float> (in angles, FMath.Sin);
		}

		/// <summary>
		/// Applies the <see cref="FMath.Cos(float)"/> function to the vector componentwise.
		/// </summary>
		[GLFunction ("cos ({0})")]
		[CLFunction ("cos ({0})")]
		public static V Cos<V> (in V angles)
			where V : struct, IVec<V, float>
		{
			return Map<V, float> (in angles, FMath.Cos);
		}

		/// <summary>
		/// Applies the <see cref="FMath.Tan(float)"/> function to the vector componentwise.
		/// </summary>
		[GLFunction ("tan ({0})")]
		[CLFunction ("tan ({0})")]
		public static V Tan<V> (in V angles)
			where V : struct, IVec<V, float>
		{
			return Map<V, float> (in angles, FMath.Tan);
		}

		/// <summary>
		/// Applies the <see cref="FMath.Asin(float)"/> function to the vector componentwise.
		/// </summary>
		[GLFunction ("asin ({0})")]
		[CLFunction ("asin ({0})")]
		public static V Asin<V> (in V x)
			where V : struct, IVec<V, float>
		{
			return Map<V, float> (in x, FMath.Asin);
		}

		/// <summary>
		/// Applies the <see cref="FMath.Acos(float)"/> function to the vector componentwise.
		/// </summary>
		[GLFunction ("acos ({0})")]
		[CLFunction ("acos ({0})")]
		public static V Acos<V> (in V x)
			where V : struct, IVec<V, float>
		{
			return Map<V, float> (in x, FMath.Acos);
		}

		/// <summary>
		/// Applies the <see cref="FMath.Atan(float)"/> function to the vector componentwise.
		/// </summary>
		[GLFunction ("atan ({0})")]
		[CLFunction ("atan ({0})")]
		public static V Atan<V> (in V y_over_x)
			where V : struct, IVec<V, float>
		{
			return Map<V, float> (in y_over_x, FMath.Atan);
		}

		/// <summary>
		/// Applies the <see cref="FMath.Atan2(float, float)"/> function to the vector componentwise.
		/// </summary>
		[GLFunction ("atan ({0})")]
		[CLFunction ("atan ({0})")]
		public static V Atan2<V> (in V y, in V x)
			where V : struct, IVec<V, float>
		{
			return Map2<V, float> (in y, in x, FMath.Atan2);
		}

        [GLFunction ("sign ({0})")]
        [CLFunction ("sign ({0})")]
        public static V Sign<V> (in V vec)
			where V : struct, IVec<V, float>
        {
            return Map<V, float> (in vec, FMath.Sign);
        }

		/// <summary>
		/// Check whether any of the components of the vector are NaN. 
		/// </summary>
		public static bool IsNaN<V> (in V vec)
			where V : struct, IVec<V, float>
		{
			return Any<V, float> (in vec, float.IsNaN);
		}

		private static readonly Random _random = new Random ();

		public static V Random<V> (float rangeMin, float rangeMax)
			where V : struct, IVec<V, float>
		{
			return Random<V> (_random, rangeMin, rangeMax);
		}

		public static V Random<V> (Random rnd, float rangeMin, float rangeMax)
			where V : struct, IVec<V, float>
		{
			var range = rangeMax - rangeMin;
			return FromArray<V, float> (
				(float)rnd.NextDouble () * range + rangeMin,
				(float)rnd.NextDouble () * range + rangeMin,
				(float)rnd.NextDouble () * range + rangeMin,
				(float)rnd.NextDouble () * range + rangeMin);
		}

		public static V Jitter<V> (in V vec, float maxDelta)
			where V : struct, IVec<V, float>
		{
			return vec.Add (Random<V> (new Random (vec.GetHashCode ()), -maxDelta, maxDelta));
		}
	}
}