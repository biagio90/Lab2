using UnityEngine;
using System.Collections;

public class VRP_GA : MonoBehaviour {

	private int maxIteration = 100;

	private MoveRobot[] robotsObject;

	private Vector3[] robots;
	private Vector3[] positions;

	private int numrobot;
	private int numaree;

	private int iteration = 0;
	private bool end = false;

	private FitnessFunction ff;
	private GeneticAlgorithm ga;

	// Use this for initialization
	void Start () {
		robots = findtag("robot");
		numrobot = robots.Length;

		Area[] aree = ConvexOverlapping.divideSpaceIntoArea ();
		positions = new Vector3[aree.Length];
		numaree = aree.Length;
		for (int i=0; i<aree.Length; i++) {
			positions[i] = aree[i].center3D;
		}

		ff = new FitnessFunction (numrobot, robots, positions);
		ga = new GeneticAlgorithm (numrobot + aree.Length, 10, maxIteration, ff);
		/*
		int[] solution = ga.run ();

		string log = "GA: [";
		for (int i=0; i<solution.Length; i++) {
			log += solution[i] + ", ";
		}
		log = "]";
		Debug.Log (log);
		*/
		ga.first_generation (ga._population);

	}
	
	// Update is called once per frame
	void Update () {
		if(iteration < maxIteration) {
			ga.oneStepRun ();
			//Debug.Log ("iteration "+iteration);
			iteration++;
		} else if(!end) {
			end = true;
			Debug.Log ("FINITO");

			string log = "GA: [";
			for (int i=0; i<ga._bestSolution.Length; i++) {
				log += ga._bestSolution[i] + ", ";
			}
			log += "]";
			Debug.Log (log);
			Debug.Log ("fitness: "+ga._bestFitness);

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


}
