using UnityEngine;
using System.Collections;

public class StaticGuarding : MonoBehaviour {

	private MyLine draw = new MyLine();

	private MoveRobot[] robotsObject;
	//private Vector3[] robots;

	// Use this for initialization
	void Start () {
		Area[] aree = ConvexOverlapping.divideSpaceIntoArea();
		Graph graph = new Graph(aree) ;
		Vector3[] robots = findtag("robot");
		//findtag("robot");

		Node[] nodes = cooperativesearch_paths (graph);
		Debug.Log ("Num areas: "+nodes.Length);

		for (int i=0; i<robots.Length && i<nodes.Length; i++) {
			createPoint(nodes[i].area.center3D);
			ArrayList path = new ArrayList ();
			path.Add (nodes[i].area.center3D);
			robotsObject [i].path = path;
		}
		for (int i=0; i<robots.Length && i<nodes.Length; i++)
			robotsObject [i].go = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	private Node[] cooperativesearch_paths(Graph graph){
		ArrayList nodesforVRP = new ArrayList();
		int j = 0;
		for(int i = graph.getnumnodes(); i>1; i =graph.getnumnodes()){
			Node nodetopick = graph.findmaxconnection (); //return the node with maximum connections
			ArrayList list = nodetopick.connection;
			foreach (Node nod in list) {//è pseudo codice perchè non ho capito come prendere il nodo dall'arraylist
				//Debug.Log (nod.area.center3D);			
				graph.removenode (nod);	
			}
			graph.removenode (nodetopick);
			graph.constructgraph ();
			nodesforVRP.Add(nodetopick);
			j++;
		}
		
		nodesforVRP.Add (graph.nodes[0]);
		
		Node[] ret = new Node[nodesforVRP.Count];
		for (int i=0; i<nodesforVRP.Count; i++) {
			ret[i] = (Node) nodesforVRP[i];
		}
		
		return ret;
	}

	Vector3[] findtag(string tag){
		GameObject[] gos;
		gos = GameObject.FindGameObjectsWithTag(tag);
		//Debug.Log (gos[0].name+" "+gos[1].name);
		if(tag == "robot") robotsObject = new MoveRobot[gos.Length];
		Vector3[] tagpositions = new Vector3[gos.Length];
		
		int i = 0;
		foreach (GameObject go in gos) {
			if(tag == "robot") robotsObject[i] = go.GetComponent<MoveRobot>();
			tagpositions[i] = go.transform.position;
			i++;
		}
		return tagpositions;
	}

	
	private void createPoint (Vector3 point) {
		Vector3 n = new Vector3 (point.x + 2f, point.y, point.z + 2f);
		draw.drawLine (point, n, Color.red);
	}
}
