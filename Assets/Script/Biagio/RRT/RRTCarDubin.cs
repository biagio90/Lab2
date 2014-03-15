using UnityEngine;
using System.Collections;

public class RRTCarDubin : MonoBehaviour {
	public float speed = 10;
	public float radius = 2f;
	public int dimX = 40;
	public int dimZ = 40;

	private bool calculating = false;
	private Vector3 destination = Vector3.zero;
	private bool go = false;
	private float mainY = 0.5f;

	private ArrayList path = new ArrayList();
	private int path_index;

	private MyLine draw = new MyLine();
	private rrDubinTree tree;

	private DubinCar car;
	private bool debugDubin = false;
//	private float turnSpeed = 10000;

//	private bool onePoint = true;
//	private bool twoPoint = true;

	private void inizializeTree (Vector3 source, Vector3 dest) {
		tree = new rrDubinTree ();
		rrtNode node = new rrtNode (source, null);
		node.forward = transform.forward;
		tree.addNode (node);
	}

	private void RRT () {
		//if(onePoint) { onePoint=false; addFixedPoint(new Vector3(-12f, 1.5f, 3f)); return;}
		//if(twoPoint) { twoPoint=false; addFixedPoint(new Vector3(-14f, 1.5f, -18f)); return;}

		ArrayList dubinWay = freeCarPath(tree.last.point, tree.last.forward, destination);
		draw.drawLine (tree.last.point, destination, Color.blue);
		if (dubinWay.Count > 0) {
			rrtNode d = new rrtNode (destination, tree.last);
			dubinWay.Reverse();
			d.connection = dubinWay;
			//draw.drawMultipleLines (d.connection, Color.magenta);
			d.forward = ((Vector3)dubinWay[dubinWay.Count-1] - (Vector3)dubinWay[dubinWay.Count-2]).normalized;
			tree.addNode (d);
			calculating = false;
			path = tree.getPath();
			draw.drawMultipleLines (path, Color.magenta);
		} else {
			//if(onePoint) { onePoint=false; addRandomNode (tree, dimX, dimZ); }
			//addRandomNode (tree, dimX, dimZ);


		}
	}

	private void addFixedPoint(Vector3 newPoint) {
		draw.drawLine (newPoint, new Vector3 (newPoint.x+1,newPoint.y,newPoint.z+1), Color.green);
		
		rrtNode point = tree.findClosestNode (newPoint);
		ArrayList dubinWay = freeCarPath(point.point,point.forward, newPoint);
		if ( point != null && dubinWay.Count > 0 ) {
			//if it is possible to reach the point from the previous point
			//add the connection
			rrtNode p2 = new rrtNode(newPoint, point);
			p2.forward = ((Vector3)dubinWay[dubinWay.Count-1] - (Vector3)dubinWay[dubinWay.Count-2]).normalized;
			dubinWay.Reverse();
			p2.connection = dubinWay;
			tree.addNode (p2);
			draw.drawMultipleLines(p2.connection, Color.magenta);

		}
	}

	private bool addRandomNode (rrDubinTree tree, int dimX, int dimZ) {
		Vector3 newPoint = new Vector3 (myRnd (dimX), mainY, myRnd (dimZ));
		draw.drawLine (newPoint, new Vector3 (newPoint.x+1,newPoint.y,newPoint.z+1), Color.green);

		rrtNode point = tree.findClosestNode (newPoint);
		ArrayList dubinWay = freeCarPath(point.point,point.forward, newPoint);
		if ( point != null && dubinWay.Count > 0 ) {
			//if it is possible to reach the point from the previous point
			//add the connection
			rrtNode p2 = new rrtNode(newPoint, point);
			p2.forward = ((Vector3)dubinWay[dubinWay.Count-1] - (Vector3)dubinWay[dubinWay.Count-2]).normalized;
			//conn.Reverse();
			p2.connection = dubinWay;
			tree.addNode (p2);
			draw.drawMultipleLines(p2.connection, Color.magenta);
			return true;
		}
		return true;
	}

	public ArrayList freeCarPath (Vector3 pos, Vector3 fwd, Vector3 dest) {
		ArrayList way;

		way = car.RSR (pos, fwd, dest);
		//Debug.Log (free (way) );
		//draw.drawMultipleLines(path, Color.magenta);
		if ( free (way) ) return way;
		way = car.LSR (pos, fwd, dest);
		//draw.drawMultipleLines(path, Color.magenta);
		if (free (way)) return way;
		way = car.LSL (pos, fwd, dest);
		//draw.drawMultipleLines(path, Color.magenta);
		if (free (way)) return way;
		way = car.RSL (pos, fwd, dest);
		//draw.drawMultipleLines(path, Color.magenta);
		if (free (way)) return way;

		return new ArrayList();
	}

	private bool free(ArrayList list) {
		return DirectionUtility.freePath (list, transform.localScale.x/3);
	}

	// Use this for initialization
	void Start () {
		car = new DubinCar (radius, 10, 2, debugDubin);
		mainY = transform.position.y;
	}

	void FixedUpdate () {
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
			draw.clean();
			path_index = 2;
			
			calculating = true;
			inizializeTree(transform.position, destination);
			
		} else {
			if (go && path.Count > 0) {

				Vector3 dest = (Vector3) path[path_index];
				//Debug.Log(path.Count+" "+path_index+" "+path[path_index]);
				//makeMove(dest);
				
				float d = Vector3.Distance(transform.position, dest);
				if (d > 1f) {
					Vector3 direction = (dest - transform.position).normalized;
					//correctAngle(direction);
					//DirectionUtility.makeKinematicMove (rigidbody, direction, speed);
					DirectionUtility.makeCarMove(rigidbody, transform, direction, speed);
				} else {
					rigidbody.velocity=Vector3.zero;
					//transform.position = dest;
					//Debug.Log(path.Count+" "+path_index);
					path_index++;
					if(path_index == path.Count ) {go = false; }
				}
			}
		}
		
	}
	/*
	private void correctAngle(Vector3 direction) {
		//direction.y = transform.position.y;
		Quaternion _lookRotation = Quaternion.LookRotation (direction);
		float angle = Vector3.Angle (direction, transform.forward);

		if (!(angle < 1f || angle == 90f)) {
				transform.rotation = Quaternion.Slerp (transform.rotation, _lookRotation, turnSpeed * Time.deltaTime);
		}
	}
*/

	private void correctAngle(Vector3 direction) {
		//direction.y = mainY;
		//Quaternion _lookRotation = Quaternion.LookRotation (direction);
		float angle = Vector3.Angle (direction, transform.forward);
		if (angle > 1 && !(angle>88 && angle<92)) {
			//transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, turnSpeed * Time.deltaTime);
			transform.Rotate (new Vector3 (0, angle, 0) * Time.deltaTime);
			//Debug.Log (angle);
		} else {
			//transform.Rotate (new Vector3 (0, 0, 0) * Time.deltaTime);
		}

	}

	private float myRnd(int dim) {
		float d = dim;
		float n = Random.Range (-d, d);
		return n;
	}

}
