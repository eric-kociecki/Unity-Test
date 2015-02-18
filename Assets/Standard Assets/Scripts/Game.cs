using UnityEngine;
using System.Collections;

/// <summary>
/// This will eventually act as the parent for the world, player, enemies, etc.
/// </summary>
public class Game : MonoBehaviour {

	World world;

	//Entity[] entities;

	// Use this for initialization
	void Start ()
	{
		world = new World();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
