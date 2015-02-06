using UnityEngine;
using System.Collections;

public class World : MonoBehaviour
{
	public readonly int WorldSizeX = 32; // Edit these to adjust world size, setting them to something other than
	public readonly int WorldSizeY = 32; // a multiple of Chunk.chunkSize is untested.
	public readonly int WorldSizeZ = 32;

	public int[,,] WorldArray; // used by current WorldGen, but should be replaced by usage of blocks array
    Block[, ,] blocks; // used by chunk / mesh system
    Chunk[, ,] chunks; // array of chunks

    WorldGen worldGen; // this class generates the world

	// Use this for initialization
	void Start ()
    {
        worldGen = new WorldGen(this);

        ConvertBlockTypesToBlockData();
        GenerateChunks();
	}

    void GenerateChunks()
    {
        int chunkSizeX = WorldSizeX / Chunk.chunkSize;
        int chunkSizeY = WorldSizeY / Chunk.chunkSize;
        int chunkSizeZ = WorldSizeZ / Chunk.chunkSize;

        chunks = new Chunk[WorldSizeX / Chunk.chunkSize,
                           WorldSizeY / Chunk.chunkSize,
                           WorldSizeZ / Chunk.chunkSize];

        for (int x = 0; x < chunkSizeX; x++)
        {
            for (int y = 0; y < chunkSizeY; y++)
            {
                for (int z = 0; z < chunkSizeZ; z++)
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
                        blocks[x, y, z] = new Block();
                    }
                    else
                    {
                        blocks[x, y, z] = new BlockAir();
                    }
                }
            }
        }
    }

    public Block GetBlock(Vector3 coords)
    {
        return GetBlock((int)coords.x, (int)coords.y, (int)coords.z);
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
