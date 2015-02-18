using UnityEngine;
using System.Collections;

public class Air : Block
{
	/// <summary>
	/// Gets the block type ID. Each block type must have a unique ID.
	/// </summary>
	public static int ID = 0;

    public Air()
        : base()
    {

    }

    public override MeshData Blockdata
        (Chunk chunk, int x, int y, int z, MeshData meshData)
    {
        return meshData;
    }

    public override bool IsSolid(Block.Direction direction)
    {
        return false;
    }

	public override int GetID()
	{
		return Air.ID;
	}

    public override string ToString()
    {
        return "block air";
    }
}