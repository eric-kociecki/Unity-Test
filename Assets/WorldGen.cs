using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldGen : MonoBehaviour {

	System.Random rnd = new System.Random();
	int[,,] worldArray;
	int worldSizeX = 2;
	int worldSizeY = 1;
	int worldSizeZ = 1;

	int subBlockResolution = 1;

	// Use this for initialization
	void Start () {

		CreateWorldArray();
		PopulateWorld();
		GenerateObjects();
	}

	void GenerateObjects()
	{
		for (int x = 0; x < worldSizeX; x++)
		{
			for (int z = 0; z < worldSizeZ; z++)
			{
				for (int y = 0; y < worldSizeY; y++)
				{
					if (worldArray[x,y,z] > 0)
					{
						CreateBlockObject(worldArray[x, y, z], new Vector3(x, y, z));
					}
				}
			}
		}
	}

	Dictionary<string, bool> GetAdjacentSides(Vector3 position)
	{
		Dictionary<string, bool> adjacentBlocks = new Dictionary<string, bool>();

		if ((position.y < worldSizeY - 1) && // if we're not at max y coord and
		    (worldArray[(int)position.x, (int)position.y + 1, (int)position.z] > 0)) // there is an adacent block
		{
			adjacentBlocks["top"] = true;
		}
		else
		{
			adjacentBlocks["top"] = false;
		}

		if ((position.y > 0) &&	// if we're not at min y coord and
		    (worldArray[(int)position.x, (int)position.y - 1, (int)position.z] > 0))	// there is an adjacent block
		{
			adjacentBlocks["bottom"] = true;
		}
		else
		{
			adjacentBlocks["bottom"] = true;
		}

		if ((position.x > 0) &&  // if we're not at min x coord and
		    (worldArray[(int)position.x - 1, (int)position.y, (int)position.z] > 0))	// there is an adjacent block
		{
			adjacentBlocks["front"] = false;
		}
		else
		{
			adjacentBlocks["front"] = false;
		}

		if ((position.x < worldSizeX - 1) &&  // if we're not at max x coord and
			(worldArray[(int)position.x + 1, (int)position.y, (int)position.z] > 0))	// there is an adjacent block
		{
			adjacentBlocks["back"] = true;
		}
		else
		{
			adjacentBlocks["back"] = false;
		}

		if ((position.z > 0) &&  // if we're not at min z coord and
		    (worldArray[(int)position.x, (int)position.y, (int)position.z - 1] > 0))	// there is an adjacent block
		{
			adjacentBlocks["left"] = true;
		}
		else
		{
			adjacentBlocks["left"] = false;
		}

		if ((position.z < worldSizeZ - 1) && // if we're not at max z coord and
		    (worldArray[(int)position.x, (int)position.y, (int)position.z + 1] > 0))	// there is an adjacent block
		{
			adjacentBlocks["right"] = true;
		}
		else
		{
			adjacentBlocks["right"] = false;
		}

		return adjacentBlocks;
	}

	void CreateBlockObject(int blockType, Vector3 position)
	{
		Vector3 precisePosition;
		Dictionary<string, bool> adjacentBlocks = GetAdjacentSides(position);

		for (int blockX = 0; blockX < subBlockResolution; blockX++)
		{
			for (int blockY = 0; blockY < subBlockResolution; blockY++)
			{
				for (int blockZ = 0; blockZ < subBlockResolution; blockZ++)
				{
					if (((blockX == 0) && !adjacentBlocks["front"]) ||
					    ((blockX == subBlockResolution - 1) && !adjacentBlocks["back"]) ||
					    ((blockY == 0) && !adjacentBlocks["bottom"]) ||
					    ((blockY == subBlockResolution - 1) && !adjacentBlocks["top"]) ||
					 	((blockZ == 0) && !adjacentBlocks["left"]) ||
					    ((blockZ == subBlockResolution - 1) && !adjacentBlocks["right"]))
					{
						precisePosition = new Vector3(position.x + (GetSubBlockScale().x * blockX),
						                              position.y + (GetSubBlockScale().y * blockY),
						                              position.z + (GetSubBlockScale().z * blockZ));
						CreateSubBlock(blockType, precisePosition);
					}
				}
			}
		}
	}

	Color GetColor(int blockType)
	{
		Color blockColor;

		switch (blockType)
		{
		case 1:
			blockColor = new Color(Random.Range(0.1f, 0.2f), Random.Range(0.1f, 0.2f), Random.Range(0f, 0.2f), 1f);
			break;
		case 2:
			blockColor = new Color(Random.Range(0.1f, 0.2f), Random.Range(0.8f, 0.9f), Random.Range(0f, 0.2f), 1f);
			break;
		default:
			blockColor = new Color(1f, 0.5f, 1f);
			break;
		}

		return blockColor;
	}

	/*void CreateSubQuad(int blockType, Vector3 position, string facing)
	{
		GameObject newBlock;
		MeshRenderer newMeshRenderer;
		
		newBlock = GameObject.CreatePrimitive(PrimitiveType.Quad);
		
		newBlock.transform.position = position;
		switch (facing)
		{
		case "top":
			newBlock.transform.position.y++;
			break;
		}
		newBlock.transform.localScale = GetSubBlockScale();
		//newMeshRenderer = newBlock.AddComponent<MeshRenderer>();
		newMeshRenderer = newBlock.GetComponent<MeshRenderer>();
		
		newMeshRenderer.material.color = GetColor(blockType);
	}*/

	void CreateSubBlock(int blockType, Vector3 position)
	{
		GameObject newBlock;
		MeshRenderer newMeshRenderer;
		
		newBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
		
		newBlock.transform.position = position;
		newBlock.transform.localScale = GetSubBlockScale();
		//newMeshRenderer = newBlock.AddComponent<MeshRenderer>();
		newMeshRenderer = newBlock.GetComponent<MeshRenderer>();
		
		newMeshRenderer.material.color = GetColor(blockType);
	}

	Vector3 GetSubBlockScale()
	{
		return new Vector3(1f / subBlockResolution, 1f / subBlockResolution, 1f / subBlockResolution);
	}

	void CreateWorldArray()
	{
		worldArray = new int[worldSizeX, worldSizeY, worldSizeZ];
	}

	void PopulateWorld()
	{
		int roll;

		for (int x = 0; x < worldSizeX; x++)
		{
			for (int z = 0; z < worldSizeZ; z++)
			{
				worldArray[x,0,z] = 1;
				for (int y = 1; y < worldSizeY; y++)
				{
					if (worldArray[x,y-1,z] > 0)
					{
						roll = rnd.Next(0, 100);

						if (roll < 30)
						{
							worldArray[x,y,z] = worldArray[x,y-1,z];
						}
						else if (roll < 45)
						{
							if (worldArray[x,y-1,z] == 1)
							{
								worldArray[x,y,z] = 2;
							}
						}
					}
				}
			}
		}
	}

	Vector3 RandomPositionAtHeight(float height)
	{
		return new Vector3(rnd.Next(-20, 20),
		                   height,
		                   rnd.Next(-20, 20));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
