using UnityEngine;
using System.Collections;

public class PEalgorithmV2 {
	
	private int[][] A; // visibility matrix

	private Graph graph;
	private ArrayList Q = new ArrayList();
	private int[] v;
	private int[] r; // for each robot, the index of the node (area) in which it is
	private int[][] x; // vector of coordinates (i, j)

	private int R;
	private int N;
	
	private Path[] path;
	
	public int Vbest;
	public Path Pbest;
	
	public PEalgorithmV2(Graph g, int[] robotPosition){
		graph = g;
		
		// Allocazione della memoria per tutte le strutture dati
		A = graph.adjacency;
		r = robotPosition;
		R = robotPosition.Length;
		
		N = graph.numberNodes();
		
		v = new int[N];
		path = new Path[N];
		x = new int[N][];
		for (int i=0; i<N; i++) {
			path[i] = new Path();
			x[i] = new int[2];
		}
	}
	
	public void inizializzation() {
		// Assign first value to all data
		DirtySet d = new DirtySet (N);
		d.calculateDirtySet(r);
		Q.Add (d);

		x [0] [0] = r [0];
		x [0] [1] = r [1];

		path[0].add (r);
		v[0] = d.cardinality();
		
		Vbest = v[0];
		Pbest = path[0];
		
	}
	
	public void PEOneStep() {

	}
	
	public Path PEstep(int MaxIteration) {
		inizializzation ();
		
		// ALGORITHM
		int iteration = 0;
		while (Vbest > 0 && iteration < MaxIteration) {
			iteration++;
			int Qsize = Q.Count-1;
			// for each configuratioQ.Countn of the robot
			for (int d = 0; d < Qsize; d++) {
				DirtySet D = (DirtySet) Q[d];

				for (int i=0; i<2; i++) {
					int[] Xhat = new int[2];
					Xhat[0] = x[d][0];
					Xhat[1] = x[d][1];

					Path Phat = new Path(path[d]);
					Phat.add(x[d][0], x[d][1]);

					int[] U = new int[2];
					U[0] = x[d][0];
					U[1] = x[d][1];

					for (int m = 0; m < N; m++) {
						if (isVisibleFrom(x[d][i], m)) {
							U[i] = m;
							Xhat[i] = m;
							(Phat.getLast())[i] = m;

							int[][] Ahat = modifyAdjacency(A, r);
							DirtySet Dhat = D.multiplication(Ahat);
							int Vhat = Dhat.cardinality();

							int Dindex = find(Dhat, Q);
							if (Dindex == -1) {
								Q.Add(Dhat);
								x[Qsize] = Xhat;
								path[Qsize] = Phat;
								v[Qsize] = Vhat;

								Dindex = Qsize;
							} else if ( dist(Xhat, Dhat.dirtySet) < dist(x[d], Dhat.dirtySet) ) {
								x[Dindex] = Xhat;
								path[Dindex] = Phat;
								v[Dindex] = Vhat;
							}

							if ( Vhat < Vbest) {
								Pbest = Phat;
								Vbest = Vhat;
							}
						}
					}
				}
			}
		}
		
		return Pbest;
	}

	private int dist(int[] x, int[] d) {
		int distance = 0;

		return distance;
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

	// true if there is a connection between two node
	private bool isVisibleFrom(int node1, int node2) {
		return A[node1][node2] == 1;
	}
	
	// Adjacency modified with robot position
	private int[][] modifyAdjacency(int[][] adjacency, int[] robotPos) {
		int[][] modify = new int[N][];
		for (int i=0; i<N; i++) {
			modify[i] = new int[N];
			for (int m=0; m<N; m++) {
				modify[i][m] = adjacency[i][m];
			}
		}
		
		for (int i=0; i<R; i++) {
			int node_index = robotPos[i];
			for (int j=0; j<N; j++) {
				modify[node_index][j] = 0;
			}
		}
		
		return modify;
	}

}
