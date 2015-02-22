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
		int chunkX = currentChunk.Location.X;
		int chunkY = currentChunk.Location.Y;
		int chunkZ = currentChunk.Location.Z;

		int underBlockID;
		int newBlockID;
		int roll;

		for (int localX = 0; localX < Chunk.ChunkSize; localX++)
		{
			for (int localY = 0; localY < Chunk.ChunkSize; localY++)
			{
				for (int localZ = 0; localZ < Chunk.ChunkSize; localZ++)
				{
					if ((chunkY < 0) ||
					    ((chunkY == 0) && localY == 0))
					{
						newBlockID = Stone.ID;
					}
					else
					{
						newBlockID = 0;
						underBlockID = world.GetBlockAt(chunkX, chunkY, chunkZ, localX, localY - 1, localZ).GetID();
						
						/*if (underBlockID > 0)
						{
							roll = rnd.Next(0, 100);
							
							if (roll < 30)
							{
								newBlockID = underBlockID;
							}
							else if (roll < 45)
							{
								if (underBlockID == Stone.ID)
								{
									newBlockID = Dirt.ID;
								}
							}
						}*/
					}

					currentChunk.blocks[localX, localY, localZ] = CreateBlock(newBlockID);
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
