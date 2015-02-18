using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Dirt : Block {

    float erosionDistance = 0.4f; // this is actually the inversion of erosion. this value is the distance from the center of the block the mesh will be.

	/// <summary>
	/// Gets the block type ID. Each block type must have a unique ID.
	/// </summary>
	public static int ID = 2;

    public Dirt()
        : base()
    {
        colorUV = new Vector2((Random.value / 10) + 0.1f,
                              (Random.value / 10) + 0.8f);
    }

    protected virtual Vector3 ComputePointInnerA(int x, int y, int z)
    {
        return new Vector3(x - erosionDistance, y - erosionDistance, z - erosionDistance);
    }

    protected virtual Vector3 ComputePointInnerB(int x, int y, int z)
    {
        return new Vector3(x + erosionDistance, y - erosionDistance, z - erosionDistance);
    }

    protected virtual Vector3 ComputePointInnerC(int x, int y, int z)
    {
        return new Vector3(x - erosionDistance, y + erosionDistance, z - erosionDistance);
    }

    protected virtual Vector3 ComputePointInnerD(int x, int y, int z)
    {
        return new Vector3(x + erosionDistance, y + erosionDistance, z - erosionDistance);
    }

    protected virtual Vector3 ComputePointInnerE(int x, int y, int z)
    {
        return new Vector3(x - erosionDistance, y - erosionDistance, z + erosionDistance);
    }

    protected virtual Vector3 ComputePointInnerF(int x, int y, int z)
    {
        return new Vector3(x + erosionDistance, y - erosionDistance, z + erosionDistance);
    }

    protected virtual Vector3 ComputePointInnerG(int x, int y, int z)
    {
        return new Vector3(x - erosionDistance, y + erosionDistance, z + erosionDistance);
    }

    protected virtual Vector3 ComputePointInnerH(int x, int y, int z)
    {
        return new Vector3(x + erosionDistance, y + erosionDistance, z + erosionDistance);
    }

    protected override MeshData FaceDataUp
        (Chunk chunk, int x, int y, int z, MeshData meshData)
    {
        if (chunk.GetBlockAt(x - 1, y, z).IsSolid(Direction.east) ||
            chunk.GetBlockAt(x, y, z + 1).IsSolid(Direction.south))
        {
            meshData.AddVertex(ComputePointG(x, y, z));
        }
        else
        {
            meshData.AddVertex(ComputePointInnerG(x, y, z));
        }

        if (chunk.GetBlockAt(x + 1, y, z).IsSolid(Direction.west) ||
            chunk.GetBlockAt(x, y, z + 1).IsSolid(Direction.south))
        {
            meshData.AddVertex(ComputePointH(x, y, z));
        }
        else
        {
            meshData.AddVertex(ComputePointInnerH(x, y, z));
        }

        if (chunk.GetBlockAt(x + 1, y, z).IsSolid(Direction.west) ||
            chunk.GetBlockAt(x, y, z - 1).IsSolid(Direction.north))
        {
            meshData.AddVertex(ComputePointD(x, y, z));
        }
        else
        {
            meshData.AddVertex(ComputePointInnerD(x, y, z));
        }

        if (chunk.GetBlockAt(x - 1, y, z).IsSolid(Direction.east) ||
            chunk.GetBlockAt(x, y, z - 1).IsSolid(Direction.north))
        {
            meshData.AddVertex(ComputePointC(x, y, z));
        }
        else
        {
            meshData.AddVertex(ComputePointInnerC(x, y, z));
        }
        
        SharedFaceData(meshData);
        return meshData;
    }

    protected override MeshData FaceDataDown
     (Chunk chunk, int x, int y, int z, MeshData meshData)
    {
        if (chunk.GetBlockAt(x - 1, y, z).IsSolid(Direction.east) ||
            chunk.GetBlockAt(x, y, z - 1).IsSolid(Direction.north))
        {
            meshData.AddVertex(ComputePointA(x, y, z));
        }
        else
        {
            meshData.AddVertex(ComputePointInnerA(x, y, z));
        }

        if (chunk.GetBlockAt(x + 1, y, z).IsSolid(Direction.west) ||
            chunk.GetBlockAt(x, y, z - 1).IsSolid(Direction.north))
        {
            meshData.AddVertex(ComputePointB(x, y, z));
        }
        else
        {
            meshData.AddVertex(ComputePointInnerB(x, y, z));
        }

        if (chunk.GetBlockAt(x + 1, y, z).IsSolid(Direction.west) ||
            chunk.GetBlockAt(x, y, z + 1).IsSolid(Direction.south))
        {
            meshData.AddVertex(ComputePointF(x, y, z));
        }
        else
        {
            meshData.AddVertex(ComputePointInnerF(x, y, z));
        }

        if (chunk.GetBlockAt(x - 1, y, z).IsSolid(Direction.east) ||
            chunk.GetBlockAt(x, y, z + 1).IsSolid(Direction.south))
        {
            meshData.AddVertex(ComputePointE(x, y, z));
        }
        else
        {
            meshData.AddVertex(ComputePointInnerE(x, y, z));
        }

        SharedFaceData(meshData);
        return meshData;
    }

    protected override MeshData FaceDataNorth
        (Chunk chunk, int x, int y, int z, MeshData meshData)
    {
        if (chunk.GetBlockAt(x - 1, y, z).IsSolid(Direction.east) ||
            chunk.GetBlockAt(x, y - 1, z).IsSolid(Direction.up))
        {
            meshData.AddVertex(ComputePointE(x, y, z));
        }
        else
        {
            meshData.AddVertex(ComputePointInnerE(x, y, z));
        }

        if (chunk.GetBlockAt(x + 1, y, z).IsSolid(Direction.west) ||
            chunk.GetBlockAt(x, y - 1, z).IsSolid(Direction.up))
        {
            meshData.AddVertex(ComputePointF(x, y, z));
        }
        else
        {
            meshData.AddVertex(ComputePointInnerF(x, y, z));
        }

        if (chunk.GetBlockAt(x + 1, y, z).IsSolid(Direction.west) ||
            chunk.GetBlockAt(x, y + 1, z).IsSolid(Direction.down))
        {
            meshData.AddVertex(ComputePointH(x, y, z));
        }
        else
        {
            meshData.AddVertex(ComputePointInnerH(x, y, z));
        }

        if (chunk.GetBlockAt(x - 1, y, z).IsSolid(Direction.east) ||
            chunk.GetBlockAt(x, y + 1, z).IsSolid(Direction.down))
        {
            meshData.AddVertex(ComputePointG(x, y, z));
        }
        else
        {
            meshData.AddVertex(ComputePointInnerG(x, y, z));
        }

        SharedFaceData(meshData);
        return meshData;
    }

    protected override MeshData FaceDataEast
        (Chunk chunk, int x, int y, int z, MeshData meshData)
    {
        if (chunk.GetBlockAt(x, y + 1, z).IsSolid(Direction.down) ||
            chunk.GetBlockAt(x, y, z + 1).IsSolid(Direction.south))
        {
            meshData.AddVertex(ComputePointH(x, y, z));
        }
        else
        {
            meshData.AddVertex(ComputePointInnerH(x, y, z));
        }

        if (chunk.GetBlockAt(x, y - 1, z).IsSolid(Direction.up) ||
            chunk.GetBlockAt(x, y, z + 1).IsSolid(Direction.south))
        {
            meshData.AddVertex(ComputePointF(x, y, z));
        }
        else
        {
            meshData.AddVertex(ComputePointInnerF(x, y, z));
        }
        
        if (chunk.GetBlockAt(x, y - 1, z).IsSolid(Direction.up) ||
            chunk.GetBlockAt(x, y, z - 1).IsSolid(Direction.north))
        {
            meshData.AddVertex(ComputePointB(x, y, z));
        }
        else
        {
            meshData.AddVertex(ComputePointInnerB(x, y, z));
        }

        if (chunk.GetBlockAt(x, y, z - 1).IsSolid(Direction.north) ||
            chunk.GetBlockAt(x, y + 1, z).IsSolid(Direction.down))
        {
            meshData.AddVertex(ComputePointD(x, y, z));
        }
        else
        {
            meshData.AddVertex(ComputePointInnerD(x, y, z));
        }

        SharedFaceData(meshData);
        return meshData;
    }

    protected override MeshData FaceDataSouth
        (Chunk chunk, int x, int y, int z, MeshData meshData)
    {
        if (chunk.GetBlockAt(x - 1, y, z).IsSolid(Direction.east) ||
            chunk.GetBlockAt(x, y + 1, z).IsSolid(Direction.down))
        {
            meshData.AddVertex(ComputePointC(x, y, z));
        }
        else
        {
            meshData.AddVertex(ComputePointInnerC(x, y, z));
        }

        if (chunk.GetBlockAt(x + 1, y, z).IsSolid(Direction.west) ||
            chunk.GetBlockAt(x, y + 1, z).IsSolid(Direction.down))
        {
            meshData.AddVertex(ComputePointD(x, y, z));
        }
        else
        {
            meshData.AddVertex(ComputePointInnerD(x, y, z));
        }

        if (chunk.GetBlockAt(x + 1, y, z).IsSolid(Direction.west) ||
            chunk.GetBlockAt(x, y - 1, z).IsSolid(Direction.up))
        {
            meshData.AddVertex(ComputePointB(x, y, z));
        }
        else
        {
            meshData.AddVertex(ComputePointInnerB(x, y, z));
        }
        
        if (chunk.GetBlockAt(x - 1, y, z).IsSolid(Direction.east) ||
            chunk.GetBlockAt(x, y - 1, z).IsSolid(Direction.up))
        {
            meshData.AddVertex(ComputePointA(x, y, z));
        }
        else
        {
            meshData.AddVertex(ComputePointInnerA(x, y, z));
        }

        SharedFaceData(meshData);
        return meshData;
    }

    protected override MeshData FaceDataWest
        (Chunk chunk, int x, int y, int z, MeshData meshData)
    {
        if (chunk.GetBlockAt(x, y, z - 1).IsSolid(Direction.north) ||
            chunk.GetBlockAt(x, y + 1, z).IsSolid(Direction.down))
        {
            meshData.AddVertex(ComputePointC(x, y, z));
        }
        else
        {
            meshData.AddVertex(ComputePointInnerC(x, y, z));
        }

        if (chunk.GetBlockAt(x, y - 1, z).IsSolid(Direction.up) ||
            chunk.GetBlockAt(x, y, z - 1).IsSolid(Direction.north))
        {
            meshData.AddVertex(ComputePointA(x, y, z));
        }
        else
        {
            meshData.AddVertex(ComputePointInnerA(x, y, z));
        }

        if (chunk.GetBlockAt(x, y - 1, z).IsSolid(Direction.up) ||
            chunk.GetBlockAt(x, y, z + 1).IsSolid(Direction.south))
        {
            meshData.AddVertex(ComputePointE(x, y, z));
        }
        else
        {
            meshData.AddVertex(ComputePointInnerE(x, y, z));
        }

        if (chunk.GetBlockAt(x, y + 1, z).IsSolid(Direction.down) ||
            chunk.GetBlockAt(x, y, z + 1).IsSolid(Direction.south))
        {
            meshData.AddVertex(ComputePointG(x, y, z));
        }
        else
        {
            meshData.AddVertex(ComputePointInnerG(x, y, z));
        }

        SharedFaceData(meshData);
        return meshData;
    }

	public override int GetID()
	{
		return Dirt.ID;
	}

}
