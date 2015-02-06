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

    public Block GetBlock(Vector3 local)
    {
        return world.GetBlock(ConvertToAbsolute(local));
    }

    public Block GetBlock(int localX, int localY, int localZ)
    {
        return world.GetBlock((chunkX * chunkSize) + localX,
                              (chunkY * chunkSize) + localY,
                              (chunkZ * chunkSize) + localZ);
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
                    meshData = GetBlock(x, y, z).Blockdata(this,
                                                           (chunkX * chunkSize) + x,
                                                           (chunkY * chunkSize) + y,
                                                           (chunkZ * chunkSize) + z,
                                                           meshData);

                }
            }
        }

        RenderMesh(meshData);
    }

    Vector3 ConvertToAbsolute(Vector3 local)
    {
        return new Vector3((chunkX * chunkSize) + local.x,
                           (chunkY * chunkSize) + local.y,
                           (chunkZ * chunkSize) + local.z);
    }

    // Sends the calculated mesh information
    // to the mesh and collision components
    void RenderMesh(MeshData meshData)
    {
        filter.mesh.Clear();
        filter.mesh.vertices = meshData.vertices.ToArray();
        filter.mesh.triangles = meshData.triangles.ToArray();
		filter.mesh.uv = meshData.uv.ToArray();

        coll.sharedMesh = null;
        Mesh mesh = new Mesh();
        mesh.vertices = meshData.colVertices.ToArray();
        mesh.triangles = meshData.colTriangles.ToArray();
        mesh.RecalculateNormals();

        coll.sharedMesh = mesh;

		renderer.material = world.blockColors;
    }

}