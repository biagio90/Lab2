using UnityEngine;
using System.Collections;

public class MoveRobotDifferentialOld : MonoBehaviour {

	public bool go = false;
	public ArrayList path;
	private int index = 0;
	private float mainY = 1;

	private bool debug_print = true;
	private float sight = 20.0f;

	// Use this for initialization
	void Start () {
		mainY = transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
		if(go && path != null) {
			if (debug_print) {
				debug_print = false;

				string log = "";
				for (int i = 0; i<path.Count;i++) {
					log += path[i]+" ";
				}
				Debug.Log(log);


			}

			Vector3 destination = (Vector3) path[index];
			destination.y = mainY;
			float d = Vector3.Distance(transform.position, destination);
			if (d > 0.5f) {
				makeDifferentialMove (transform, rigidbody, destination, 20f, 10f);
			} else {
				Debug.Log(index);
				index++;
				transform.position = destination; //snap to destination
				rigidbody.velocity = Vector3.zero;
				rigidbody.angularVelocity = Vector3.zero;
				if(index == path.Count) {
					go = false;
					index = 0;
				}
			}
		}
	}

	public void makeDifferentialMove (Transform transform, Rigidbody rigidbody, Vector3 destination, float speed, float turnSpeed){
		//float d = Vector3.Distance(transform.position, destination);
		Vector3 direction = (destination - transform.position).normalized;
		bool c = false;
		direction = avoidCollision (direction, ref c);

		Quaternion _lookRotation = Quaternion.LookRotation (direction);
		float angle = Vector3.Angle (direction, transform.forward);
		
		if (angle < 1f || angle == 90f) {
			rigidbody.velocity = direction * speed;
		} else {
			transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, turnSpeed * Time.deltaTime);
			
		}
	}

	
	private Vector3 avoidCollision (Vector3 direction, ref bool collision) {
		Ray ray = new Ray(transform.position, direction);
		RaycastHit hit = new RaycastHit ();
		float angleR = 0.0f, angleL = 0.0f;
		//Vector3 dir = new Vector3 (direction);
		
		collision = false;
		
		while (Physics.Raycast(ray, out hit, sight))
		{
			collision = true;
			Debug.Log ( "Collision detected " + hit.collider.name);
			
			angleL -= 20.0f;
			ray.direction = Quaternion.Euler(0, angleL, 0) * direction;
		}
		
		//dir.Set (direction);
		ray.direction = direction;
		while (Physics.Raycast(ray, out hit, sight))
		{
			collision = true;
			Debug.Log ( "Collision detected " + hit.collider.name);
			
			angleR += 20.0f;
			ray.direction = Quaternion.Euler(0, angleR, 0) * direction;
		}
		
		if (collision)
			if (Mathf.Abs(angleL) < Mathf.Abs(angleR+20) )
				direction = Quaternion.Euler(0, angleL-20, 0) * direction;
		else 
			direction = Quaternion.Euler(0, angleR+20, 0) * direction;
		
		return direction;
	}

}
