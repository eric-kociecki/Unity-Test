using UnityEngine;
using System.Collections;

public class PlayerAnimationState : MonoBehaviour {

	private Animator animator;

	// Use this for initialization
	void Start () {
		animator = this.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		/*Vector3 acceleration = Vector3.zero;
		int i = 0;
		while (i < Input.accelerationEventCount) {
			AccelerationEvent accEvent = Input.GetAccelerationEvent(i);
			acceleration += accEvent.acceleration * accEvent.deltaTime;
			++i;
		}
		if (acceleration == Vector3.zero)
		{
			animator.SetBool("isWalking", false);
		}
		else
		{
			animator.SetBool("isWalking", true);
		}*/

		var horizontal = Input.GetAxis ("Horizontal");
		var vertical = Input.GetAxis ("Vertical");

		if ((vertical + horizontal) > 0)
		{
			animator.SetBool("isWalking", true);
		}
		else
		{
			animator.SetBool("isWalking", false);
		}

		if (Input.GetButtonDown("Jump"))
		{
			animator.SetBool("isJumping", true);
		}
		else if (Input.GetButtonUp("Jump"))
		{
			animator.SetBool("isJumping", false);
		}

	}
}
