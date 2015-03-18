using UnityEngine;
using System;

public class Chunk
{
	#region Public Static Variables

    public static int ChunkSize = 8;

	#endregion
	#region Public Properties

	public Block this[Index local]
	{
		get
		{
			return this[local.X, local.Y, local.Z];
		}
		set
		{
			this[local.X, local.Y, local.Z] = value;
		}
	}

	public Block this[int localX, int localY, int localZ]
	{
		get
		{
			if (IsValidLocalCoordinates(localX, localY, localZ))
			{
				Block requested = blocks[localX, localY, localZ];
				
				if (requested != null)
				{
					return requested;
				}
				else
				{
					return DefaultBlock;
				}
			}
			else
			{
				Debug.LogError(String.Format("Invalid coordinates passed in Chunk[] getter: {0},{1},{2}", localX, localY, localZ));
				
				// consider throwing an exception here
				return DefaultBlock;
			}
		}
		set
		{
			if (IsValidLocalCoordinates(localX, localY, localZ))
			{
				blocks[localX, localY, localZ] = value;
			}
			else
			{
				Debug.LogError(String.Format("Invalid coordinates passed in Chunk[] setter: {0},{1},{2}", localX, localY, localZ));
			}
		}
	}

	public Block DefaultBlock { get; set; }

	public bool IsGenerated { get; set; }

	// must be set before adding blocks
	public Index Location { get; set; }
	
	public World ParentWorld { get; set; } // must be set before Start() runs

	//public bool ShouldRender { get; set; }

	#endregion
	#region Private/Protected Fields

	private Block[,,] blocks;
    MeshFilter meshFilter;
    MeshCollider meshCollider;
	MeshRenderer meshRenderer;
	MeshData meshData;

	protected bool needsRerender;
	protected bool hasGameObject;

	protected GameObject gameObject;

	#endregion
	#region Constructors

	public Chunk()
	{
		IsGenerated = false;
		needsRerender = false;
		hasGameObject = false;

		blocks = new Block[ChunkSize, ChunkSize, ChunkSize];
	}

	#endregion
	#region Public Methods

	public void AssignGameObject(GameObject inGameObject)
	{
		gameObject = inGameObject;
		meshFilter = gameObject.AddComponent<MeshFilter>();
		meshCollider = gameObject.AddComponent<MeshCollider>();
		meshRenderer = gameObject.AddComponent<MeshRenderer>();

		hasGameObject = true;
	}

	/// <summary>
	/// Returns the block object at the given coordinates. This must use absolute coords because it is sometimes calles from outside of Chunk in code that only knows absolute coords.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="z"></param>
	/// <returns></returns>
	public Block GetBlockAt(int x, int y, int z)
	{
		return ParentWorld.GetBlockAt(x, y, z);
	}

	// consider replacing this with a proper destructor
	public void Unload()
	{
		UnityEngine.Object.Destroy(gameObject);
	}
	
	// Updates the chunk based on its contents
	public void UpdateChunk()
	{
		meshData = new MeshData();
		
		for (int x = 0; x < ChunkSize; x++)
		{
			for (int y = 0; y < ChunkSize; y++)
			{
				for (int z = 0; z < ChunkSize; z++)
				{
					meshData = this[new Index(x, y, z)].Blockdata(this,
					                                              ConvertXToAbsolute(x),
					                                              ConvertYToAbsolute(y),
					                                              ConvertZToAbsolute(z),
					                                              meshData);
				}
			}
		}

		needsRerender = true;
	}

	#endregion
	#region Private/Protected Methods

	protected bool IsValidLocalCoordinates(int localX, int localY, int localZ)
	{
		return ((localX >= 0) && (localX < ChunkSize) &&
		        (localY >= 0) && (localY < ChunkSize) &&
		        (localZ >= 0) && (localZ < ChunkSize));
	}
    
    protected int ConvertXToAbsolute(int localX)
    {
        return (Location.X * ChunkSize) + localX;
    }

    protected int ConvertYToAbsolute(int localY)
    {
        return (Location.Y * ChunkSize) + localY;
    }

    protected int ConvertZToAbsolute(int localZ)
    {
        return (Location.Z * ChunkSize) + localZ;
    }

    // Sends the calculated mesh information
    // to the mesh and collision components
    public void RenderMesh()
    {
		if (needsRerender && IsGenerated && hasGameObject)
		{
	        meshFilter.mesh.Clear();
			meshFilter.mesh.vertices = meshData.vertices.ToArray();
			meshFilter.mesh.triangles = meshData.triangles.ToArray();
			meshFilter.mesh.uv = meshData.uv.ToArray();
			meshFilter.mesh.RecalculateNormals();

	        meshCollider.sharedMesh = null;
	        Mesh mesh = new Mesh();
	        mesh.vertices = meshData.colVertices.ToArray();
	        mesh.triangles = meshData.colTriangles.ToArray();
	        mesh.RecalculateNormals();

	        meshCollider.sharedMesh = mesh;

			meshRenderer.material = ParentWorld.BlockColors;

			needsRerender = false;
		}
    }
	#endregion
}