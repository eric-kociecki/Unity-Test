using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

/// <summary>
/// Stores the world data (in RAM for now) and has a WorldGen object for creating the world. Uses Chunk objects to combine blocks into meshes and add them into the game space.
/// </summary>
public class World
{
	int renderDistance = 10; // this is measured in chunks

	Sparse3DArray<Chunk> chunks;

    WorldGen worldGen; // this class generates the world

	Index currentPlayerPosition;
	System.Object currentPlayerPositionLock = new System.Object();

	Queue<Chunk> newChunks = new Queue<Chunk>();
	System.Object newChunksLock = new System.Object();

	Thread chunkGenThread;
	public bool isRunning = true;
	bool isSpinning = false;

	Thread chunkUpdateThread;


	bool isStillAlive = true;

	/// <summary>
	/// Stores the material and associated texture for coloring blocks.
	/// </summary>
	public Material BlockColors;

	public bool IsGenerating { get; set; }

	// Use this for initialization
	public World(Vector3 playerPosition)
    {
		currentPlayerPosition = ConvertPositionToChunkCoordinates(playerPosition);

		BlockColors = Resources.Load<Material>("hsv");

		chunks = new Sparse3DArray<Chunk>();
		int chunkArrayLength = (renderDistance * 2) + 1;
		chunks.ResizeArray(chunkArrayLength * chunkArrayLength * chunkArrayLength);

		AssignGameObjects();

		// temporarily set gen distance low so we can get the game started quickly
		//generateDistance = 1;

        worldGen = new WorldGen(this);
		chunkGenThread = new Thread(new ThreadStart(ChunkGeneratorThread));
		chunkGenThread.Start();

		chunkUpdateThread = new Thread(new ThreadStart(ChunkUpdater));
		//chunkUpdateThread.Start ();

		/*while (!isSpinning)
		{

		}*/

		//generateDistance = 6;

		IsGenerating = false;
	}

    public Block GetBlockAt(int x, int y, int z)
    {
		Index chunkPosition = ConvertPositionToChunkCoordinates(new Vector3(x, y, z));

		if (chunks[chunkPosition] == null)
		{
			return new Air();
		}
		else
		{
			Index localPosition = ConvertPositionToLocalCoordinates(new Vector3(x, y, z));

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

	public static Index ConvertPositionToChunkCoordinates(Vector3 position)
	{
		return new Index((int)Math.Floor(position.x / Chunk.ChunkSize),
						 (int)Math.Floor(position.y / Chunk.ChunkSize),
		                 (int)Math.Floor(position.z / Chunk.ChunkSize));
	}

	public static Index ConvertPositionToLocalCoordinates(Vector3 position)
	{
		Index chunkCoords = ConvertPositionToChunkCoordinates(position);

		return new Index((int)position.x - (chunkCoords.X * Chunk.ChunkSize),
		                 (int)position.y - (chunkCoords.Y * Chunk.ChunkSize),
		                 (int)position.z - (chunkCoords.Z * Chunk.ChunkSize));
	}

	public static Index ConvertPositionToAbsoluteCoordinates(Index localPosition, Index chunkPosition)
	{
		return new Index((chunkPosition.X * Chunk.ChunkSize) + localPosition.X,
		                 (chunkPosition.Y * Chunk.ChunkSize) + localPosition.Y,
		                 (chunkPosition.Z * Chunk.ChunkSize) + localPosition.Z);
	}

	protected Chunk CreateChunk(Index chunkPosition)
	{
		Chunk newChunk;

		// create the game object and add the Chunk script
		lock (newChunksLock)
		{
			newChunk = new Chunk();
		}

		newChunks.Enqueue(newChunk);

		// pass some needed info to the chunk
		newChunk.Location = chunkPosition;
        newChunk.ParentWorld = this;

		// generate the blocks in the chunk
		//Benchmark.Measure(() => worldGen.GenerateChunk(newChunk), "chunk gen");
		worldGen.GenerateChunk(newChunk);

		chunks.Add(chunkPosition, newChunk);

		return newChunk;
	}

	public IEnumerator UpdateChunksAround(Vector3 position, MonoBehaviour coroutineParent)
	{
		IsGenerating = true;

		isStillAlive = true;

		lock (currentPlayerPositionLock)
		{
			currentPlayerPosition = ConvertPositionToChunkCoordinates(position);
		}

		AssignGameObjects();

		//yield return coroutineParent.StartCoroutine(CullChunks(position));

		yield return coroutineParent.StartCoroutine(UpdateChunkMeshes());
		yield return null;

		IsGenerating = false;
	}

	public IEnumerator UpdateChunkMeshes()
	{
		Chunk[] allChunks;

		try
		{
			allChunks = chunks.ToArray();
		}
		catch
		{
			UnityEngine.Debug.Log("Oh noes!");
			allChunks = new Chunk[1];
		}
		
		foreach (Chunk c in allChunks)
		{
			if (c != null)
			{
				c.RenderMesh();
				//yield return null;
			}
		}

		yield return null;
	}

	public void CullChunks(Vector3 position)
	{
		Index currentChunk = ConvertPositionToChunkCoordinates(position);

		Chunk[] allChunks = chunks.ToArray();

		foreach (Chunk c in allChunks)
		{
			if (c != null)
			{
				if ((Math.Abs (c.Location.X - currentChunk.X) > renderDistance) ||
				    (Math.Abs (c.Location.Y - currentChunk.Y) > renderDistance) ||
				    (Math.Abs (c.Location.Z - currentChunk.Z) > renderDistance))
				{
					chunks.Remove(c);
					c.Unload();
				}
			}
		}
	}

	public void ChunkGeneratorThread()
	{
		Index currentChunkPosition;
		bool chunksGenned;
		int deathCount = 0;

		while (isRunning)
		{
			chunksGenned = false;

			lock (currentPlayerPositionLock)
			{
				currentChunkPosition = currentPlayerPosition;
			}

			for (int x = 0; x <= renderDistance; x++)
			{
				for (int y = 0; y <= renderDistance; y++)
				{
					for (int z = 0; z <= renderDistance; z++)
					{

						for (int xSign = -1; xSign < 2; xSign += 2)
						{
							for (int ySign = -1; ySign < 2; ySign += 2)
							{
								for (int zSign = -1; zSign < 2; zSign += 2)
								{
									if (CreateChunkIfNotNull(new Index(currentChunkPosition.X + (x * xSign),
									                                   currentChunkPosition.Y + (y * ySign),
									                                   currentChunkPosition.Z + (z * zSign))))
									{
										chunksGenned = true;
									}

									if (!isRunning)
									{
										UnityEngine.Debug.Log("Closing chunk gen thread.");
										return;
									}
								}
							}
						}

					}	
				}
			}

			/*for (int x = (currentChunkPosition.X - generateDistance); x <= (currentChunkPosition.X + generateDistance); x++)
			{
				for (int y = (currentChunkPosition.Y - generateDistance); y <= (currentChunkPosition.Y + generateDistance); y++)
				{
					for (int z = (currentChunkPosition.Z - generateDistance); z <= (currentChunkPosition.Z + generateDistance); z++)
					{
						if (chunks[x, y, z] == null)
						{
							isSpinning = false;
							chunksGenned++;
							CreateChunk(new Index(x, y, z));
						}
					}	
				}
			}*/

			if (!chunksGenned)
			{
				//isSpinning = true;
				Thread.Sleep(500);
			}

			if (isStillAlive)
			{
				deathCount = 0;
				isStillAlive = false;
			}
			/*else
			{
				deathCount++;
				if (deathCount > 100)
				{

					isRunning = false;
				}
			}*/


		}

		UnityEngine.Debug.Log("Closing chunk gen thread.");
	}

	private bool CreateChunkIfNotNull(Index location)
	{
		if (chunks[location] == null)
		{
			CreateChunk(location);
			return true;
		}

		return false;
	}

	private void ChunkUpdater()
	{
		int deathCount = 0;

		while (isRunning)
		{
			CullChunks(new Vector3(0f, 0f, 0f));
			//UpdateChunkMeshes ();

			Thread.Sleep(15);
		}

		UnityEngine.Debug.Log("Closing chunk update thread.");
	}

	public void Quit()
	{
		isRunning = false;

		Thread.Sleep(100);
	}

	protected void AssignGameObjects()
	{
		int i = 0;
		Chunk c;

		while ((newChunks.Count > 0) && (i < 100000))
		{
			lock (newChunksLock)
			{
				c = newChunks.Dequeue();
				c.AssignGameObject(new GameObject(String.Format("Chunk {0}, {1}, {2}", c.Location.X, c.Location.Y, c.Location.Z)));
				c.IsGenerated = true;
				i++;
			}
		}

		UnityEngine.Debug.Log(i.ToString());
	}
}
