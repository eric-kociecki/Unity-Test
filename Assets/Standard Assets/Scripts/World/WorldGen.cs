using System.Collections;
using UnityEngine;

public class WorldGen
{
	System.Random rnd = new System.Random(1);
	public World world;

	public WorldGen(World newWorld)
	{
		this.world = newWorld;
	}

	public virtual void GenerateChunk(Chunk currentChunk)
	{
		int chunkY = currentChunk.Location.Y;

		int newBlockID;

		Index absolutePosition;

		float noise;

		for (int localX = 0; localX < Chunk.ChunkSize; localX++)
		{
			for (int localY = 0; localY < Chunk.ChunkSize; localY++)
			{
				for (int localZ = 0; localZ < Chunk.ChunkSize; localZ++)
				{
					if (chunkY < 0)
					{
						currentChunk.DefaultBlock = CreateBlock(Stone.ID);
						return;
					}
					else if ((chunkY == 0) && (localY == 0))
					{
						newBlockID = Stone.ID;
					}
					else
					{
						newBlockID = Air.ID;

						absolutePosition = world.ConvertPositionToAbsoluteCoordinates(new Index(localX, localY, localZ), currentChunk.Location);

						noise = Mathf.PerlinNoise(absolutePosition.X / 100f, absolutePosition.Z / 100f) * Chunk.ChunkSize;
						if ((chunkY == 0) && (localY <= noise))
						{
							newBlockID = Stone.ID;
						}
					}

					currentChunk[localX, localY, localZ] = CreateBlock(newBlockID);
				}	
			}
		}
	}

	/// <summary>
	/// Creates a block based on the given block type ID.
	/// </summary>
	/// <returns>The newly created block.</returns>
	/// <param name="type">Block type ID of desired block.</param>
	public Block CreateBlock(int type)
	{
		Block newBlock;

		switch (type)
		{
		case 1:
			newBlock = new Stone();
			break;
		case 2:
			newBlock = new Dirt();
			break;
		default:
			newBlock = new Air();
			break;
		}

		return newBlock;
	}
}
