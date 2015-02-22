using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores the world data (in RAM for now) and has a WorldGen object for creating the world. Uses Chunk objects to combine blocks into meshes and add them into the game space.
/// </summary>
public class World
{
	int renderDistance = 1; // this is measured in chunks

	ChunkStore chunks;

    WorldGen worldGen; // this class generates the world

	/// <summary>
	/// Stores the material and associated texture for coloring blocks.
	/// </summary>
	public Material BlockColors;

	// Use this for initialization
	public World()
    {
		BlockColors = Resources.Load<Material>("hsv");

		chunks = new ChunkStore();

        worldGen = new WorldGen(this);
	}

    public Block GetBlockAt(int x, int y, int z)
    {
		Index chunkPosition = ConvertPositionToChunkCoordinates(new Vector3(x, y, z));
		Index localPosition = ConvertPositionToLocalCoordinates(new Vector3(x, y, z));

		if (chunks[chunkPosition] == null)
		{
			return new Air();
		}
		else
		{
			return chunks[chunkPosition].GetLocalBlockAt(localPosition);
		}
    }

	public Block GetBlockAt(int chunkX, int chunkY, int chunkZ, int localX, int localY, int localZ)
	{
		return GetBlockAt((chunkX * Chunk.ChunkSize) + localX,
		                  (chunkY * Chunk.ChunkSize) + localY,
		                  (chunkZ * Chunk.ChunkSize) + localZ);
	}

	public Index ConvertPositionToChunkCoordinates(Vector3 position)
	{
		return new Index((int)Math.Floor(position.x / Chunk.ChunkSize),
						 (int)Math.Floor(position.y / Chunk.ChunkSize),
		                 (int)Math.Floor(position.z / Chunk.ChunkSize));
	}

	public Index ConvertPositionToLocalCoordinates(Vector3 position)
	{
		Index chunkCoords = ConvertPositionToChunkCoordinates(position);

		return new Index((int)position.x - (chunkCoords.X * Chunk.ChunkSize),
		                 (int)position.y - (chunkCoords.Y * Chunk.ChunkSize),
		                 (int)position.z - (chunkCoords.Z * Chunk.ChunkSize));
	}

	protected Chunk CreateChunk(Index chunkPosition)
	{
		// create the game object and add the Chunk script
		GameObject go = new GameObject(String.Format ("Chunk{0},{1},{2}", chunkPosition.X, chunkPosition.Y, chunkPosition.Z));
		Chunk newChunk = go.AddComponent<Chunk>();

		// pass some needed info to the chunk
		newChunk.Location = chunkPosition;
        newChunk.ParentWorld = this;

		// this must be added to the ChunkStore before generating blocks in the chunk
		chunks.Add(newChunk);

		// generate the blocks in the chunk
		worldGen.GenerateChunk(newChunk);

		return newChunk;
	}

    public void GenerateNewChunksAround(Vector3 position)
    {
        Index currentChunk = ConvertPositionToChunkCoordinates(position);
		for (int x = (currentChunk.X - renderDistance); x <= (currentChunk.X + renderDistance); x++)
		{
			for (int y = (currentChunk.Y - renderDistance); y <= (currentChunk.Y + renderDistance); y++)
			{
				for (int z = (currentChunk.Z - renderDistance); z <= (currentChunk.Z + renderDistance); z++)
				{
					if (chunks[x, y, z] == null)
					{
						CreateChunk(new Index(x, y, z));
					}
				}	
			}
		}
	}

	public void CullChunks(Vector3 position)
	{
		Index currentChunk = ConvertPositionToChunkCoordinates(position);

		Chunk[,,] allChunks = chunks.GetAllChunks();

		foreach (Chunk c in allChunks)
		{
			if (c != null)
			{
				if ((Math.Abs (c.Location.X - currentChunk.X) > renderDistance) ||
				    (Math.Abs (c.Location.Y - currentChunk.Y) > renderDistance) ||
				    (Math.Abs (c.Location.Z - currentChunk.Z) > renderDistance))
				{
					chunks.Remove(c);
				}
			}
		}
	}
}
