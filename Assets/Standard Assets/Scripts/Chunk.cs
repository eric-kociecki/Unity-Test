using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class Chunk : MonoBehaviour
{
    public static int ChunkSize = 16;
    
    MeshFilter filter;
    MeshCollider meshCollider;

    public World world; // must be set before Start() runs

    private int chunkX; // all 3 of these must be set before Start() runs. Use SetChunkCoordinates()
    private int chunkY;
    private int chunkZ;

    // Use this for initialization
    void Start()
    {
        filter = gameObject.GetComponent<MeshFilter>();
        meshCollider = gameObject.GetComponent<MeshCollider>();

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
    public Block GetBlockAt(int x, int y, int z)
    {
        return world.GetBlockAt(x, y, z);
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
                    meshData = GetBlockAt(ConvertXToAbsolute(x), ConvertYToAbsolute(y), ConvertZToAbsolute(z)).Blockdata(this,
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
        return (chunkX * ChunkSize) + localX;
    }

    protected int ConvertYToAbsolute(int localY)
    {
        return (chunkY * ChunkSize) + localY;
    }

    protected int ConvertZToAbsolute(int localZ)
    {
        return (chunkZ * ChunkSize) + localZ;
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

		GetComponent<Renderer>().material = world.BlockColors;
    }

}