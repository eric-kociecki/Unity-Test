using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldRender : MonoBehaviour
{
	World world;
	WorldGen worldGen;

	public readonly int SubBlockResolution = 4;	// blocks are always 1m^3, this setting makes blocks be made up of N x N x N smaller sub-blocks

	// Use this for initialization
	void Start () {
		world = new World();
		worldGen = new WorldGen(world);

		GenerateObjects();
	}

	/// <summary>
	/// Generates all the GameObjects from the world data.
	/// </summary>
	void GenerateObjects()
	{
		for (int x = 0; x < world.WorldSizeX; x++)
		{
			for (int z = 0; z < world.WorldSizeZ; z++)
			{
				for (int y = 0; y < world.WorldSizeY; y++)
				{
					if (world.WorldArray[x,y,z] > 0)
					{
						CreateBlockObject(world.WorldArray[x, y, z], new Vector3(x, y, z));
					}
				}
			}
		}
	}

	Dictionary<string, bool> GetAdjacentSides(Vector3 position)
	{
		Dictionary<string, bool> adjacentBlocks = new Dictionary<string, bool>();
		
		if ((position.y < world.WorldSizeY - 1) && // if we're not at max y coord and
		    (world.WorldArray[(int)position.x, (int)position.y + 1, (int)position.z] > 0)) // there is an adacent block
		{
			adjacentBlocks["top"] = true;
		}
		else
		{
			adjacentBlocks["top"] = false;
		}
		
		if ((position.y > 0) &&	// if we're not at min y coord and
		    (world.WorldArray[(int)position.x, (int)position.y - 1, (int)position.z] > 0))	// there is an adjacent block
		{
			adjacentBlocks["bottom"] = true;
		}
		else
		{
			adjacentBlocks["bottom"] = true;
		}
		
		if ((position.x > 0) &&  // if we're not at min x coord and
		    (world.WorldArray[(int)position.x - 1, (int)position.y, (int)position.z] > 0))	// there is an adjacent block
		{
			adjacentBlocks["front"] = false;
		}
		else
		{
			adjacentBlocks["front"] = false;
		}
		
		if ((position.x < world.WorldSizeX - 1) &&  // if we're not at max x coord and
		    (world.WorldArray[(int)position.x + 1, (int)position.y, (int)position.z] > 0))	// there is an adjacent block
		{
			adjacentBlocks["back"] = true;
		}
		else
		{
			adjacentBlocks["back"] = false;
		}
		
		if ((position.z > 0) &&  // if we're not at min z coord and
		    (world.WorldArray[(int)position.x, (int)position.y, (int)position.z - 1] > 0))	// there is an adjacent block
		{
			adjacentBlocks["left"] = true;
		}
		else
		{
			adjacentBlocks["left"] = false;
		}
		
		if ((position.z < world.WorldSizeZ - 1) && // if we're not at max z coord and
		    (world.WorldArray[(int)position.x, (int)position.y, (int)position.z + 1] > 0))	// there is an adjacent block
		{
			adjacentBlocks["right"] = true;
		}
		else
		{
			adjacentBlocks["right"] = false;
		}
		
		return adjacentBlocks;
	}

	/// <summary>
	/// Creates one GameObject based on the data passed in. If the GameObject wouldn't be visible due to other blocks, nothing is generated.
	/// </summary>
	/// <param name="blockType">Block type.</param>
	/// <param name="position">Position.</param>
	void CreateBlockObject(int blockType, Vector3 position)
	{
		Vector3 precisePosition;
		Dictionary<string, bool> adjacentBlocks = GetAdjacentSides(position);
		
		for (int blockX = 0; blockX < SubBlockResolution; blockX++)
		{
			for (int blockY = 0; blockY < SubBlockResolution; blockY++)
			{
				for (int blockZ = 0; blockZ < SubBlockResolution; blockZ++)
				{
					if (((blockX == 0) && !adjacentBlocks["front"]) ||
					    ((blockX == SubBlockResolution - 1) && !adjacentBlocks["back"]) ||
					    ((blockY == 0) && !adjacentBlocks["bottom"]) ||
					    ((blockY == SubBlockResolution - 1) && !adjacentBlocks["top"]) ||
					    ((blockZ == 0) && !adjacentBlocks["left"]) ||
					    ((blockZ == SubBlockResolution - 1) && !adjacentBlocks["right"]))
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

	/// <summary>
	/// Gets a slightly randomized color based on the block type.
	/// </summary>
	/// <returns>The color.</returns>
	/// <param name="blockType">Block type.</param>
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

	/// <summary>
	/// Creates one sub-block of a block using the given parameters.
	/// </summary>
	/// <param name="blockType">Block type.</param>
	/// <param name="position">Position.</param>
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

	/// <summary>
	/// Returns the size of a sub-block based on a given sub-block resolution. E.G. resolution of 2 would be size 0.5^3, resolution of 4 would be size 0.25^3
	/// </summary>
	/// <returns>The sub block scale.</returns>
	Vector3 GetSubBlockScale()
	{
		return new Vector3(1f / SubBlockResolution, 1f / SubBlockResolution, 1f / SubBlockResolution);
	}

	// Update is called once per frame
	void Update () {
	
	}
}
