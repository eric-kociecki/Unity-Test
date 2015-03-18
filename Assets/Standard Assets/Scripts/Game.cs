using UnityEngine;
using System.Collections;

/// <summary>
/// This will eventually act as the parent for the world, player, enemies, etc.
/// </summary>
public class Game : MonoBehaviour {

	World world;
	GameObject player;

	// Use this for initialization
	IEnumerator Start ()
	{
		player = GameObject.Find("Player");
		player.SetActive(false);
		world = new World(player.transform.position);
		yield return StartCoroutine(world.UpdateChunksAround(player.transform.position, this));
		player.SetActive(true);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!world.IsGenerating)
		{
			StartCoroutine(world.UpdateChunksAround(player.transform.position, this));
		}
	}

	void OnApplicationQuit()
	{
		world.isRunning = false;
	}
}
