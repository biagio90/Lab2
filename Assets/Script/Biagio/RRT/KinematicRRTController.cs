using UnityEngine;
using System.Collections;

public class KinematicRRTController : MonoBehaviour {
	public float speed = 10;
	public int numPoints = 1000;
	public int dim = 35;

	private float y = 0.5f;
	private float timeDebug = 30f;

	private bool calculating = false;
	private Vector3 destination = Vector3.zero;
	private bool go = false;

	private ArrayList path = new ArrayList();
	private int path_index;

	private rrTree tree;

	private MyLine draw = new MyLine();

	// Use this for initialization
	void Start () {
		y = transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
		if (calculating) {
			RRT ();
		} else {
			moveKinematic();
		}
	}

	private void moveKinematic() {
		if (Input.GetMouseButtonDown (0)) {
			destination = DirectionUtility.getMouseDirection ();
			destination.y = transform.position.y;
			go = true;
			calculating = true;
			tree = new rrTree ();
			tree.addNode (new rrtNode(transform.position, null) );
			draw.clean();
		} else {
			if (go && path.Count > 0) {
				destination = (Vector3) path[path_index];
				float d = Vector3.Distance(transform.position, destination);
				if (d > 0.5) {
					Vector3 direction = (destination - transform.position).normalized;
					DirectionUtility.makeKinematicMove (rigidbody, direction, speed);
				} else {
					rigidbody.velocity=Vector3.zero;
					transform.position = destination;
					path_index++;
					if(path_index == path.Count ) {go = false; path_index = 0; }
				}
			}
		}

	}

	private void RRT () {
		if (tree.getCount() > numPoints) {
			calculating = false;
			return;
		}

		if (freePath (tree.last.point, destination)) {
			Debug.DrawLine (tree.last.point, destination, Color.blue, timeDebug, false);
			rrtNode d = new rrtNode (destination, tree.last);
			tree.last.addConnection (d);
			tree.addNode (d);
			calculating = false;
			path = rrTree.optimizePath(tree.getPath());
			printPath (path);
		} else {
			addRandomNode (tree, dim);
		}
	}
	
	private bool addRandomNode (rrTree tree, int dim) {
		Vector3 newPoint = new Vector3 (myRnd (dim), y, myRnd (dim));
		createPoint (newPoint);

		rrtNode point = tree.findClosestNode (newPoint);
		if ( point != null && freePath(point.point, newPoint) ) {
			//if it is possible to reach the point from the previous point
			//add the connection
			rrtNode p2 = new rrtNode(newPoint, point);
			point.addConnection(p2);
			tree.addNode (p2);
			myDrawLine (point.point, newPoint, Color.blue);
			return true;
		}
		return true;
	}
	
	private void printPath (ArrayList path) {
		for (int i = 0; i<path.Count-1; i++) {
			myDrawLine ((Vector3)path[i], (Vector3)path[i+1], Color.red);
		}
		draw.drawMultipleLines (path, Color.red);
	}
	
	private void createPoint (Vector3 point) {
		Vector3 n = new Vector3 (point.x + 0.5f, point.y, point.z + 0.5f);
		myDrawLine (point, n, Color.green);
		draw.drawLine (point, n, Color.green);
	}
	
	private void myDrawLine (Vector3 start, Vector3 end, Color color) {
		Debug.DrawLine (start, end, color, timeDebug, false);
		draw.drawLine(start, end, color);
	}

	/*
	public bool freePath (Vector3 p1, Vector3 p2) {
		Vector3 direction = (p2 - p1).normalized;
		float sight = (p2 - p1).magnitude;
		RaycastHit hit;
		return !rigidbody.SweepTest (direction, out hit, sight);
	}
	*/

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

	private float myRnd(int dim) {
		float d = dim;
		float n = Random.Range (-d, d);
		return n;
	}
}
