namespace ComposeFX.Maths
{
    using System;
    using System.Text;
	using System.Globalization;
	using Compute;
	using Graphics;

	/// <summary>
	/// Vector stucture that is mapped to `vec3` when used in
	/// OpenGL shaders and `float3` when used in OpenCL kernels.
	/// </summary>
	[GLType ("vec3")]
	[CLType ("float3")]
    public readonly struct Vec3 : IVec<Vec3, float>
    { 
		/// <summary>
		/// The X component of the vector.
		/// </summary>
		[GLField ("x")]
		[CLField ("x")]
        public readonly float X; 

		/// <summary>
		/// The Y component of the vector.
		/// </summary>
		[GLField ("y")]
		[CLField ("y")]
        public readonly float Y; 

		/// <summary>
		/// The Z component of the vector.
		/// </summary>
		[GLField ("z")]
		[CLField ("z")]
        public readonly float Z; 

		/// <summary>
		/// Initialize all of the components of the vector.
		/// </summary>
		[GLConstructor ("vec3 ({0})")]
		[CLConstructor ("(float3) ({0})")]
		public Vec3 (float x, float y, float z)
		{	
			X = x; 
			Y = y; 
			Z = z; 
		}

		/// <summary>
		/// Initialize all of the components with a same value.
		/// </summary>
		[GLConstructor ("vec3 ({0})")]
		[CLConstructor ("(float3) ({0})")]
		public Vec3 (float value)
		{	
			X = value; 
			Y = value; 
			Z = value; 
		}
		/// <summary>
		/// Copy the components of the vector from another vector.
		/// </summary>
		[GLConstructor ("vec3 ({0})")]
		[CLConstructor ("(float3) ({0})")]
		public Vec3 (in Vec2 vec, float z)
		{	
			X = vec.X; 
			Y = vec.Y; 
			Z = z; 
		}

		/// <summary>
		/// Copy the components of the vector from another vector.
		/// </summary>
		[GLConstructor ("vec3 ({0})")]
		[CLConstructor ("(float3) ({0})")]
		public Vec3 (in Vec3 vec)
		{	
			X = vec.X; 
			Y = vec.Y; 
			Z = vec.Z; 
		}

		/// <summary>
		/// Copy the components of the vector from another vector.
		/// </summary>
		[GLConstructor ("vec3 ({0})")]
		[CLConstructor ("(float3) ({0})")]
		public Vec3 (in Vec4 vec)
		{	
			X = vec.X; 
			Y = vec.Y; 
			Z = vec.Z; 
		}

		/// <summary>
		/// Initialize vector from an array.
		/// </summary>
		public Vec3 FromArray (params float[] components)
		{	
			return new Vec3 (components[0], components[1], components[2]);
		}

		/// <summary>
		/// Copy vector components to an array.
		/// </summary>
		public float[] ToArray ()
		{	
			return new float[] { X, Y, Z };
		}

		/// <summary>
		/// Negate all of the components of the vector.
		/// </summary>
		[GLUnaryOperator ("-{0}")]
		[CLUnaryOperator ("-{0}")]
		public Vec3 Invert ()
		{
			return new Vec3 (-X, -Y, -Z);
		}

		/// <summary>
		/// Add another vector this one componentwise.
		/// </summary>
		[GLBinaryOperator ("{0} + {1}")]
		[CLBinaryOperator ("{0} + {1}")]
		public Vec3 Add (in Vec3 other)
		{
			return new Vec3 (X + other.X, Y + other.Y, Z + other.Z);
		}

		/// <summary>
		/// Subtract the given vector from this one componentwise.
		/// </summary>
		[GLBinaryOperator ("{0} - {1}")]
		[CLBinaryOperator ("{0} - {1}")]
		public Vec3 Subtract (in Vec3 other)
		{
			return new Vec3 (X - other.X, Y - other.Y, Z - other.Z);
		}

		/// <summary>
		/// Multiply with another vector componentwise.
		/// </summary>
		[GLBinaryOperator ("{0} * {1}")]
		[CLBinaryOperator ("{0} * {1}")]
		public Vec3 Multiply (in Vec3 other)
		{
			return new Vec3 (X * other.X, Y * other.Y, Z * other.Z);
		}

		/// <summary>
		/// Multiply the components of this vector with a same scalar value.
		/// </summary>
		[GLBinaryOperator ("{0} * {1}")]
		[CLBinaryOperator ("{0} * {1}")]
		public Vec3 Multiply (float scalar)
		{
			return new Vec3 (X * scalar, Y * scalar, Z * scalar);
		}

		/// <summary>
		/// Divide the two vectors componentwise.
		/// </summary>
		[GLBinaryOperator ("{0} / {1}")]
		[CLBinaryOperator ("{0} / {1}")]
		public Vec3 Divide (in Vec3 other)
		{
			return new Vec3 (X / other.X, Y / other.Y, Z / other.Z);
		}

		/// <summary>
		/// Divide the components of this vector by a same scalar value.
		/// </summary>
		[GLBinaryOperator ("{0} / {1}")]
		[CLBinaryOperator ("{0} / {1}")]
		public Vec3 Divide (float scalar)
		{
			return new Vec3 (X / scalar, Y / scalar, Z / scalar);
		}

		/// <summary>
		/// Calculate the dot product with another vector.
		/// </summary>
		[GLFunction ("dot ({0})")]
		[CLFunction ("dot ({0})")]
		public float Dot (in Vec3 other)
		{
			return X * other.X + Y * other.Y + Z * other.Z;
		}

		/// <summary>
		/// Equality comparison with another vector.
		/// </summary>
		public bool Equals (Vec3 other)
		{
			return X == other.X && Y == other.Y && Z == other.Z;
		}

		/// <summary>
		/// Number of dimensions/components in the vector.
		/// </summary>
		public int Dimensions
		{
			get { return 3; }
		}

		/// <summary>
		/// The value of the index'th component of the vector.
		/// </summary>
		public float this[int index]
		{
			get
			{
				switch (index)
				{	         
					case 0: return X;          
					case 1: return Y;          
					case 2: return Z; 
			        default: throw new ArgumentOutOfRangeException("index");
				}
			} 
		}
		
		/// <summary>
		/// Swizzling of the vector returns the specified components in the specified order.
		/// </summary>
		public Vec3 this[Coord x, Coord y, Coord z]
		{
			get { return new Vec3 (this[(int)x], this[(int)y], this[(int)z]); }
		}

		
		/// <summary>
		/// Swizzling of the vector returns the specified components in the specified order.
		/// </summary>
		public Vec2 this[Coord x, Coord y]
		{
			get { return new Vec2 (this[(int)x], this[(int)y]); }
		}

		/// <summary>
		/// The lengh of the vector squared. This is bit faster to calculate than the actual length
		/// because the square root operation is omitted.
		/// </summary>
		public float LengthSquared
		{
			get { return X * X + Y * Y + Z * Z; }
		}

		/// <summary>
		/// The lengh of the vector.
		/// </summary>
		[GLFunction ("length ({0})")]
		[CLFunction ("length ({0})")]
		public float Length
		{
			get { return (float)Math.Sqrt (LengthSquared); }
		}

		/// <summary>
		/// The normalized vector. I.e. vector with same direction, but with lenght of 1.
		/// </summary>
		[GLFunction ("normalize ({0})")]
		[CLFunction ("normalize ({0})")]
		public Vec3 Normalized
		{
			get { return Divide (Length); }
		}

		/// <summary>
		/// Equality comparison inherited from Object. It is overridden to do the comparison componentwise.
		/// </summary>
		public override bool Equals (object obj)
		{
            return obj is Vec3 && Equals ((Vec3)obj);
		}

		/// <summary>
		/// Hash code generation inherited from Object. It is overridden to calculate the hash code componentwise.
		/// </summary>
        public override int GetHashCode ()
        {
			return X.GetHashCode () ^ Y.GetHashCode () ^ Z.GetHashCode ();
        }

		/// <summary>
		/// String conversion inherited from Object. Formats the vector in matrix style.
		/// I.e. components inside square brackets without commas in between.
		/// </summary>
        public override string ToString ()
        {
            var sb = new StringBuilder ("[");
            for (int i = 0; i < 3; i++)
                sb.AppendFormat (" {0}", this[i].ToString (CultureInfo.InvariantCulture));
            sb.Append (" ]");
            return sb.ToString ();
        }

		/// <summary>
		/// Negate all of the components of the vector.
		/// </summary>
		[GLUnaryOperator ("-{0}")]
		[CLUnaryOperator ("-{0}")]
        public static Vec3 operator - (in Vec3 vec)
        {
            return vec.Invert ();
        }

		/// <summary>
		/// Subtracts the right vector from the left componentwise.
		/// </summary>
		[GLBinaryOperator ("{0} - {1}")]
		[CLBinaryOperator ("{0} - {1}")]
        public static Vec3 operator - (in Vec3 left, in Vec3 right)
        {
            return left.Subtract (in right);
        }

		/// <summary>
		/// Multiply the components of the vector with a same scalar value.
		/// </summary>
		[GLBinaryOperator ("{0} * {1}")]
		[CLBinaryOperator ("{0} * {1}")]
        public static Vec3 operator * (float scalar, in Vec3 vec)
        {
            return vec.Multiply (scalar);
        }

		/// <summary>
		/// Multiply the components of the vector with a same scalar value.
		/// </summary>
		[GLBinaryOperator ("{0} * {1}")]
		[CLBinaryOperator ("{0} * {1}")]
        public static Vec3 operator * (in Vec3 vec, float scalar)
        {
            return vec.Multiply (scalar);
        }

		/// <summary>
		/// Multiply the two vectors componentwise.
		/// </summary>
		[GLBinaryOperator ("{0} * {1}")]
		[CLBinaryOperator ("{0} * {1}")]
        public static Vec3 operator * (in Vec3 vec, in Vec3 scale)
        {
            return vec.Multiply (in scale);
        }

		/// <summary>
		/// Divide the components of the vector by a same scalar value.
		/// </summary>
		[GLBinaryOperator ("{0} / {1}")]
		[CLBinaryOperator ("{0} / {1}")]
        public static Vec3 operator / (in Vec3 vec, float scalar)
        {
            return vec.Divide (scalar);
        }

		/// <summary>
		/// Divide the two vectors componentwise.
		/// </summary>
		[GLBinaryOperator ("{0} / {1}")]
		[CLBinaryOperator ("{0} / {1}")]
        public static Vec3 operator / (in Vec3 vec, in Vec3 scale)
        {
            return vec.Divide (scale);
        }

		/// <summary>
		/// Divide a scalar by a vector.
		/// </summary>
		[GLBinaryOperator ("{0} / {1}")]
		[CLBinaryOperator ("{0} / {1}")]
        public static Vec3 operator / (float scalar, in Vec3 vec)
        {
            return new Vec3 (scalar).Divide (vec);
        }

		/// <summary>
		/// Add the two vectors together componentwise.
		/// </summary>
		[GLBinaryOperator ("{0} + {1}")]
		[CLBinaryOperator ("{0} + {1}")]
        public static Vec3 operator + (in Vec3 left, in Vec3 right)
        {
            return left.Add (right);
        }

		/// <summary>
		/// Componentwise equality comparison between the two vectors.
		/// </summary>
		[GLBinaryOperator ("{0} == {1}")]
		[CLBinaryOperator ("{0} == {1}")]
        public static bool operator == (in Vec3 left, in Vec3 right)
        {
            return left.Equals (right);
        }

		/// <summary>
		/// Componentwise inequality comparison between the two vectors.
		/// </summary>
		[GLBinaryOperator ("{0} != {1}")]
		[CLBinaryOperator ("{0} != {1}")]
        public static bool operator != (in Vec3 left, in Vec3 right)
        {
            return !left.Equals (right);
        }
    }
} 