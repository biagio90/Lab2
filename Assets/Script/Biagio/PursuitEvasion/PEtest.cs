using UnityEngine;
using System.Collections;

public class PEtest : MonoBehaviour {

	private int maxIteration = 100000;

	private Graph graph;
	private Vector3[] robots;
	private PEalgorithm pe;

	private int iteration=0;
	private bool end = false;

	// Use this for initialization
	void Start () {
		Area[] aree = ConvexOverlapping.divideSpaceIntoArea ();
		graph = new Graph (aree);
		graph.drawConnections ();
		graph.printNodes ();
		//Debug.Log ();

		robots = findtag("robot");

		int R = robots.Length;
		int[] r = new int[R];
		for (int i=0; i<R; i++) {
			r[i] = graph.findInGraph (robots [i]);
			Debug.Log ("robot "+r[i]);
		}


		pe = new PEalgorithm (graph, r);
		//Path path = pe.PEstep (maxIteration);

		//path.print ();
		pe.inizializzation ();
	}
	
	// Update is called once per frame
	void Update () {
		if (pe.Vbest > 0 && iteration < maxIteration) {
			iteration++;
			pe.PEOneStep ();
			//Debug.Log ("V: "+pe.Vbest);
		} else if(!end) {
			end = true;
			Debug.Log ("V: "+pe.Vbest);
			pe.Pbest.print();
			Debug.Log ("FINITO");
		}
		if (iteration%1000 == 0) Debug.Log ("V: "+pe.Vbest);
	}

	
	Vector3[] findtag(string tag){
		GameObject[] gos;
		gos = GameObject.FindGameObjectsWithTag(tag);
		//Debug.Log (gos[0].name+" "+gos[1].name);
		//if(tag == "robot") robotsObject = new MoveRobotDifferential[gos.Length];
		Vector3[] tagpositions = new Vector3[gos.Length];
		
		int i = 0;
		foreach (GameObject go in gos) {
			//if(tag == "robot") robotsObject[i] = go.GetComponent<MoveRobotDifferential>();
			tagpositions[i] = go.transform.position;
			i++;
		}
		return tagpositions;
	}
}
