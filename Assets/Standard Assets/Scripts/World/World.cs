using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

/// <summary>
/// Stores the world data (in RAM for now) and has a WorldGen object for creating the world. Uses Chunk objects to combine blocks into meshes and add them into the game space.
/// </summary>
public class World
{
	int renderDistance = 2; // this is measured in chunks
	int generateDistance = 2;

	Sparse3DArray<Chunk> chunks;

    WorldGen worldGen; // this class generates the world

	/// <summary>
	/// Stores the material and associated texture for coloring blocks.
	/// </summary>
	public Material BlockColors;

	public bool IsGenerating { get; set; }

	// Use this for initialization
	public World()
    {
		BlockColors = Resources.Load<Material>("hsv");


		chunks = new Sparse3DArray<Chunk>();
		int chunkArrayLength = (generateDistance * 2) + 1;
		chunks.ResizeArray(chunkArrayLength * chunkArrayLength * chunkArrayLength);

        worldGen = new WorldGen(this);

		IsGenerating = false;
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
			return chunks[chunkPosition][localPosition];
		}
    }

	// this function takes separate chunk and local coords, converts them into absolute coords, and sends them to GetBlockAt which breaks them into
	// separate chunk and local coords again. This seems like wasted effort (and it mostly is), but it handles cases where the local coords are
	// actually outside of the local coord system. E.G. localY = -1. In this case, the conversion from local to absolute and back to local will
	// correct this issue by looking at localY = 15 in the chunk below the originally requesed one
	public Block GetBlockAt(int chunkX, int chunkY, int chunkZ, int localX, int localY, int localZ)
	{
		return GetBlockAt((chunkX * Chunk.ChunkSize) + localX,
		                  (chunkY * Chunk.ChunkSize) + localY,
		                  (chunkZ * Chunk.ChunkSize) + localZ);
	}

	public Block GetBlockAt2(int chunkX, int chunkY, int chunkZ, int localX, int localY, int localZ)
	{
		Index chunkPosition = new Index(chunkX, chunkY, chunkZ);

		if (chunks[chunkPosition] == null)
		{
			return new Air();
		}
		else
		{
			if (chunkY == 1)
			{
				UnityEngine.Debug.Log("wait");
			}

			return chunks[chunkPosition][localX, localY, localZ];
		}
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

	public Index ConvertPositionToAbsoluteCoordinates(Index localPosition, Index chunkPosition)
	{
		return new Index((chunkPosition.X * Chunk.ChunkSize) + localPosition.X,
		                 (chunkPosition.Y * Chunk.ChunkSize) + localPosition.Y,
		                 (chunkPosition.Z * Chunk.ChunkSize) + localPosition.Z);
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
		chunks.Add(chunkPosition, newChunk);

		// generate the blocks in the chunk
		Benchmark.Measure(() => worldGen.GenerateChunk(newChunk), "chunk gen");

		return newChunk;
	}

	public IEnumerator UpdateChunksAround(Vector3 position, MonoBehaviour coroutineParent)
	{
		IsGenerating = true;

		Index currentChunkPosition = ConvertPositionToChunkCoordinates(position);

		yield return coroutineParent.StartCoroutine(GenerateNewChunksAround(currentChunkPosition));

		yield return coroutineParent.StartCoroutine (SetRenderStatus (currentChunkPosition));

		yield return coroutineParent.StartCoroutine(CullChunks(position));

		IsGenerating = false;
	}

	/*public IEnumerator InitialChunkGeneration(Vector3 position, MonoBehaviour coroutineParent)
	{
		IsGenerating = true;
		
		Index currentChunkPosition = ConvertPositionToChunkCoordinates(position);

		yield return coroutineParent.StartCoroutine(GenerateNewChunksAround(currentChunkPosition));

		IsGenerating = false;
	}*/

	protected IEnumerator GenerateNewChunksAround(Index currentChunkPosition)
    {
		for (int x = (currentChunkPosition.X - generateDistance); x <= (currentChunkPosition.X + generateDistance); x++)
		{
			for (int y = (currentChunkPosition.Y - generateDistance); y <= (currentChunkPosition.Y + generateDistance); y++)
			{
				for (int z = (currentChunkPosition.Z - generateDistance); z <= (currentChunkPosition.Z + generateDistance); z++)
				{
					if (chunks[x, y, z] == null)
					{
						CreateChunk(new Index(x, y, z));
						yield return null;
					}
				}	
			}
		}
	}

	protected IEnumerator SetRenderStatus(Index currentChunkPosition)
	{
		// set chunks within renderDistance to render
		for (int x = (currentChunkPosition.X - renderDistance); x <= (currentChunkPosition.X + renderDistance); x++)
		{
			for (int y = (currentChunkPosition.Y - renderDistance); y <= (currentChunkPosition.Y + renderDistance); y++)
			{
				for (int z = (currentChunkPosition.Z - renderDistance); z <= (currentChunkPosition.Z + renderDistance); z++)
				{
					chunks[x, y, z].ShouldRender = true;
					yield return null;
				}	
			}
		}

		yield return null;

		if (generateDistance > renderDistance)
		{
			// set chunks within generateDistance but outside of renderDistance to not render
			for (int x = (currentChunkPosition.X - generateDistance); x < (currentChunkPosition.X - renderDistance); x++)
			{
				for (int y = (currentChunkPosition.Y - generateDistance); y < (currentChunkPosition.Y - renderDistance); y++)
				{
					for (int z = (currentChunkPosition.Z - generateDistance); z < (currentChunkPosition.Z - renderDistance); z++)
					{
						chunks[x, y, z].ShouldRender = false;
					}	
				}
			}

			yield return null;

			for (int x = (currentChunkPosition.X + renderDistance + 1); x <= (currentChunkPosition.X + generateDistance); x++)
			{
				for (int y = (currentChunkPosition.Y + renderDistance + 1); y <= (currentChunkPosition.Y + generateDistance); y++)
				{
					for (int z = (currentChunkPosition.Z + renderDistance + 1); z <= (currentChunkPosition.Z + generateDistance); z++)
					{
						chunks[x, y, z].ShouldRender = false;
					}	
				}
			}
		}

	}

	public IEnumerator CullChunks(Vector3 position)
	{
		Index currentChunk = ConvertPositionToChunkCoordinates(position);

		Chunk[] allChunks = chunks.ToArray();

		foreach (Chunk c in allChunks)
		{
			if (c != null)
			{
				if ((Math.Abs (c.Location.X - currentChunk.X) > generateDistance) ||
				    (Math.Abs (c.Location.Y - currentChunk.Y) > generateDistance) ||
				    (Math.Abs (c.Location.Z - currentChunk.Z) > generateDistance))
				{
					chunks.Remove(c);
					UnityEngine.Object.Destroy(c.gameObject);
					yield return null;
				}
			}
		}
	}
}
