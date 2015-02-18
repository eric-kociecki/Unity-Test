using System.Collections;
using UnityEngine;

public class WorldGen
{
	System.Random rnd = new System.Random(1);
	public World world;

	public WorldGen(World newWorld)
	{
		this.world = newWorld;
		CreateWorldArray();
		PopulateWorld();
	}

	/// <summary>
	/// Creates an empty world array of the appropriate size.
	/// </summary>
	void CreateWorldArray()
	{
		world.Blocks = new Block[world.WorldSizeX, world.WorldSizeY, world.WorldSizeZ];

	}

	/// <summary>
	/// Populates the world based on the current world generation algorithm (this is exceedingly basic right now).
	/// </summary>
	void PopulateWorld()
	{
		int roll;
		int newBlockType;

		for (int x = 0; x < world.WorldSizeX; x++)
		{
			for (int z = 0; z < world.WorldSizeZ; z++)
			{
				world.Blocks[x,0,z] = CreateBlock (Stone.ID);
				for (int y = 1; y < world.WorldSizeY; y++)
				{
					newBlockType = 0;

					if (world.Blocks[x,y-1,z].GetID() > 0)
					{
						roll = rnd.Next(0, 100);

						if (roll < 30)
						{
							newBlockType = world.Blocks[x,y-1,z].GetID();
						}
						else if (roll < 45)
						{
							if (world.Blocks[x,y-1,z].GetID() == Stone.ID)
							{
								newBlockType = Dirt.ID;
							}
						}
					}

					world.Blocks[x, y, z] = CreateBlock (newBlockType);
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
			newBlock =  new Air();
			break;
		}

		return newBlock;
	}

	// Update is called once per frame
	void Update () {
	
	}
}
