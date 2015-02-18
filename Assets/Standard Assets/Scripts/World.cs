using System.Collections;
using UnityEngine;

/// <summary>
/// Stores the world data (in RAM for now) and has a WorldGen object for creating the world. Uses Chunk objects to combine blocks into meshes and add them into the game space.
/// </summary>
public class World
{
	public readonly int WorldSizeX = 64; // Edit these to adjust world size, setting them to something other than
	public readonly int WorldSizeY = 32; // a multiple of Chunk.chunkSize is untested.
	public readonly int WorldSizeZ = 64;

	public Block[, ,] Blocks; // 3D array of all blocks
    Chunk[, ,] chunks; // array of chunks

    WorldGen worldGen; // this class generates the world

	/// <summary>
	/// Stores the material and associated texture for coloring blocks.
	/// </summary>
	public Material BlockColors;

	// Use this for initialization
	public World()
    {
		BlockColors = Resources.Load<Material>("hsv");

        worldGen = new WorldGen(this);

        GenerateChunks();
	}

    void GenerateChunks()
    {
        int numChunksX = WorldSizeX / Chunk.ChunkSize;
        int numChunksY = WorldSizeY / Chunk.ChunkSize;
        int numChunksZ = WorldSizeZ / Chunk.ChunkSize;
        
        chunks = new Chunk[numChunksX,
                           numChunksY,
                           numChunksZ];

        for (int x = 0; x < numChunksX; x++)
        {
            for (int y = 0; y < numChunksY; y++)
            {
                for (int z = 0; z < numChunksZ; z++)
                {
                    GameObject go = new GameObject("Chunk" + x + "," + y + "," + z);
                    chunks[x, y, z] = go.AddComponent<Chunk>();
                    chunks[x, y, z].SetChunkCoordinates(x, y, z);
                    chunks[x, y, z].world = this;
                }
            }
        }
    }

    public Block GetBlockAt(int x, int y, int z)
    {
        if ((x < 0) || (x > WorldSizeX - 1) ||
            (y < 0) || (y > WorldSizeY - 1) ||
            (z < 0) || (z > WorldSizeZ - 1))
        {
            return new Air();
        }

        return Blocks[x, y, z];
    }

	// Update is called once per frame
	void Update ()
    {
	
	}
}
