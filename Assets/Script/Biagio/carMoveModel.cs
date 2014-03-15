using UnityEngine;
using System.Collections;

public class carMoveModel : MonoBehaviour {
	public float speed = 10;
	public int numPoints = 1000;
	public int dim = 35;
	public float radius;
	
	private float y = 0.5f;
	private float timeDebug = 30f;
	
	private bool calculating = false;
	private Vector3 destination = Vector3.zero;
	private bool go = false;
	
	private ArrayList path = new ArrayList();
	private int path_index;
	
	private rrDubinTree tree;
	private DubinCar car;

	private MyLine draw = new MyLine();
	
	// Use this for initialization
	void Start () {
		y = transform.position.y;
		car = new DubinCar(radius);
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
			//go = true;
			calculating = true;
			tree = new rrDubinTree ();
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

		ArrayList dubinConn = freePath (tree.last.point, destination);
		if (dubinConn.Count > 0) {
			Debug.DrawLine (tree.last.point, destination, Color.blue, timeDebug, false);
			rrtNode d = new rrtNode (destination, tree.last);
			//d.connection = dubinConn;
			tree.addNode (d);
			calculating = false;
			//path = rrTree.optimizePath(tree.getPath());
			path = tree.getPath();
			//printPath (path);
		} else {
			addRandomNode (tree, dim);
		}
	}

	
	private bool addRandomNode (rrDubinTree tree, int dim) {
		Vector3 newPoint = new Vector3 (myRnd (dim), y, myRnd (dim));
		createPoint (newPoint);
		
		rrtNode point = tree.findClosestNode (newPoint);
		ArrayList dubinConn = freePath (point.point, newPoint);
		if ( point != null &&  dubinConn.Count > 0) {
			//if it is possible to reach the point from the previous point
			//add the connection
			rrtNode p2 = new rrtNode(newPoint, point);
			//p2.connection = dubinConn;
			tree.addNode (p2);
			//myDrawLine (point.point, newPoint, Color.blue);
			return true;
		}
		return true;
	}

	private void createPoint (Vector3 point) {
		Vector3 n = new Vector3 (point.x + 0.5f, point.y, point.z + 0.5f);
		draw.drawLine (point, n, Color.green);
	}

	public ArrayList freePath (Vector3 p1, Vector3 p2) {
		ArrayList dubinPath;

		dubinPath = car.RSR (transform.position, transform.forward, destination);
		draw.drawMultipleLines(path, Color.magenta);
		if ( free (dubinPath) ) return dubinPath;
		dubinPath = car.LSR (transform.position, transform.forward, destination);
		draw.drawMultipleLines(path, Color.magenta);
		if (free (dubinPath)) return dubinPath;
		dubinPath = car.LSL (transform.position, transform.forward, destination);
		draw.drawMultipleLines(path, Color.magenta);
		if (free (dubinPath)) return dubinPath;
		dubinPath = car.RSL (transform.position, transform.forward, destination);
		draw.drawMultipleLines(path, Color.magenta);
		if (free (dubinPath)) return dubinPath;

		return new ArrayList();
	}

	private bool free(ArrayList list) {
		return DirectionUtility.freePath (list, transform.localScale.x/3);
	}

	private float myRnd(int dim) {
		float d = dim;
		float n = Random.Range (-d, d);
		return n;
	}
}
