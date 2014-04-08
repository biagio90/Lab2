using UnityEngine;
using System.Collections;

public class CoopSearch : MonoBehaviour {

	private bool loading = true;

	void OnGUI() {
		if (loading) GUI.Label(new Rect (400, 300, 100, 30), "Calculating...");
	}

	private Graph graph;
	private Area[] aree;

	
	public int maxIteration = 50;
	
	private MoveRobot[] robotsObject;
	
	private Vector3[] robots;
	private Vector3[] positions;
	
	private int numrobot;
	private int numaree;
	
	private int iteration = 0;
	private bool end = false;
	
	private FitnessFunction ff;
	private GeneticAlgorithm ga;
	
	private float lastSolution = 0.0f;
	private int countLastSolution = 0;

	// Use this for initialization
	void Start () {
		aree = ConvexOverlapping.divideSpaceIntoArea();
		graph = new Graph(aree) ; //secondo me aree dovrà essere statico utilizzabile da tutti gli algoritmi
//		graph.constructgraph ();

		Node[] nodes = cooperativesearch_paths (graph);
		for (int i=0; i<nodes.Length; i++) {
			Debug.Log(nodes[i].area.center3D);
		}

		robots = findtag("robot");
		numrobot = robots.Length;
		Debug.Log ("robots: " + numrobot);

		positions = new Vector3[nodes.Length];
		numaree = nodes.Length;
		for (int i=0; i<numaree; i++) {
			positions[i] = nodes[i].area.center3D;
		}

		Debug.Log ("num robot: "+numrobot);
		Debug.Log ("num aree: " +numaree);
		ff = new FitnessFunction (numrobot, robots, positions);
		ga = new GeneticAlgorithm (numrobot + numaree, 10, maxIteration, ff);
		ga.first_generation (ga._population);

		/*
		graph.printNodes ();
		Node n = graph.nodes [1];
		ArrayList list = n.connection;
		foreach (Node nod in list) {//è pseudo codice perchè non ho capito come prendere il nodo dall'arraylist
			Debug.Log (nod.area.center3D);
		}

		graph.removenode (graph.nodes[1]);
		graph.printNodes ();

		list = n.connection;
		foreach (Node nod in list) {//è pseudo codice perchè non ho capito come prendere il nodo dall'arraylist
			Debug.Log (nod.area.center3D);
		}
		*/
	}
	
	// Update is called once per frame
	void Update () {
		
		if(iteration < maxIteration ){//&& countLastSolution < 20) {
			ga.oneStepRun ();
			//Debug.Log ("iteration "+iteration);
			iteration++;
			
			if (lastSolution == ga._bestFitness) {
				countLastSolution++;
			} else {
				lastSolution = ga._bestFitness;
				countLastSolution = 1;
			}
		} else if(!end) {
			loading = false;

			end = true;
			Debug.Log ("FINITO");
			
			string log = "GA: [";
			for (int i=0; i<ga._bestSolution.Length; i++) {
				log += ga._bestSolution[i] + ", ";
			}
			log += "]";
			Debug.Log (log);
			Debug.Log ("fitness: "+ga._bestFitness);
			//Debug.Log ("countLastSolution: "+countLastSolution);
			
			makeItMove(ga._bestSolution);
		}
	}
	
	
	private void makeItMove(int[] input) {
		
		//now maybe we have to draw the lines, or simply make them move in update
		int[] solution = rotateToRobot (input);
		/*
		string log = "";
		for (int i = 0; i<vrp_bestfit.Length;i++) {
			log += vrp_bestfit[i]+" ";
			createPoint((Vector3) everyone[vrp_bestfit[i]]);
		}
		Debug.Log(log);*/
		
		
		int actual_robot = solution[0];
		ArrayList path = new ArrayList();
		for (int i=1; i < (numaree + numrobot); i++) {
			if(solution[i] < numrobot ) {
				robotsObject[actual_robot].path = path;
				// new robot
				actual_robot = solution[i];
				path = new ArrayList();
			} else {
				//if it is a area
				path.Add(positions[solution[i]-numrobot]);
			}
		}
		if (actual_robot != -1) {
			robotsObject[actual_robot].path = path;
		}
		
		foreach (MoveRobot rob in robotsObject) {
			rob.go = true;
		}
	}
	
	
	private int[] rotateToRobot (int[] vrp_array) {
		bool find = false;
		int index = -1;
		for (int i=0; i < vrp_array.Length && !find; i++) {
			if(vrp_array[i] < numrobot ) {
				find = true;
				index = i;
			}
		}
		
		int[] aux = new int[vrp_array.Length];
		for (int i=0; i < vrp_array.Length; i++) {
			aux[i] = vrp_array[index];
			index = (index+1)%vrp_array.Length;
		}
		/*
		for (int i=0; i < vrp_array.Length; i++) {
			vrp_array[i] = aux[i];
		}*/
		return aux;
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
}
