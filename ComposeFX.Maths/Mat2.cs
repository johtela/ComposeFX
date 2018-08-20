namespace ComposeFX.Maths
{
    using System;
    using System.Text;
	using Graphics;

	/// <summary>
	/// Matrix with 2 columns and rows.
	/// </summary>
	/// Matrices are defined in column-major way as in GLSL. 
	[GLType ("mat2")]
    public readonly struct Mat2 : ISquareMat<Mat2, float>
    { 
		/// <summary>
		/// Column 0 of the matrix.
		/// </summary>
		public readonly Vec2 Column0; 
		/// <summary>
		/// Column 1 of the matrix.
		/// </summary>
		public readonly Vec2 Column1; 

		/// <summary>
		/// Initialize a matrix given its columns.
		/// </summary>
		[GLConstructor ("mat2 ({0})")]
		public Mat2 (Vec2 column0, Vec2 column1)
		{
			Column0 = column0; 
			Column1 = column1; 
		}

 		/// <summary>
		/// Initialize a matrix using the elements of another matrix.
		/// If source matrix is smaller than the created one, unspecified
		/// elements are initialized to zero.
		/// </summary>
		[GLConstructor ("mat2 ({0})")]
		public Mat2 (in Mat2 mat)
		{	
			Column0 = new Vec2 (mat.Column0);
			Column1 = new Vec2 (mat.Column1);
		}

		/// <summary>
		/// Initialize a matrix using the elements of another matrix.
		/// If source matrix is smaller than the created one, unspecified
		/// elements are initialized to zero.
		/// </summary>
		[GLConstructor ("mat2 ({0})")]
		public Mat2 (in Mat3 mat)
		{	
			Column0 = new Vec2 (mat.Column0);
			Column1 = new Vec2 (mat.Column1);
		}

		/// <summary>
		/// Initialize a matrix using the elements of another matrix.
		/// If source matrix is smaller than the created one, unspecified
		/// elements are initialized to zero.
		/// </summary>
		[GLConstructor ("mat2 ({0})")]
		public Mat2 (in Mat4 mat)
		{	
			Column0 = new Vec2 (mat.Column0);
			Column1 = new Vec2 (mat.Column1);
		}

		/// <summary>
		/// Initialize the diagonal of the matrix with a given value.
		/// The rest of the elements will be zero.
		/// </summary>
		[GLConstructor ("mat2 ({0})")]
		public Mat2 (float value)
		{	
			Column0 = new Vec2 (value, 0); 
			Column1 = new Vec2 (0, value); 
		}

 		/// <summary>
		/// Initialize all of the elements of the matrix individually.
		/// </summary>
		[GLConstructor ("mat2 ({0})")]
		public Mat2 (
			float m00, float m01, 
			float m10, float m11)
		{	
			Column0 = new Vec2 (m00, m01); 
			Column1 = new Vec2 (m10, m11); 
		}

 		/// <summary>
		/// Create matrix from a 2-dimensional array. The array is in column-major format.
		/// First dimension refers to columns, second to rows.
		/// </summary>
		public Mat2 FromArray (float[,] elems)
		{
			return new Mat2 (
				elems[0, 0], elems[0, 1], 
				elems[1, 0], elems[1, 1]);
		}

		/// <summary>
		/// Number of columns in the matrix.
		/// </summary>
		public int Columns
		{
			get { return 2; }
		}

		/// <summary>
		/// Number of rows in the matrix.
		/// </summary>
		public int Rows
		{
			get { return 2; }
		}

		/// <summary>
		/// Get/set a column by its index.
		/// </summary>
		public Vec2 this[int column]
		{
			get
			{
				switch (column)
				{	         
					case 0: return Column0;          
					case 1: return Column1; 
			        default: throw new ArgumentOutOfRangeException("column");
				}
			} 
		}

		/// <summary>
		/// Get/set a single element in the given position.
		/// </summary>
		public float this[int column, int row]
		{
			get { return this[column][row]; }
		} 
					
		/// <summary>
		/// Add two matrices together componentwise.
		/// </summary>
		[GLBinaryOperator ("{0} + {1}")]
		public Mat2 Add (in Mat2 other)
		{
			return new Mat2 (Column0 + other.Column0, Column1 + other.Column1);
		}

		/// <summary>
		/// Componentwise subtraction of two matrices.
		/// </summary>
		[GLBinaryOperator ("{0} - {1}")]
		public Mat2 Subtract (in Mat2 other)
		{
			return new Mat2 (Column0 - other.Column0, Column1 - other.Column1);
		}

		/// <summary>
		/// Multiply the matrix by a scalar componentwise.
		/// </summary>
		[GLBinaryOperator ("{0} * {1}")]
		public Mat2 Multiply (float scalar)
		{
			return new Mat2 (Column0 * scalar, Column1 * scalar);
		}

		/// <summary>
		/// Divide the matrix by a scalar componentwise.
		/// </summary>
		[GLBinaryOperator ("{0} / {1}")]
		public Mat2 Divide (float scalar)
		{
			return new Mat2 (Column0 / scalar, Column1 / scalar);
		}

		/// <summary>
		/// Multiply the matrix by a vector which has as many elements as the 
		/// matrix has columns. The result is a vector with same dimensions as the
		/// vector given as argument.
		/// </summary>
        public V Multiply<V> (in V vec) where V : struct, IVec<V, float>, IEquatable<V>
        {
            if (vec.Dimensions != Columns)
                throw new ArgumentException (
					string.Format ("Cannot multiply {0}x{1} matrix with {2}D vector", Columns, Rows, vec.Dimensions), "vec");
            return vec.FromArray (
				Column0.X * vec[0] + Column1.X * vec[1], 
				Column0.Y * vec[0] + Column1.Y * vec[1]);
        }

		/// <summary>
		/// Implementation of the <see cref="System.IEquatable{Mat2}"/> interface.
		/// </summary>
		public bool Equals (Mat2 other)
		{
			return Column0 == other.Column0 && Column1 == other.Column1;
		}

		/// <summary>
		/// The multiplication of two matrices.
		/// </summary>
		[GLBinaryOperator ("{0} * {1}")]
        public Mat2 Multiply (in Mat2 mat)
        {
            return this * mat;
        }

		/// <summary>
		/// Return the matrix transposed, i.e. rows and columns exchanged.
		/// </summary>
		[GLFunction ("transpose ({0})")]
        public Mat2 Transposed
        {
            get { return Mat.Transpose<Mat2, float> (this); }
        }

		/// <summary>
		/// Return the determinant of the matrix.
		/// </summary>
		[GLFunction ("determinant ({0})")]
        public float Determinant
        {
            get { return Mat.Determinant (this); }
        }

		/// <summary>
		/// Return the inverse matrix.
		/// </summary>
		[GLFunction ("inverse ({0})")]
        public Mat2 Inverse
        {
            get { return Mat.Inverse (this); }
        }

		/// <summary>
		/// Override the Equals method to work with matrices.
		/// </summary>
		public override bool Equals (object obj)
		{
            return obj is Mat2 && Equals ((Mat2)obj);
		}

		/// <summary>
		/// Override the hash code.
		/// </summary>
        public override int GetHashCode ()
        {
			return Column0.GetHashCode () ^ Column1.GetHashCode ();
        }

		/// <summary>
		/// Return the matrix as formatted string.
		/// </summary>
        public override string ToString ()
        {
            var sb = new StringBuilder ();
            sb.AppendLine ();
            for (int r = 0; r < 2; r++)
            {
                sb.Append ("[");
                for (int c = 0; c < 2; c++)
                    sb.AppendFormat (" {0}", this[c, r]);
                sb.AppendLine (" ]");
            }
            return sb.ToString ();
        }

		/// <summary>
		/// Componentwise subtraction of two matrices.
		/// </summary>
		[GLBinaryOperator ("{0} - {1}")]
		public static Mat2 operator - (in Mat2 left, in Mat2 right)
        {
            return left.Subtract (in right);
        }

		/// <summary>
		/// Multiply a matrix with a scalar componentwise.
		/// </summary>
		[GLBinaryOperator ("{0} * {1}")]
        public static Mat2 operator * (in float scalar, in Mat2 mat)
        {
            return mat.Multiply (scalar);
        }

		/// <summary>
		/// Multiply a matrix with a scalar componentwise.
		/// </summary>
		[GLBinaryOperator ("{0} * {1}")]
        public static Mat2 operator * (in Mat2 mat, float scalar)
        {
            return mat.Multiply (scalar);
        }

		/// <summary>
		/// Multiply two matrices together using the matrix product operation.
		/// </summary>
		[GLBinaryOperator ("{0} * {1}")]
        public static Mat2 operator * (in Mat2 left, in Mat2 right)
        {
			return new Mat2 (
				left.Column0.X * right.Column0.X + left.Column1.X * right.Column0.Y,
				left.Column0.Y * right.Column0.X + left.Column1.Y * right.Column0.Y,
				left.Column0.X * right.Column1.X + left.Column1.X * right.Column1.Y,
				left.Column0.Y * right.Column1.X + left.Column1.Y * right.Column1.Y);
        }

		/// <summary>
		/// Multiply a matrix by a Vec2 producing a vector with the same type.
		/// </summary>
		[GLBinaryOperator ("{0} * {1}")]
        public static Vec2 operator * (in Mat2 mat, in Vec2 vec)
        {
			return new Vec2 (
				mat.Column0.X * vec.X + mat.Column1.X * vec.Y,
				mat.Column0.Y * vec.X + mat.Column1.Y * vec.Y);
        }

		/// <summary>
		/// Divide a matrix by a scalar componentwise.
		/// </summary>
		[GLBinaryOperator ("{0} / {1}")]
        public static Mat2 operator / (in Mat2 mat, float scalar)
        {
            return mat.Divide (scalar);
        }

		/// <summary>
		/// Add two matrices together componentwise.
		/// </summary>
		[GLBinaryOperator ("{0} + {1}")]
        public static Mat2 operator + (in Mat2 left, in Mat2 right)
        {
            return left.Add (in right);
        }

		/// <summary>
		/// Overloaded equality operator that works with matrices.
		/// </summary>
        public static bool operator == (in Mat2 left, in Mat2 right)
        {
            return left.Equals (right);
        }

		/// <summary>
		/// Overloaded inequality operator that works with matrices.
		/// </summary>
        public static bool operator != (in Mat2 left, in Mat2 right)
        {
            return !left.Equals (right);
        }
	}
}
 