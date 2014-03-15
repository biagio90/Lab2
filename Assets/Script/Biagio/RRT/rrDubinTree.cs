using UnityEngine;
using System.Collections;

public class rrDubinTree {

	private ArrayList _nodes = new ArrayList();
	//private ArrayList connection = new ArrayList();
	public rrtNode source;
	public rrtNode last;
//	static private float margin = 1f;

	public void addNode( rrtNode node) 
	{
		if (nodes.Count == 0 )
			source = node;

		nodes.Add (node);
		last = node;
	}

	public ArrayList getPath()
	{
		ArrayList path = new ArrayList ();
		rrtNode node = last;

		path.Add (node.point);
		foreach(Vector3 n in node.connection){
			path.Add (n);
		}

		while (node.back != null) {
			node = node.back;
			foreach(Vector3 n in node.connection){
				path.Add (n);
			}
		}
		path.Reverse ();

		return path;
	}
	/*
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
*/
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
