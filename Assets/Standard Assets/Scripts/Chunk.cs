using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class Chunk : MonoBehaviour
{
    public static int chunkSize = 16;
    public bool update = true;

    MeshFilter filter;
    MeshCollider coll;
	MeshRenderer renderer;

	public Material material;

    public World world; // must be set before Start() runs

    public int chunkX; // all 3 of these must be set before Start() runs. Use SetChunkCoordinates()
    public int chunkY;
    public int chunkZ;

    // Use this for initialization
    void Start()
    {
        filter = gameObject.GetComponent<MeshFilter>();
        coll = gameObject.GetComponent<MeshCollider>();
		renderer = gameObject.GetComponent<MeshRenderer>();

        UpdateChunk();
    }

    public void SetChunkCoordinates(int x, int y, int z)
    {
        chunkX = x;
        chunkY = y;
        chunkZ = z;
    }

    //Update is called once per frame
    void Update()
    {

    }
    
    /// <summary>
    /// Returns the block object at the given coordinates. This must use absolute coords because it is sometimes calles from outside of Chunk in code that only knows absolute coords.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public Block GetBlock(int x, int y, int z)
    {
        return world.GetBlock(x, y, z);
    }

    // Updates the chunk based on its contents
    public void UpdateChunk()
    {
        MeshData meshData = new MeshData();

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    meshData = GetBlock(ConvertXToAbsolute(x), ConvertYToAbsolute(y), ConvertZToAbsolute(z)).Blockdata(this,
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
        return (chunkX * chunkSize) + localX;
    }

    protected int ConvertYToAbsolute(int localY)
    {
        return (chunkY * chunkSize) + localY;
    }

    protected int ConvertZToAbsolute(int localZ)
    {
        return (chunkZ * chunkSize) + localZ;
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

        coll.sharedMesh = null;
        Mesh mesh = new Mesh();
        mesh.vertices = meshData.colVertices.ToArray();
        mesh.triangles = meshData.colTriangles.ToArray();
        mesh.RecalculateNormals();

        coll.sharedMesh = mesh;

		renderer.material = world.blockColors;
    }

}