using UnityEngine;
using System.Collections;


public class WorldGen
{
	System.Random rnd = new System.Random();
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
		world.WorldArray = new int[world.WorldSizeX, world.WorldSizeY, world.WorldSizeZ];
	}

	/// <summary>
	/// Populates the world based on the current world generation algorithm (this is exceedingly basic right now).
	/// </summary>
	void PopulateWorld()
	{
		int roll;

		for (int x = 0; x < world.WorldSizeX; x++)
		{
			for (int z = 0; z < world.WorldSizeZ; z++)
			{
				world.WorldArray[x,0,z] = 1;
				for (int y = 1; y < world.WorldSizeY; y++)
				{
					if (world.WorldArray[x,y-1,z] > 0)
					{
						roll = rnd.Next(0, 100);

						if (roll < 30)
						{
							world.WorldArray[x,y,z] = world.WorldArray[x,y-1,z];
						}
						else if (roll < 45)
						{
							if (world.WorldArray[x,y-1,z] == 1)
							{
								world.WorldArray[x,y,z] = 2;
							}
						}
					}
				}
			}
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
