using UnityEngine;
using System;
using System.Collections;

public class GA : MonoBehaviour {

	static System.Random _random = new System.Random();

	public int iteration;

	private MoveRobotDifferential[] robotsObject;

	private Vector3[] robots;
	private Vector3[] positions;
	private Vector3[] everyone;

	private int numrobot;
	private int numpos;

	private int[] vrpstart;
	//private int[] vrpsolution;

	private MyLine draw = new MyLine();


	// Use this for initialization
	void Start () {
		robots = findtag("robot");
		positions = findtag("area");
		vrpstart = fill(robots,positions);
		
		numrobot = robots.Length;
		numpos = positions.Length;

		everyone = new Vector3[numpos + numrobot];
		int j = 0;
		for (int i=0; i < numrobot; i++) {
			everyone[j]= robots[i];
			j++;
		}
		for (int i=0; i < numpos; i++) {
			everyone[j]= positions[i];
			j++;
		}

		int[] vrp_actual = Shuffleint(vrpstart); 
		int[] vrp_bestfit = new int[vrp_actual.Length] ;
		for (int i=0; i < vrp_actual.Length; i++) {
			vrp_bestfit[i] = vrp_actual[i];
		}

		int actualfit = fit(vrp_actual);
		int bestfit = actualfit;

		for (int i = 0; i < iteration; i++) {
			vrp_actual = Shuffleint(vrp_actual);
			actualfit = fit(vrp_actual);
				if(actualfit < bestfit){ 
					bestfit = actualfit;
					//vrp_bestfit = vrp_actual;
					for (int z=0; z < vrp_actual.Length; z++) {
						vrp_bestfit[z] = vrp_actual[z];
					}
				}
		}

		//now maybe we have to draw the lines, or simply make them move in update
		vrp_bestfit = rotateToRobot (vrp_bestfit);

		string log = "";
		for (int i = 0; i<vrp_bestfit.Length;i++) {
			log += vrp_bestfit[i]+" ";
			createPoint((Vector3) everyone[vrp_bestfit[i]]);
		}
		Debug.Log(log);


		int actual_robot = -1;
		ArrayList path = new ArrayList();
		for (int i=0; i < (numpos + numrobot); i++) {
			if(vrp_bestfit[i] < numrobot ) {
				//if it is a robot
				if (actual_robot != -1) {
					robotsObject[actual_robot].path = path;
				}
				actual_robot = vrp_bestfit[i];
				path = new ArrayList();
			} else {
				//if it is a area
				path.Add(everyone[vrp_bestfit[i]]);
			}
		}
		if (actual_robot != -1) {
			robotsObject[actual_robot].path = path;
		}

		foreach (MoveRobotDifferential rob in robotsObject) {
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

	// Update is called once per frame
	void Update () {
	
	
	}

	Vector3[] findtag(string tag){
		GameObject[] gos;
		gos = GameObject.FindGameObjectsWithTag(tag);
		//Debug.Log (gos[0].name+" "+gos[1].name);
		if(tag == "robot") robotsObject = new MoveRobotDifferential[gos.Length];
		Vector3[] tagpositions = new Vector3[gos.Length];

		int i = 0;
		foreach (GameObject go in gos) {
			if(tag == "robot") robotsObject[i] = go.GetComponent<MoveRobotDifferential>();
			tagpositions[i] = go.transform.position;
			i++;
			}
		return tagpositions;
	}

	int[] fill(Vector3[] rob, Vector3[] pos){
		int i = 0;
		int[] solution = new int[pos.Length + rob.Length];
		for (i=0; i < rob.Length; i++) {
			solution[i]= i;
		}
	
		for (i = rob.Length; i < (pos.Length + rob.Length); i++) {
			solution[i]= i;
		}
		return solution;
	}

	public static int[] Shuffleint(int[] array)
	{
		var random = _random;
		for (int i = array.Length; i > 1; i--)
		{
			// Pick random element to swap.
			int j = random.Next(i); // 0 <= j <= i-1
			// Swap.
			int tmp = array[j];
			array[j] = array[i - 1];
			array[i - 1] = tmp;
		}

		return array;
	}

	private float distance (Vector3 p1, Vector3 p2) {
		return Vector3.Distance (p1, p2);
	}

	private int fit(int[] vrp){
		int actual, next, fitn = 0;
		float update;

		// * Construct the de-mapping
		for (int i = 0; i < vrp.Length; i++) {
			actual = vrp[i];
			next = vrp[(i+1)%vrp.Length];
			//if(i + 1 >= vrp.Length) next = vrp[0];
			//* calculate the sum of the distances that each robot has to do
			if(next > numrobot){
				//Debug.Log ("Actual "+actual+" next "+next);
				update = distance(everyone[actual],everyone[next]);
				// * store the value in a variable and return. //
				fitn = fitn + Mathf.RoundToInt(update);
			}
		}

		return fitn;
	
	}

	
	private void createPoint (Vector3 point) {
		Vector3 n = new Vector3 (point.x + 2f, point.y, point.z + 2f);
		draw.drawLine (point, n, Color.green);
	}
}
