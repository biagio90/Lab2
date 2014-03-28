using UnityEngine;
using System.Collections;

public class MoveRobot : MonoBehaviour {
	
	private AstarCreator Astar = new AstarCreator(600, 10);
	
	public float speed = 20f;
	
	public bool go = false;
	public ArrayList path;
	
	private ArrayList pathAstar;
	private bool AstarReady = false;
	private int index = 0;
	private int indexAstar = 0;
	private float mainY = 1;
	
	private bool debug_print = false;
	private MyLine draw = new MyLine();
	
	// Use this for initialization
	void Start () {
		mainY = transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
		if (debug_print) {
			debug_print = false;
			
			string log = "";
			for (int i = 0; i<path.Count;i++) {
				log += path[i]+" ";
			}
			Debug.Log(log);
			
			
		}
		
		if(go && path != null && path.Count != 0) {
			
			
			if (!AstarReady) {
				//				Debug.Log(path[index]);
				pathAstar = Astar.getPath(transform.position, (Vector3) path[index]);
				pathAstar.Reverse();
				pathAstar.Add(path[index]);
				foreach (Vector3 p in pathAstar) createPoint(p);
				AstarReady = true;
				indexAstar = 0;
				//Debug.Log("Count "+pathAstar.Count);
			}
			
			if (pathAstar == null || pathAstar.Count == 0) {
				index++;
				if(index == path.Count) {
					go = false;
					index = 0;
				}
				return;
			}
			
			Vector3 destination = (Vector3) pathAstar[indexAstar];
			//Debug.Log(indexAstar);
			
			destination.y = mainY;
			float d = Vector3.Distance(transform.position, destination);
			if (d > 0.5f) {
				//makeDifferentialMove (transform, rigidbody, destination, 15f, 10f);
				
				Vector3 direction = (destination - transform.position).normalized;
				DirectionUtility.makeKinematicMove(rigidbody, direction, speed);
			} else {
				//Debug.Log(index);
				
				transform.position = destination; //snap to destination
				rigidbody.velocity = Vector3.zero;
				rigidbody.angularVelocity = Vector3.zero;
				
				indexAstar++;
				if(indexAstar == pathAstar.Count) {
					//go = false;
					//					Debug.Log(indexAstar);
					indexAstar = 0;
					AstarReady = false;
					index++;
					if(index == path.Count) {
						go = false;
						index = 0;
					}
				}
			}
		}
	}
	
	
	
	private void createPoint (Vector3 point) {
		Vector3 n = new Vector3 (point.x + 2f, point.y, point.z + 2f);
		draw.drawLine (point, n, Color.red);
	}
	
	public void makeDifferentialMove (Transform transform, Rigidbody rigidbody, Vector3 destination, float speed, float turnSpeed){
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
	
	

}
