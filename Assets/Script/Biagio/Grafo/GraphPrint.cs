using UnityEngine;
using System.Collections;

public class GraphPrint : MonoBehaviour {

	private Graph graph;

	// Use this for initialization
	void Start () {
		Area[] aree = ConvexOverlapping.divideSpaceIntoArea ();
		graph = new Graph (aree);
		graph.drawConnections ();
		graph.printNodes ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
