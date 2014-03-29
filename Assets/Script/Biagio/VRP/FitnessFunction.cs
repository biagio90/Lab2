using UnityEngine;
using System.Collections;

public class FitnessFunction {

	private AstarCreator Astar = new AstarCreator(600, 10);

	private int robotNumber;
	private Vector3[] robots;
	private Vector3[] positions;

	public FitnessFunction (int n_rob, Vector3[] robots, Vector3[] positions){
		robotNumber = n_rob;
		this.robots = robots;
		this.positions = positions;
	}

	public float calculate(int[] input) {
		float distance = 0.0f;
		int size = input.Length;

		//int robotNumber = getRobotNumber();

		int firstRobot = -1;
		for (int i=0; i<size && firstRobot == -1; i++) {
			if(input[i] < robotNumber) {
				firstRobot = i;
			}
		}

		bool firstTime = true;
		for (int i=firstRobot; i != firstRobot || firstTime ; i = (i+1)%size) {
			firstTime = false;
			if(input[(i+1)%size] >= robotNumber) {
				distance += AstarDistance(input[i], input[(i+1)%size]);
			}
		}

		return 1000-distance;
	}

	private float AstarDistance(int pos1, int pos2) {
		Vector3 from, to;
		//Debug.Log ("distance from "+pos1+" to "+pos2);
		if (pos1 < robotNumber) from = robots[pos1];
		else   from = positions[pos1-robotNumber];

		if (pos2 < robotNumber) to = robots[pos2];
		else   to = positions[pos2-robotNumber];

		ArrayList path = Astar.getPath (from, to);

		float distance = 0.0f;
		for (int i=0; i<path.Count-1; i++) {
			distance += Vector3.Distance((Vector3)path[i], (Vector3)path[i+1]);
		}

		return distance;
	}

	private int getRobotNumber() {
		GameObject[] gos = GameObject.FindGameObjectsWithTag("robot");
		return gos.Length;
	}

}
