using UnityEngine;
using System.Collections;
using Tests;

public class EnemyAI : MonoBehaviour {
	private GameObject player;
    private CharacterController enemy;
    private float verticalSpeed = 0f;
    private float jumpSpeed = 5f;
    private bool jump = false;

	void Awake()
	{
		player = GameObject.FindGameObjectWithTag ("Player");
        enemy = GetComponent<CharacterController>();
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == player) {
			Debug.Log ("Player entered perception sphere");
		}
	}

	void OnTriggerStay(Collider other)
	{
		if (other.gameObject == player) 
        {
            Point3D start = new Point3D((int)this.transform.position.x, (int)this.transform.position.y, (int)this.transform.position.z);
            Point3D end = new Point3D((int)player.transform.position.x, (int)player.transform.position.y, (int)player.transform.position.z);

            Game game = GameObject.FindGameObjectWithTag("World").GetComponent<Game>();
            World world = game.getWorld();

            SearchNode path = PathFinder.FindPath(world, start, end);
            while (path.next != null)
            {
                Debug.DrawLine(point3dToVector3(path.position), point3dToVector3(path.next.position));
                
                Point3D nextNodePosition = path.next.position;
                Vector3 nodePostionVector = point3dToVector3(nextNodePosition);

                Vector3 deltaPosition = nodePostionVector - this.transform.position;

                if (deltaPosition.sqrMagnitude > 1.25f)
                {
                    Debug.Log("Player is within perception sphere, moving to intercept");

                    //will need to change this when implementing A*, should follow given path instead
                    //this.transform.position = Vector3.MoveTowards(this.transform.position, nodePostionVector, Time.deltaTime);
                    if (enemy.isGrounded)
                    {
                        verticalSpeed = 0;
                        if (jump)
                        {
                            verticalSpeed = jumpSpeed;
                            jump = false;
                        }
                    }
                    deltaPosition.y += verticalSpeed;

                    enemy.Move(deltaPosition * Time.deltaTime);
                }


                path = path.next;
            }
		}
	}

    private Vector3 point3dToVector3(Point3D point3d)
    {
        Vector3 result = new Vector3();
        result.x = point3d.X;
        result.y = point3d.Y;
        result.z = point3d.Z;
        return result;
    }

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject == player) {
			Debug.Log ("Player has left perception sphere");
		}
	}

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //checking the sides
        if (Mathf.Abs(hit.normal.y) < 0.5) 
        {
            jump = true;
        }
    }

    


}
