using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class Chunk : MonoBehaviour
{
    public static int ChunkSize = 16;
    
    MeshFilter filter;
    MeshCollider meshCollider;

	public World ParentWorld { get; set; } // must be set before Start() runs

	public Sparse3DArray<Block> blocks;

	// must be set before adding blocks
	public Index Location { get; set; }

	public bool IsGenerated { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether this chunk has ever been modified.
	/// </summary>
	/// <value><c>true</c> if this instance is original; otherwise, <c>false</c>.</value>
	public bool IsOriginal { get; set; }

	public bool ShouldRender { get; set; }

	protected bool isModified;

	public Chunk()
	{
		IsGenerated = false;
		IsOriginal = true;
		ShouldRender = false;
		isModified = true;

		blocks = new Sparse3DArray<Block>();
		blocks.ResizeArray(ChunkSize * ChunkSize * ChunkSize);
	}

    // Use this for initialization
    void Start()
    {
        filter = gameObject.GetComponent<MeshFilter>();
        meshCollider = gameObject.GetComponent<MeshCollider>();
    }

    //Update is called once per frame
    void Update()
    {
		if (ShouldRender && isModified)
		{
			UpdateChunk();
			isModified = false;
		}
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

	public Block GetLocalBlockAt(Index position)
	{
		Block requested = blocks[position];

		if (requested == null)
		{
			Debug.Log (String.Format("Requested block was null at {0}, {1}, {2} in chunk {3}, {4}, {5}.",
			                         position.X,
			                         position.Y,
			                         position.Z,
			                         Location.X,
			                         Location.Y,
			                         Location.Z));
		}

		return requested;
	}

	public void SetDefaultBlock(Block newDefault)
	{
		blocks.SetDefault(newDefault);
	}

    // Updates the chunk based on its contents
    public void UpdateChunk()
    {
        MeshData meshData = new MeshData();

        for (int x = 0; x < ChunkSize; x++)
        {
            for (int y = 0; y < ChunkSize; y++)
            {
                for (int z = 0; z < ChunkSize; z++)
                {
					meshData = GetLocalBlockAt(new Index(x, y, z)).Blockdata(this,
					                                                         ConvertXToAbsolute(x),
					                                                         ConvertYToAbsolute(y),
					                                                         ConvertZToAbsolute(z),
					                                                         meshData);
                }
            }
        }

        RenderMesh(meshData);
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
    void RenderMesh(MeshData meshData)
    {
        filter.mesh.Clear();
        filter.mesh.vertices = meshData.vertices.ToArray();
        filter.mesh.triangles = meshData.triangles.ToArray();
		filter.mesh.uv = meshData.uv.ToArray();
        filter.mesh.RecalculateNormals();

        meshCollider.sharedMesh = null;
        Mesh mesh = new Mesh();
        mesh.vertices = meshData.colVertices.ToArray();
        mesh.triangles = meshData.colTriangles.ToArray();
        mesh.RecalculateNormals();

        meshCollider.sharedMesh = mesh;

		GetComponent<Renderer>().material = ParentWorld.BlockColors;
    }
}