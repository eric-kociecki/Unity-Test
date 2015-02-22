/// <summary>
/// A simple data structure to combine three integer coordinates together. This is similar to how we use Vector3, but for integers.
/// </summary>
public struct Index
{
	public int X;
	public int Y;
	public int Z;
	
	public Index (int x, int y, int z)
	{
		X = x;
		Y = y;
		Z = z;
	}
}
