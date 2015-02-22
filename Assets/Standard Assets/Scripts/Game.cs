using UnityEngine;
using System.Collections;

/// <summary>
/// This will eventually act as the parent for the world, player, enemies, etc.
/// </summary>
public class Game : MonoBehaviour {

	World world;
	GameObject player;

	int frameCycle = 0;

	// Use this for initialization
	void Start ()
	{
		world = new World();
		player = GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update ()
	{
		frameCycle++;

		// this switch is for tasks that don't need to run every frame
		switch (frameCycle)
		{
		case 1:
			world.GenerateNewChunksAround(player.transform.position);
			break;
		case 15:
			world.CullChunks(player.transform.position);
			break;
		case 30:
			frameCycle = 0;
			break;
		}
	}


}
