using UnityEngine;
using System.Collections;

public class PEtest : MonoBehaviour {

	private int maxIteration = 10000;

	private MoveRobot[] robotsObject;

	private Graph graph;
	private Vector3[] robots;
	private PEalgorithmV2 pe;

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
			//Debug.Log ("robot "+r[i]);
		}


		pe = new PEalgorithmV2 (graph, r);
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

			SendPathToRobot(pe.Pbest);
		}
		if (iteration%1000 == 0) {
			Debug.Log ("V: "+pe.Vbest);
			//pe.Pbest.print();

		}
	}

	private void SendPathToRobot(Path path) {
		ArrayList r1_path = new ArrayList();
		ArrayList r2_path = new ArrayList();

		foreach(int[] pos in path.path) {
			r1_path.Add( nodeToPos( pos[0] ) );
			r2_path.Add( nodeToPos( pos[1] ) );
		}
		robotsObject [0].path = r1_path;
		robotsObject [1].path = r1_path;
		robotsObject [0].go = true;
		robotsObject [1].go = true;

		/*
		foreach (MoveRobotDifferential rob in robotsObject) {
			rob.path = path;
		}

		foreach (MoveRobotDifferential rob in robotsObject) {
			rob.go = true;
		}
		*/
	}

	private Vector3 nodeToPos (int node) {
		return graph.nodes [node].area.center3D;
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
}
