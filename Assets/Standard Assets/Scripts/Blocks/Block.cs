using UnityEngine;
using System.Collections;

/// <summary>
///                           Top
///     G--------H             ^  7 North
///    /|       /|             | /
///   / |      / |             |/
///  C--------D  |   West <----+----> East
///  |  E-----|--F            /|
///  | /      | /            / |
///  |/       |/      South L  V
///  A--------B              Bottom
///
/// </summary>
public abstract class Block
{
    public enum Direction { north, east, south, west, up, down };

    protected Vector2 colorUV;

    //Base block constructor
    public Block()
    {
        colorUV = new Vector2(0, 0);
    }

    public virtual bool IsSolid(Direction direction)
    {
        switch (direction)
        {
            case Direction.north:
                return true;
            case Direction.east:
                return true;
            case Direction.south:
                return true;
            case Direction.west:
                return true;
            case Direction.up:
                return true;
            case Direction.down:
                return true;
        }

        return false;
    }

    protected virtual Vector3 ComputePointA(int x, int y, int z)
    {
        return new Vector3(x - 0.5f, y - 0.5f, z - 0.5f);
    }

    protected virtual Vector3 ComputePointB(int x, int y, int z)
    {
        return new Vector3(x + 0.5f, y - 0.5f, z - 0.5f);
    }

    protected virtual Vector3 ComputePointC(int x, int y, int z)
    {
        return new Vector3(x - 0.5f, y + 0.5f, z - 0.5f);
    }

    protected virtual Vector3 ComputePointD(int x, int y, int z)
    {
        return new Vector3(x + 0.5f, y + 0.5f, z - 0.5f);
    }

    protected virtual Vector3 ComputePointE(int x, int y, int z)
    {
        return new Vector3(x - 0.5f, y - 0.5f, z + 0.5f);
    }

    protected virtual Vector3 ComputePointF(int x, int y, int z)
    {
        return new Vector3(x + 0.5f, y - 0.5f, z + 0.5f);
    }

    protected virtual Vector3 ComputePointG(int x, int y, int z)
    {
        return new Vector3(x - 0.5f, y + 0.5f, z + 0.5f);
    }

    protected virtual Vector3 ComputePointH(int x, int y, int z)
    {
        return new Vector3(x + 0.5f, y + 0.5f, z + 0.5f);
    }

    public virtual MeshData Blockdata
         (Chunk chunk, int x, int y, int z, MeshData meshData)
    {
        meshData.useRenderDataForCol = true;

        if (!chunk.GetBlockAt(x, y + 1, z).IsSolid(Direction.down))
        {
            meshData = FaceDataUp(chunk, x, y, z, meshData);
        }

        if (!chunk.GetBlockAt(x, y - 1, z).IsSolid(Direction.up))
        {
            meshData = FaceDataDown(chunk, x, y, z, meshData);
        }

        if (!chunk.GetBlockAt(x, y, z + 1).IsSolid(Direction.south))
        {
            meshData = FaceDataNorth(chunk, x, y, z, meshData);
        }

        if (!chunk.GetBlockAt(x, y, z - 1).IsSolid(Direction.north))
        {
            meshData = FaceDataSouth(chunk, x, y, z, meshData);
        }

        if (!chunk.GetBlockAt(x + 1, y, z).IsSolid(Direction.west))
        {
            meshData = FaceDataEast(chunk, x, y, z, meshData);
        }

        if (!chunk.GetBlockAt(x - 1, y, z).IsSolid(Direction.east))
        {
            meshData = FaceDataWest(chunk, x, y, z, meshData);
        }

        return meshData;

    }

	public virtual MeshData SharedFaceData(MeshData meshData)
	{
		meshData.AddQuadTriangles();

		meshData.AddUVCoord(colorUV);
		meshData.AddUVCoord(colorUV);
		meshData.AddUVCoord(colorUV);
		meshData.AddUVCoord(colorUV);
		
		return meshData;
	}

    protected virtual MeshData FaceDataUp
        (Chunk chunk, int x, int y, int z, MeshData meshData)
    {
        meshData.AddVertex(ComputePointG(x, y, z));
        meshData.AddVertex(ComputePointH(x, y, z));
        meshData.AddVertex(ComputePointD(x, y, z));
        meshData.AddVertex(ComputePointC(x, y, z));

		SharedFaceData (meshData);
        return meshData;
    }

    protected virtual MeshData FaceDataDown
     (Chunk chunk, int x, int y, int z, MeshData meshData)
    {
        meshData.AddVertex(ComputePointA(x, y, z));
        meshData.AddVertex(ComputePointB(x, y, z));
        meshData.AddVertex(ComputePointF(x, y, z));
        meshData.AddVertex(ComputePointE(x, y, z));

		SharedFaceData (meshData);
        return meshData;
    }

    protected virtual MeshData FaceDataNorth
        (Chunk chunk, int x, int y, int z, MeshData meshData)
    {
        meshData.AddVertex(ComputePointF(x, y, z));
        meshData.AddVertex(ComputePointH(x, y, z));
        meshData.AddVertex(ComputePointG(x, y, z));
        meshData.AddVertex(ComputePointE(x, y, z));

		SharedFaceData (meshData);
        return meshData;
    }

    protected virtual MeshData FaceDataEast
        (Chunk chunk, int x, int y, int z, MeshData meshData)
    {
        meshData.AddVertex(ComputePointB(x, y, z));
        meshData.AddVertex(ComputePointD(x, y, z));
        meshData.AddVertex(ComputePointH(x, y, z));
        meshData.AddVertex(ComputePointF(x, y, z));

		SharedFaceData (meshData);
        return meshData;
    }

    protected virtual MeshData FaceDataSouth
        (Chunk chunk, int x, int y, int z, MeshData meshData)
    {
        meshData.AddVertex(ComputePointA(x, y, z));
        meshData.AddVertex(ComputePointC(x, y, z));
        meshData.AddVertex(ComputePointD(x, y, z));
        meshData.AddVertex(ComputePointB(x, y, z));

		SharedFaceData (meshData);
        return meshData;
    }

    protected virtual MeshData FaceDataWest
        (Chunk chunk, int x, int y, int z, MeshData meshData)
    {
        meshData.AddVertex(ComputePointE(x, y, z));
        meshData.AddVertex(ComputePointG(x, y, z));
        meshData.AddVertex(ComputePointC(x, y, z));
        meshData.AddVertex(ComputePointA(x, y, z));

		SharedFaceData (meshData);
        return meshData;
    }

	public abstract int GetID();
	
    public override string ToString()
    {
        return "block";
    }
}