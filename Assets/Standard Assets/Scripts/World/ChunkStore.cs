using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChunkStore
{
	protected Index maxSize;
	protected Chunk[,,] chunks;
	protected Dictionary<Index, Index> mapper;
	protected bool[,,] isChunkStored;

	public Chunk this[Index logical]
	{
		get
		{
			return GetChunk (logical);
		}
		set
		{
			AddAt (value, logical);
		}
	}

	public Chunk this[int x, int y, int z]
	{
		get
		{
			return GetChunk (new Index(x, y, z));
		}
		set
		{
			AddAt (value, new Index(x, y, z));
		}
	}

	public ChunkStore()
	{
		resizeArray (new Index(11, 11, 11));
		mapper = new Dictionary<Index, Index>();
	}

	protected bool GetPhysicalIndex(Index logical, out Index physical)
	{
		return mapper.TryGetValue (new Index(logical.X, logical.Y, logical.Z), out physical);
	}

	public Chunk GetChunk(Index logical)
	{
		Index physical;

		if (GetPhysicalIndex (logical, out physical))
		{
			return chunks[physical.X, physical.Y, physical.Z];
		}
		else
		{
			return null;
		}
	}

	/// <summary>
	/// Add the given chunk into the chunk store.
	/// </summary>
	/// <param name="newChunk">New chunk.</param>
	public void Add(Chunk newChunk)
	{
		// find an empty spot
		Index newIndex = GetEmptyIndex();

		AddAt (newChunk, newIndex);
	}

	protected void AddAt(Chunk newChunk, Index newPhysicalIndex)
	{
		// store the chunk in given index
		chunks[newPhysicalIndex.X, newPhysicalIndex.Y, newPhysicalIndex.Z] = newChunk;
		
		// map the chunk coords to the stored spot
		mapper.Add (newChunk.Location, newPhysicalIndex);

		// mark the spot as taken
		isChunkStored[newPhysicalIndex.X, newPhysicalIndex.Y, newPhysicalIndex.Z] = true;
	}

	public void Remove(Chunk oldChunk)
	{
		Index physicalIndex;

		if (GetPhysicalIndex(oldChunk.Location, out physicalIndex))
		{
			isChunkStored[physicalIndex.X,
			              physicalIndex.Y,
			              physicalIndex.Z] = false;
			Object.Destroy(oldChunk.gameObject);
			mapper.Remove(oldChunk.Location);
		}
		else
		{
			// TODO deal with this
		}

	}

	protected Index GetEmptyIndex()
	{
		for (int x = 0; x < maxSize.X; x++)
		{
			for (int y = 0; y < maxSize.Y; y++)
			{
				for (int z = 0; z < maxSize.Z; z++)
				{
					if (!isChunkStored[x, y, z])
					{
						return new Index(x, y, z);
					}
				}
			}
		}

		// ERROR! TODO: handle this better
		return new Index(-1, -1, -1);
	}

	/// <summary>
	/// Gets all chunks in no particular order. Ths indeces of the returned array do not match up with the chunk coordinates.
	/// </summary>
	/// <returns>All chunks stored.</returns>
	public Chunk[,,] GetAllChunks()
	{
		return chunks;
	}

	/// <summary>
	/// Resizes the array. Eventually this should try to preserve exisitng data. For now it just creates a new empty array of the given size.
	/// </summary>
	/// <param name="newSize">New size.</param>
	protected void resizeArray(Index newSize)
	{
		chunks = new Chunk[newSize.X, newSize.Y, newSize.Z];
		isChunkStored = new bool[newSize.X, newSize.Y, newSize.Z];
		maxSize = newSize;
	}
}
