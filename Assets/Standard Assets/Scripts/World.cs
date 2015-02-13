using UnityEngine;
using System.Collections;

public class World : MonoBehaviour
{
	public readonly int WorldSizeX = 64; // Edit these to adjust world size, setting them to something other than
	public readonly int WorldSizeY = 32; // a multiple of Chunk.chunkSize is untested.
	public readonly int WorldSizeZ = 64;

	public int[,,] WorldArray; // used by current WorldGen, but should be replaced by usage of blocks array
    Block[, ,] blocks; // used by chunk / mesh system
    Chunk[, ,] chunks; // array of chunks

    WorldGen worldGen; // this class generates the world

	public Material blockColors;

	// Use this for initialization
	void Start ()
    {
		blockColors = Resources.Load<Material>("hsv");

        worldGen = new WorldGen(this);

        ConvertBlockTypesToBlockData();
        GenerateChunks();
	}

    void GenerateChunks()
    {
        int numChunksX = WorldSizeX / Chunk.chunkSize;
        int numChunksY = WorldSizeY / Chunk.chunkSize;
        int numChunksZ = WorldSizeZ / Chunk.chunkSize;
        
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

    // this won't be needed once the world gen us updated for the new Block class
    void ConvertBlockTypesToBlockData()
    {
        blocks = new Block[WorldSizeX, WorldSizeY, WorldSizeZ];

        for (int x = 0; x < WorldSizeX; x++)
        {
            for (int y = 0; y < WorldSizeY; y++)
            {
                for (int z = 0; z < WorldSizeZ; z++)
                {
                    if (WorldArray[x, y, z] > 0)
                    {
						if (y == 1) 
						{
							blocks[x, y, z] = new BlockStone();
						}
						else
						{
                        	blocks[x, y, z] = new Dirt();
						}
                    }
                    else
                    {
                        blocks[x, y, z] = new BlockAir();
                    }
                }
            }
        }
    }

    public Block GetBlock(int x, int y, int z)
    {
        if ((x < 0) || (x > WorldSizeX - 1) ||
            (y < 0) || (y > WorldSizeY - 1) ||
            (z < 0) || (z > WorldSizeZ - 1))
        {
            return new BlockAir();
        }

        return blocks[x, y, z];
    }

	// Update is called once per frame
	void Update ()
    {
	
	}
}
