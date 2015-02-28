using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour {
	private GameObject player;

	void Awake()
	{
		player = GameObject.FindGameObjectWithTag ("Player");
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == player) {
			Debug.Log ("Player entered perception sphere");
		}
	}

	void OnTriggerStay(Collider other)
	{
		if (other.gameObject == player) {

			// Create a vector to the player position
			Vector3 deltaPosition = this.transform.position - player.transform.position;

			//figure out how to tell if there's terrain collision in between AI and player of y > 2

			if (deltaPosition.sqrMagnitude > 1.25f)
			{
				Debug.Log ("Player is within perception sphere, moving to intercept");

				//will need to change this when implementing A*, should follow given path instead
				this.transform.position = Vector3.MoveTowards(this.transform.position, player.transform.position, .075f);
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject == player) {
			Debug.Log ("Player has left perception sphere");
		}
	}


}
