using UnityEngine;
using System.Collections;

public class GAtester : MonoBehaviour {
	
	private MoveRobot[] robotsObject;
	
	private Vector3[] robots;
	private Vector3[] positions;
	
	private int numrobot;

	private int r = 2;
	private int a = 5;
	private int N = 7;
	private GeneticAlgorithm ge = new GeneticAlgorithm ();

	// Use this for initialization
	void Start () {
		testKillSelection ();
	}

	
	void testKillSelection() {
		float[] fitness = new float[]{130, 201, 170, 50, 90, 700, 500};
		
		
		int[] count = new int[7];
		for (int i=0; i<100; i++) {
			count[ge.killSelection(fitness)]++;
		}
		for (int i=0; i<7; i++) {
			Debug.Log(count[i]);
		}
	}

	void testParentsSelection() {
		float[] fitness = new float[]{130, 201, 170, 50, 90, 700, 500};

		int[,] count = new int[7, 7];
		for (int i=0; i<100; i++) {
			int parent1 = -1, parent2 = -1;
			ge.parentsSelection (fitness, ref parent1, ref parent2);

			count[parent1, parent2]++;
		}
		//Debug.Log ("parent1 "+parent1+" parent2 "+parent2);
		for (int i=0; i<7; i++) {
			for (int j=0; j<7; j++) {
				Debug.Log("["+i+", "+j+"] : "+count[i, j]);
			}
		}
	}

	void testRanking () {
		float[] fitness = new float[]{130, 201, 170, 50};
//		int[] order = ge.fillOrder (fitness);
//		printArray (order, "order: ");

		int[] count = new int[4];
		for (int i=0; i<100; i++) {
			count[ge.rankingSelection(fitness)]++;
		}
		for (int i=0; i<4; i++) {
			Debug.Log(count[i]);
		}
	}

	void testRoulette () {
		float[] fitness = new float[]{130, 170, 210, 50};

		int[] count = new int[4];
		for (int i=0; i<100; i++) {
			count[ge.rouletteWheelSelection(fitness)]++;
		}
		for (int i=0; i<4; i++) {
			Debug.Log(count[i]);
		}

	}

	void testFitness() {
		robots = findtag("robot");
		numrobot = robots.Length;
		
		Area[] aree = ConvexOverlapping.divideSpaceIntoArea ();
		positions = new Vector3[aree.Length];
		for (int i=0; i<aree.Length; i++) {
			positions[i] = aree[i].center3D;
		}
		//Debug.Log (positions.Length);
		FitnessFunction ff = new FitnessFunction (numrobot, robots, positions);

		GeneticAlgorithm ga = new GeneticAlgorithm (numrobot + aree.Length, 10, 1000, ff);

		int[] individual = new int[]{0, 1, 2, 3, 4, 5, 6};
		Debug.Log ( "Fitness: "+ga.calculateFitness (individual));

		individual = new int[]{0, 1, 2, 3, 6, 4, 5};
		Debug.Log ( "Fitness: "+ga.calculateFitness (individual));

		individual = new int[]{0, 2, 3, 6, 1, 4, 5};
		Debug.Log ( "Fitness: "+ga.calculateFitness (individual));

		makeItMove (individual);
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
		for (int i=1; i < (a + r); i++) {
			if(solution[i] < numrobot ) {
				robotsObject[actual_robot].path = path;
				// new robot
				actual_robot = solution[i];
				path = new ArrayList();
			} else {
				//if it is a area
				path.Add(positions[solution[i]-r]);
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

	void testCrossover() {
		
		int[] parent1 = new int[]{0, 2, 3, 1, 4, 5, 6};
		int[] parent2 = new int[]{2, 6, 0, 1, 3, 5, 4};
		printArray (parent1, "before");
		printArray (parent2, "before");
		int[] individual = ge.crossover (parent1, parent2);
		
		printArray (individual, "after");
	}

	void testFirstGeneration() {
		/*
		int[,] population = new int[,]{
			{0, 2, 3, 1, 4, 5, 6},
			{0, 4, 3, 1, 4, 5, 6},
			{0, 2, 3, 1, 4, 5, 6},
			{0, 2, 3, 1, 4, 5, 6}
			};*/

		int n_pop = 4;
		int[][] population = new int[4][];
		for (int i=0; i<n_pop; i++) {
			population[i] = new int[N];
		}
	}

	private void printArray(int[] array, string label) {
		string log = label+" [";
		for (int i=0; i<array.Length; i++) {
			log += array[i]+ ", ";
		}
		Debug.Log (log+" ]");
	}

	// Update is called once per frame
	void Update () {
	
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
