﻿namespace ComposeFX.Maths
{
    using System;
    using System.Text;
	using System.Globalization;
	using Compute;
	using Graphics;

	/// <summary>
	/// Vector stucture that is mapped to `ivec4` when used in
	/// OpenGL shaders and `int4` when used in OpenCL kernels.
	/// </summary>
	[GLType ("ivec4")]
	[CLType ("int4")]
    public readonly struct Vec4i : IVec<Vec4i, int>
    { 
		private static readonly Vec4i _zero = new Vec4i ();
		private static readonly Vec4i _one = new Vec4i (1);

		/// <summary>
		/// The X component of the vector.
		/// </summary>
		[GLField ("x")]
		[CLField ("x")]
        public readonly int X; 

		/// <summary>
		/// The Y component of the vector.
		/// </summary>
		[GLField ("y")]
		[CLField ("y")]
        public readonly int Y; 

		/// <summary>
		/// The Z component of the vector.
		/// </summary>
		[GLField ("z")]
		[CLField ("z")]
        public readonly int Z; 

		/// <summary>
		/// The W component of the vector.
		/// </summary>
		[GLField ("w")]
		[CLField ("w")]
        public readonly int W; 

		/// <summary>
		/// Static references to vectors with all zeroes or ones.
		/// </summary>
		public static ref readonly Vec4i Zero => ref _zero;
		public static ref readonly Vec4i One => ref _one;

		/// <summary>
		/// Initialize all of the components of the vector.
		/// </summary>
		[GLConstructor ("ivec4 ({0})")]
		[CLConstructor ("(int4) ({0})")]
		public Vec4i (int x, int y, int z, int w)
		{	
			X = x; 
			Y = y; 
			Z = z; 
			W = w; 
		}

		/// <summary>
		/// Initialize all of the components with a same value.
		/// </summary>
		[GLConstructor ("ivec4 ({0})")]
		[CLConstructor ("(int4) ({0})")]
		public Vec4i (int value)
		{	
			X = value; 
			Y = value; 
			Z = value; 
			W = value; 
		}
		/// <summary>
		/// Copy the components of the vector from another vector.
		/// </summary>
		[GLConstructor ("ivec4 ({0})")]
		[CLConstructor ("(int4) ({0})")]
		public Vec4i (in Vec2i vec, int z, int w)
		{	
			X = vec.X; 
			Y = vec.Y; 
			Z = z; 
			W = w; 
		}

		/// <summary>
		/// Copy the components of the vector from another vector.
		/// </summary>
		[GLConstructor ("ivec4 ({0})")]
		[CLConstructor ("(int4) ({0})")]
		public Vec4i (in Vec3i vec, int w)
		{	
			X = vec.X; 
			Y = vec.Y; 
			Z = vec.Z; 
			W = w; 
		}

		/// <summary>
		/// Copy the components of the vector from another vector.
		/// </summary>
		[GLConstructor ("ivec4 ({0})")]
		[CLConstructor ("(int4) ({0})")]
		public Vec4i (in Vec4i vec)
		{	
			X = vec.X; 
			Y = vec.Y; 
			Z = vec.Z; 
			W = vec.W; 
		}

		/// <summary>
		/// Initialize vector from an array.
		/// </summary>
		public Vec4i FromArray (params int[] components)
		{	
			return new Vec4i (components[0], components[1], components[2], components[3]);
		}

		/// <summary>
		/// Copy vector components to an array.
		/// </summary>
		public int[] ToArray ()
		{	
			return new int[] { X, Y, Z, W };
		}

		/// <summary>
		/// Negate all of the components of the vector.
		/// </summary>
		[GLUnaryOperator ("-{0}")]
		[CLUnaryOperator ("-{0}")]
		public Vec4i Invert ()
		{
			return new Vec4i (-X, -Y, -Z, -W);
		}

		/// <summary>
		/// Add another vector this one componentwise.
		/// </summary>
		[GLBinaryOperator ("{0} + {1}")]
		[CLBinaryOperator ("{0} + {1}")]
		public Vec4i Add (in Vec4i other)
		{
			return new Vec4i (X + other.X, Y + other.Y, Z + other.Z, W + other.W);
		}

		/// <summary>
		/// Subtract the given vector from this one componentwise.
		/// </summary>
		[GLBinaryOperator ("{0} - {1}")]
		[CLBinaryOperator ("{0} - {1}")]
		public Vec4i Subtract (in Vec4i other)
		{
			return new Vec4i (X - other.X, Y - other.Y, Z - other.Z, W - other.W);
		}

		/// <summary>
		/// Multiply with another vector componentwise.
		/// </summary>
		[GLBinaryOperator ("{0} * {1}")]
		[CLBinaryOperator ("{0} * {1}")]
		public Vec4i Multiply (in Vec4i other)
		{
			return new Vec4i (X * other.X, Y * other.Y, Z * other.Z, W * other.W);
		}

		/// <summary>
		/// Multiply the components of this vector with a same scalar value.
		/// </summary>
		[GLBinaryOperator ("{0} * {1}")]
		[CLBinaryOperator ("{0} * {1}")]
		public Vec4i Multiply (int scalar)
		{
			return new Vec4i (X * scalar, Y * scalar, Z * scalar, W * scalar);
		}

		/// <summary>
		/// Divide the two vectors componentwise.
		/// </summary>
		[GLBinaryOperator ("{0} / {1}")]
		[CLBinaryOperator ("{0} / {1}")]
		public Vec4i Divide (in Vec4i other)
		{
			return new Vec4i (X / other.X, Y / other.Y, Z / other.Z, W / other.W);
		}

		/// <summary>
		/// Divide the components of this vector by a same scalar value.
		/// </summary>
		[GLBinaryOperator ("{0} / {1}")]
		[CLBinaryOperator ("{0} / {1}")]
		public Vec4i Divide (int scalar)
		{
			return new Vec4i (X / scalar, Y / scalar, Z / scalar, W / scalar);
		}

		/// <summary>
		/// Calculate the dot product with another vector.
		/// </summary>
		[GLFunction ("dot ({0})")]
		[CLFunction ("dot ({0})")]
		public int Dot (in Vec4i other)
		{
			return X * other.X + Y * other.Y + Z * other.Z + W * other.W;
		}

		/// <summary>
		/// Equality comparison with another vector.
		/// </summary>
		public bool Equals (Vec4i other)
		{
			return X == other.X && Y == other.Y && Z == other.Z && W == other.W;
		}

		/// <summary>
		/// Number of dimensions/components in the vector.
		/// </summary>
		public int Dimensions
		{
			get { return 4; }
		}

		/// <summary>
		/// The value of the index'th component of the vector.
		/// </summary>
		public int this[int index]
		{
			get
			{
				switch (index)
				{	         
					case 0: return X;          
					case 1: return Y;          
					case 2: return Z;          
					case 3: return W; 
			        default: throw new ArgumentOutOfRangeException("index");
				}
			} 
		}
		
		/// <summary>
		/// Swizzling of the vector returns the specified components in the specified order.
		/// </summary>
		public Vec4i this[Coord x, Coord y, Coord z, Coord w]
		{
			get { return new Vec4i (this[(int)x], this[(int)y], this[(int)z], this[(int)w]); }
		}

		
		/// <summary>
		/// Swizzling of the vector returns the specified components in the specified order.
		/// </summary>
		public Vec3i this[Coord x, Coord y, Coord z]
		{
			get { return new Vec3i (this[(int)x], this[(int)y], this[(int)z]); }
		}

		
		/// <summary>
		/// Swizzling of the vector returns the specified components in the specified order.
		/// </summary>
		public Vec2i this[Coord x, Coord y]
		{
			get { return new Vec2i (this[(int)x], this[(int)y]); }
		}

		/// <summary>
		/// The lengh of the vector squared. This is bit faster to calculate than the actual length
		/// because the square root operation is omitted.
		/// </summary>
		public int LengthSquared
		{
			get { return X * X + Y * Y + Z * Z + W * W; }
		}

		/// <summary>
		/// The lengh of the vector.
		/// </summary>
		[GLFunction ("length ({0})")]
		[CLFunction ("length ({0})")]
		public int Length
		{
			get { return (int)Math.Sqrt (LengthSquared); }
		}

		/// <summary>
		/// The normalized vector. I.e. vector with same direction, but with lenght of 1.
		/// </summary>
		[GLFunction ("normalize ({0})")]
		[CLFunction ("normalize ({0})")]
		public Vec4i Normalized
		{
			get { return Divide (Length); }
		}

		/// <summary>
		/// Equality comparison inherited from Object. It is overridden to do the comparison componentwise.
		/// </summary>
		public override bool Equals (object obj)
		{
            return obj is Vec4i && Equals ((Vec4i)obj);
		}

		/// <summary>
		/// Hash code generation inherited from Object. It is overridden to calculate the hash code componentwise.
		/// </summary>
        public override int GetHashCode ()
        {
			return X.GetHashCode () ^ Y.GetHashCode () ^ Z.GetHashCode () ^ W.GetHashCode ();
        }

		/// <summary>
		/// String conversion inherited from Object. Formats the vector in matrix style.
		/// I.e. components inside square brackets without commas in between.
		/// </summary>
        public override string ToString ()
        {
            var sb = new StringBuilder ("[");
            for (int i = 0; i < 4; i++)
                sb.AppendFormat (" {0}", this[i].ToString (CultureInfo.InvariantCulture));
            sb.Append (" ]");
            return sb.ToString ();
        }

		/// <summary>
		/// Negate all of the components of the vector.
		/// </summary>
		[GLUnaryOperator ("-{0}")]
		[CLUnaryOperator ("-{0}")]
        public static Vec4i operator - (in Vec4i vec)
        {
            return vec.Invert ();
        }

		/// <summary>
		/// Subtracts the right vector from the left componentwise.
		/// </summary>
		[GLBinaryOperator ("{0} - {1}")]
		[CLBinaryOperator ("{0} - {1}")]
        public static Vec4i operator - (in Vec4i left, in Vec4i right)
        {
            return left.Subtract (in right);
        }

		/// <summary>
		/// Multiply the components of the vector with a same scalar value.
		/// </summary>
		[GLBinaryOperator ("{0} * {1}")]
		[CLBinaryOperator ("{0} * {1}")]
        public static Vec4i operator * (int scalar, in Vec4i vec)
        {
            return vec.Multiply (scalar);
        }

		/// <summary>
		/// Multiply the components of the vector with a same scalar value.
		/// </summary>
		[GLBinaryOperator ("{0} * {1}")]
		[CLBinaryOperator ("{0} * {1}")]
        public static Vec4i operator * (in Vec4i vec, int scalar)
        {
            return vec.Multiply (scalar);
        }

		/// <summary>
		/// Multiply the two vectors componentwise.
		/// </summary>
		[GLBinaryOperator ("{0} * {1}")]
		[CLBinaryOperator ("{0} * {1}")]
        public static Vec4i operator * (in Vec4i vec, in Vec4i scale)
        {
            return vec.Multiply (in scale);
        }

		/// <summary>
		/// Divide the components of the vector by a same scalar value.
		/// </summary>
		[GLBinaryOperator ("{0} / {1}")]
		[CLBinaryOperator ("{0} / {1}")]
        public static Vec4i operator / (in Vec4i vec, int scalar)
        {
            return vec.Divide (scalar);
        }

		/// <summary>
		/// Divide the two vectors componentwise.
		/// </summary>
		[GLBinaryOperator ("{0} / {1}")]
		[CLBinaryOperator ("{0} / {1}")]
        public static Vec4i operator / (in Vec4i vec, in Vec4i scale)
        {
            return vec.Divide (scale);
        }

		/// <summary>
		/// Divide a scalar by a vector.
		/// </summary>
		[GLBinaryOperator ("{0} / {1}")]
		[CLBinaryOperator ("{0} / {1}")]
        public static Vec4i operator / (int scalar, in Vec4i vec)
        {
            return new Vec4i (scalar).Divide (vec);
        }

		/// <summary>
		/// Add the two vectors together componentwise.
		/// </summary>
		[GLBinaryOperator ("{0} + {1}")]
		[CLBinaryOperator ("{0} + {1}")]
        public static Vec4i operator + (in Vec4i left, in Vec4i right)
        {
            return left.Add (right);
        }

		/// <summary>
		/// Componentwise equality comparison between the two vectors.
		/// </summary>
		[GLBinaryOperator ("{0} == {1}")]
		[CLBinaryOperator ("{0} == {1}")]
        public static bool operator == (in Vec4i left, in Vec4i right)
        {
            return left.Equals (right);
        }

		/// <summary>
		/// Componentwise inequality comparison between the two vectors.
		/// </summary>
		[GLBinaryOperator ("{0} != {1}")]
		[CLBinaryOperator ("{0} != {1}")]
        public static bool operator != (in Vec4i left, in Vec4i right)
        {
            return !left.Equals (right);
        }
    }
} 