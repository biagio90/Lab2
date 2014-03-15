using UnityEngine;
using System.Collections;

public class DirectionUtility
{
	public DirectionUtility ()
	{
	}

	public static Vector3 getMouseDirection () {
		Ray ray = (Camera.main.ScreenPointToRay(Input.mousePosition)); //create the ray
		RaycastHit hit; //create the var that will hold the information of where the ray hit
		
		if (Physics.Raycast(ray, out hit)) //did we hit something?
			if (hit.transform.tag == "sourface")
				return hit.point; //set the destinatin to the vector3 where the ground was contacted
		return Vector3.zero;
	}

	public static void makeDiscreteMove (Transform transform, Vector3 direction, float speed){
		transform.position = transform.position + speed * direction.normalized;
	}

	public static void makeKinematicMove (Rigidbody rigidbody, Vector3 direction, float speed){
		rigidbody.velocity = direction * speed;
	}
	
	public static void makeDynamicMove (Rigidbody rigidbody, PID pid, Vector3 origin, Vector3 dest){
		rigidbody.AddForce( pid.regulation(origin, dest) );
	}

	public static void makeDifferentialMove (Transform transform, Rigidbody rigidbody, Vector3 destination, float speed, float turnSpeed){
		//float d = Vector3.Distance(transform.position, destination);
		Vector3 direction = (destination - transform.position).normalized;
		Quaternion _lookRotation = Quaternion.LookRotation (direction);
		float angle = Vector3.Angle (direction, transform.forward);

		if (angle < 1f || angle == 90f) {
			rigidbody.velocity = direction * speed;
		} else {
			transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, turnSpeed * Time.deltaTime);
			
		}
	}

	public static void makeCarMove (Rigidbody rigidbody, Transform transform, Vector3 direction, float speed){
		//rigidbody.AddForce( direction * speed * 100.0f * Time.deltaTime );
		rigidbody.velocity = direction * speed;
		correctAngle(direction, transform);
		/*
		if (rigidbody.velocity.magnitude > speed)
			correctAngle(direction, transform);
		else
			transform.Rotate (new Vector3 (0, 0, 0));
			*/
	}

	static private void correctAngle(Vector3 direction, Transform transform) {
		/*float angle = Vector3.Angle (direction, transform.forward);
		if (angle > 1 && !(angle > 88 && angle < 92)) {
			transform.Rotate (new Vector3 (0, angle, 0) * Time.deltaTime);
			Debug.Log (angle);
		}*/

		Quaternion _lookRotation = Quaternion.LookRotation (direction);
		float angle = Vector3.Angle (direction, transform.forward);
		
		if (!(angle < 1f || angle == 90f)) {
			transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, 10 * Time.deltaTime);
		}
		
	}
	
	static public bool freePath(ArrayList list) {
		bool free = true;
		for (int i=0; i<list.Count-1 && free; i++) {
			if(!DirectionUtility.freePath ((Vector3)list[i], (Vector3) list[i+1])) free = false;
		}
		return free;
	}
	
	static public bool freePath (Vector3 p1, Vector3 p2) {
		Vector3 direction = (p2 - p1).normalized;
		float sight = (p2 - p1).magnitude;
		
		Ray ray = new Ray(p1, direction);
		RaycastHit hit = new RaycastHit ();
		bool collision = true;
		
		if (Physics.Raycast (ray, out hit, sight)) {
			collision = false;
			Debug.Log("Collision: "+hit.collider.name);
		}
		return collision;
	}

	
	static public bool freePath(ArrayList list, float margin) {
		bool free = true;
		for (int i=0; i<list.Count-1 && free; i++) {
			if(!DirectionUtility.freePath ((Vector3)list[i], (Vector3) list[i+1], margin)) free = false;
		}
		return free;
	}

	static public bool freePath (Vector3 from, Vector3 to, float margin) {
		Vector3 p1 = new Vector3 (from.x + margin, from.y, from.z + margin);
		Vector3 p2 = new Vector3 (to.x + margin, to.y, to.z + margin);
		
		Vector3 direction = (p2 - p1).normalized;
		float sight = (p2 - p1).magnitude;
		
		Ray ray = new Ray(p1, direction);
		RaycastHit hit = new RaycastHit ();
		bool free = true;

		if (Physics.Raycast (ray, out hit, sight)) {
			free = false;
			Debug.Log("Collision: "+hit.collider.name);
		}
		
		Vector3 p3 = new Vector3 (from.x - margin, from.y, from.z - margin);
		Vector3 p4 = new Vector3 (to.x - margin, to.y, to.z - margin);
		
		direction = (p4 - p3).normalized;
		sight = (p4 - p3).magnitude;
		
		ray = new Ray(p3, direction);
		hit = new RaycastHit ();
		
		if (Physics.Raycast (ray, out hit, sight)) {
			free = false;
			Debug.Log("Collision: "+hit.collider.name);
		}
		
		return free;
	}
}
