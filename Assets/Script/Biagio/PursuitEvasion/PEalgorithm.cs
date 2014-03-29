using UnityEngine;
using System.Collections;

public class PEalgorithm {

	private int[][] A; // visibility matrix

	private Graph graph;
	private DirtySet[][] d;
	private int[][] v;
	private int[] r; // for each robot, the index of the node (area) in which it is

	private int R;
	private int N;

	private Path[][] path;

	public int Vbest;
	public Path Pbest;

	public PEalgorithm(Graph g, int[] robotPosition){
		graph = g;

		// Allocazione della memoria per tutte le strutture dati
		A = graph.adjacency;
		r = robotPosition;
		R = robotPosition.Length;

		N = graph.getnumnodes();

		d = new DirtySet[N][];
		for (int i=0; i<N; i++) {
			d[i] = new DirtySet[N];
			for (int j=0; j<N; j++) {
				d[i][j] = new DirtySet(N);
			}
		}

		v = new int[N][];
		path = new Path[N][];
		for (int i=0; i<N; i++) {
			path[i] = new Path[N];
			for (int j=0; j<N; j++) {
				path[i][j] = new Path();
			}
			v[i] = new int[N];
		}
	}

	public void inizializzation() {
		// Assign first value to all data
		//Debug.Log ("N: "+N);
		for (int n=0; n<N; n++) {
			for (int m=0; m<N; m++) {
				v[n][m] = N;
				path[n][m] = new Path();
				d[n][m].calculateDirtySet(r);
			}
		}

		path[r[0]][r[1]].add (r);
		d[r[0]][r[1]].updateVisibility(r, A);
		d[r[0]][r[1]].print();
		v[r[0]][r[1]] = d[r[0]][r[1]].cardinality();

		Vbest = v[r[0]][r[1]];
		Pbest = path[r[0]][r[1]];

	}

	public void PEOneStep() {
		// for each configuration of the robot
		for (int n1 = 0; n1 < N; n1++) {
			for(int n2=0; n2 < N; n2++) {
				
				// for each future configuration of the robot
				for (int m1 = 0; m1 < N; m1++) {
					if (isVisibleFrom(n1, m1)) {
						for(int m2=0; m2 < N; m2++) {
							if (isVisibleFrom(n2, m2)) {
								
								// Calculate next step and, if it is a better value, update the matrix d, v and path
								int[][] Ahat = modifyAdjacency(A, r);
								DirtySet Dhat = d[n1][n2].multiplication(Ahat);
								int Vhat = Dhat.cardinality();
								
								if ( Vhat < v[m1][m2] ) {
									path[m1][m2].add(m1, m2);
									d[m1][m2] = Dhat;
									v[m1][m2] = Vhat;
								}
								
								// check for the best path so far
								if (v[m1][m2] < Vbest) {
									Vbest = v[m1][m2];
									Pbest = path[m1][m2];
								}
								
							}
						}
					}
				}
				
			}
		}
	}

	public Path PEstep(int MaxIteration) {
		inizializzation ();

		// ALGORITHM
		int iteration = 0;
		while (Vbest > 0 && iteration < MaxIteration) {
			iteration++;

			// for each configuration of the robot
			for (int n1 = 0; n1 < N; n1++) {
				for(int n2=0; n2 < N; n2++) {

					// for each future configuration of the robot
					for (int m1 = 0; m1 < N; m1++) {
						if (isVisibleFrom(n1, m1)) {
							for(int m2=0; m2 < N; m2++) {
								if (isVisibleFrom(n2, m2)) {

									// Calculate next step and, if it is a better value, update the matrix d, v and path
									int[][] Ahat = modifyAdjacency(A, r);
									DirtySet Dhat = d[n1][n2].multiplication(Ahat);
									int Vhat = Dhat.cardinality();

									if ( Vhat < v[m1][m2] ) {
										path[m1][m2].add(m1, m2);
										d[m1][m2] = Dhat;
										v[m1][m2] = Vhat;
									}

									// check for the best path so far
									if (v[m1][m2] < Vbest) {
										Vbest = v[m1][m2];
										Pbest = path[m1][m2];
									}

								}
							}
						}
					}

				}
			}
		}

		return Pbest;
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
