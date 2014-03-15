using UnityEngine;
using System.Collections;

public class DubinCarModel : MonoBehaviour {

	//public float angle = 40;
	public float radius;
	public float speed = 10;

	//private float numPointsCircle = 15.0f;
	//private float precision = 1f;
	private float turnSpeed = 10000;

	//private float y = 0.5;
	private Vector3 destination;
	private bool go = false;
	private bool calcPath = false;
	private MyLine draw = new MyLine();

	private ArrayList path;
	private int path_index = 1;
	private DubinCar car;

	void Start () {
		car = new DubinCar(radius);
	}

	void Update () {
		if (Input.GetMouseButtonDown(0))
		{
			destination = DirectionUtility.getMouseDirection();
			destination.y = transform.position.y;
			calcPath = true;
		}
		if (calcPath) {
			calcPath = false;
			go = true;
			path = car.RSR (transform.position, transform.forward, destination);
			draw.drawMultipleLines(path, Color.magenta);
			if ( free (path) ) return;
			path = car.LSR (transform.position, transform.forward, destination);
			draw.drawMultipleLines(path, Color.magenta);
			if (free (path)) return;
			path = car.LSL (transform.position, transform.forward, destination);
			draw.drawMultipleLines(path, Color.magenta);
			if (free (path)) return;
			path = car.RSL (transform.position, transform.forward, destination);
			draw.drawMultipleLines(path, Color.magenta);
			if (free (path)) return;

			go = false;
		}
		else if (go && path.Count > 0) {
			destination = (Vector3) path[path_index];

			float d = Vector3.Distance(transform.position, destination);
			if (d > 2) {
				Vector3 direction = (destination - transform.position).normalized;
				correctAngle(direction);
				DirectionUtility.makeKinematicMove (rigidbody, direction, speed);
			} else {
				rigidbody.velocity=Vector3.zero;
				transform.position = destination;
				path_index++;
				if(path_index == path.Count ) {go = false; path_index = 1; }
			}
		}
	}

	private void correctAngle(Vector3 direction) {
		Quaternion _lookRotation = Quaternion.LookRotation (direction);
		//float angle = Vector3.Angle (direction, transform.forward);
		transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, turnSpeed * Time.deltaTime);

	}

	private bool free(ArrayList list) {
		return DirectionUtility.freePath (path, transform.localScale.x/3);
	}
	
}
