using UnityEngine;
using System.Collections;

public class rrTree {

	private ArrayList _nodes = new ArrayList();
	//private ArrayList connection = new ArrayList();
	public rrtNode source;
	public rrtNode last;
	static private float margin = 1f;

	public DubinCar car;
	private MyLine draw = new MyLine();

	public rrTree(){
		car = new DubinCar();
	}

	public rrTree(float r){
		car = new DubinCar(r);
	}

	public void addNode( rrtNode node) 
	{
		if (nodes.Count == 0 )
			source = node;

		nodes.Add (node);
		last = node;
	}

	//Try to merge the last node of the tree with the first possible node of the OTHER tree
	public ArrayList Merge (rrTree other) {

		ArrayList nodeList = other.nodes;
		bool find = false;
		rrtNode conn = null;
		foreach (rrtNode checkNode in nodeList) {
			if (freePath (last, checkNode)) {
				conn = checkNode;
				find = true;
				break;
			}
		}

		if (!find)
			return null;

		ArrayList fwdPath = getPath ();
		ArrayList bwdPath = other.getPath (conn);
		bwdPath.Reverse ();
		foreach (Vector3 v in bwdPath) {
			fwdPath.Add(v);
		}

		return fwdPath;
	}

	static public bool freePath (rrtNode n1, rrtNode n2) {
		return freePath (n1.point, n2.point);
	}

	static public bool freePath (Vector3 from, Vector3 to) {
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

	
	public ArrayList freeCarPath (Vector3 p1, Vector3 p2, Vector3 forward) {
		ArrayList dubinPath;
		
		dubinPath = car.RSR (p1, forward, p2);
		draw.drawMultipleLines(dubinPath, Color.magenta);
		if ( free (dubinPath) ) return dubinPath;
		dubinPath = car.LSR (p1, forward, p2);
		draw.drawMultipleLines(dubinPath, Color.magenta);
		if (free (dubinPath)) return dubinPath;
		dubinPath = car.LSL (p1, forward, p2);
		draw.drawMultipleLines(dubinPath, Color.magenta);
		if (free (dubinPath)) return dubinPath;
		dubinPath = car.RSL (p1, forward, p2);
		draw.drawMultipleLines(dubinPath, Color.magenta);
		if (free (dubinPath)) return dubinPath;
		
		return new ArrayList();
	}
	
	private bool free(ArrayList list) {
		return DirectionUtility.freePath (list);
	}

	public ArrayList getCarPath()
	{
		ArrayList path = new ArrayList ();
		rrtNode node = last;
		
		path.Add (node.point);
		while (node.back != null) {
			ArrayList carPath = freeCarPath (node.back.point, node.point, node.back.forward);
			node = node.back;
			
			//path.Add (node.point);
			carPath.Reverse();
			foreach (Vector3 v in carPath) {
				path.Add (v);
			}

		}
		path.Reverse ();
		
		return path;
	}

	public ArrayList getPath()
	{
		ArrayList path = new ArrayList ();
		rrtNode node = last;

		path.Add (node.point);
		while (node.back != null) {
			node = node.back;
			path.Add (node.point);
		}
		path.Reverse ();

		return path;
	}

	static public ArrayList optimizePath (ArrayList pathOld) {
		ArrayList path = new ArrayList (pathOld);
		//eliminate intermediate points
		for (int i=0; i<path.Count-1; i++) {
			for (int j=i+1; j<path.Count; j++) {
				if (freePath((Vector3)path[i], (Vector3)path[j])){
					path.RemoveRange(i+1, j-i-1);
				}
			}
		}
		return path;
	}

	public ArrayList getPath(rrtNode node)
	{
		if (node == null) return null;
		ArrayList path = new ArrayList ();
		
		path.Add (node.point);
		while (node.back != null) {
			node = node.back;
			path.Add (node.point);
		}
		path.Reverse ();
		return path;
	}

	public rrtNode findClosestNode (Vector3 point) {
		//ArrayList reachble = getAllReachbleNode (point);
		ArrayList reachble = nodes;

		float min = 100000;
		rrtNode closest = null;
		foreach (rrtNode node in reachble) {
			float d = Vector3.Distance(node.point, point);
			if (d < min) {
				min = d;
				closest = node;
			}
		}

		return closest;
	}

	public ArrayList getAllReachbleNode (Vector3 point) {
		ArrayList reachble = new ArrayList ();
		
		foreach (rrtNode node in nodes) {
			if ( rrtAlgorithm.freePath(node.point, point) ) {
				reachble.Add(node);
			}
		}
		return reachble;
	}

	public void printTree() {
		/*for (int i = 0; i<path.Count-1; i++) {
			Debug.Log ();
		}*/
	}

	public ArrayList nodes
	{
		//set the person name
		set { this._nodes = value; }
		//get the person name 
		get { return this._nodes; }
	}

	public int getCount () {
		return nodes.Count;
	}
}
