namespace ComposeFX.Maths
{
	public struct Plane
	{
		public readonly Vec3 Normal;
		public readonly float Distance;
		
		public Plane (in Vec3 normal, float distance)
		{
			Normal = normal;
			Distance = distance;
		}
		
		public Plane (in Vec3 p0, in Vec3 p1, in Vec3 p2)
		{
			Normal = Vec.CalculateNormal (in p0, in p1, in p2);
			Distance = -Normal.Dot (in p0);
		}
		
		public float DistanceFromPoint (in Vec3 p)
		{
			return p.Dot (in Normal) + Distance;
		}
		
		public Vec3 ProjectPoint (in Vec3 p)
		{
			return p - DistanceFromPoint (in p) * Normal;
		}

		public bool PointInside (in Vec3 p)
		{
			return DistanceFromPoint (in p) >= 0f;
		}

		public bool BoundingBoxInside (Aabb<Vec3> bb)
		{
			foreach (var p in bb.Corners)
				if (DistanceFromPoint (in p) >= 0f)
					return true;
			return false;
		}
	}
}

