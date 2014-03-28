using UnityEngine;
using System.Collections;

public class multiplicationTest : MonoBehaviour {

	private int N = 5;
	private int R = 2;

	private int[,] A = {
		{1, 0, 0, 1, 0},
		{0, 1, 1, 0, 0},
		{0, 1, 1, 1, 1},
		{1, 0, 1, 1, 1},
		{0, 0, 1, 1, 1} 
	};

	// Use this for initialization
	void Start () {
		//testMultiplication ();
		//findDirtySetTest ();
		distTest ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	Graph graph;
	private void distTest()  {
		Area[] aree = ConvexOverlapping.divideSpaceIntoArea ();
		graph = new Graph (aree);

//		Debug.Log ("Shortest path dist nodes 3 to 0: "+graph.dist (3, 0));
//		Debug.Log ("Shortest path dist nodes 3 to 1: "+graph.dist (3, 1));
//		Debug.Log ("Shortest path dist nodes 3 to 2: "+graph.dist (3, 2));

		int[] robots = new int[] {0, 0};
		int[] dirtySet = new int[] {1, 0, 0, 1};
		Debug.Log ("Distance test: robot [0, 0]: "+dist(robots, dirtySet));
	}

	private int dist(int[] x, int[] d) {
		int distance = 0;
		for (int i=0; i<d.Length; i++) {
			if(d[i] == 1) {
				for (int j=0; j<x.Length; j++) {
					distance += graph.dist (x[j], i);
				}
			}
		}
		return distance;
	}

	private void findDirtySetTest () {
		DirtySet d1 = new DirtySet (N);
		DirtySet d2 = new DirtySet (N);
		DirtySet d3 = new DirtySet (N);
		DirtySet d4 = new DirtySet (N);

		d1.dirtySet = new int[] {1, 0, 0, 0, 0};
		d2.dirtySet = new int[] {0, 0, 1, 0, 0};
		d3.dirtySet = new int[] {0, 0, 0, 0, 1};
		d4.dirtySet = new int[] {0, 0, 0, 0, 0};

		ArrayList Q = new ArrayList();
		Q.Add (d1);
		Q.Add (d2);
		Q.Add (d3);

		Debug.Log (find (d4, Q));
	}

	private void testMultiplication() {
		DirtySet d = new DirtySet (N);
		d.dirtySet = new int[] {1, 0, 0, 0, 1};
		int[] robots = new int[] {4, 1};
		
		int[][] Ahat = modifyAdjacency (A, robots);
		/*
		for (int i=0; i<N; i++) {
			string log = "";
			for (int j=0; j<N; j++) {
				log += Ahat[i][j] + " ";
			}
			Debug.Log(log);
		}*/
		DirtySet Dhat = d.multiplication (Ahat);
		Dhat.print ();
	}

	private int find(DirtySet d, ArrayList list) {
		int index = -1;
		for(int i=0; i<list.Count && index == -1; i++) {
			if (d.equal((DirtySet) list[i])) {
				index = i;
			}
		}
		
		return index;
	}

	// Adjacency modified with robot position
	private int[][] modifyAdjacency(int[,] adjacency, int[] robotPos) {
		int[][] modify = new int[N][];
		for (int i=0; i<N; i++) {
			modify[i] = new int[N];
			for (int m=0; m<N; m++) {
				modify[i][m] = adjacency[i,m];
			}
		}
		
		for (int i=0; i<R; i++) {
			int node_index = robotPos[i]-1;
			for (int j=0; j<N; j++) {
				modify[node_index][j] = 0;
				modify[j][node_index] = 0;

			}
		}
		
		return modify;
	}

}
