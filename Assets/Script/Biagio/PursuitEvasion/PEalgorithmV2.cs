using UnityEngine;
using System.Collections;

public class PEalgorithmV2 {
	/**
	 * Dimension:
	 * A[node][node]
	 * v[node]
	 * r[robot]
	 * x[node][robot]
	 * path[node]
	 * 
	 */
	private bool debugOn = false;

	private int[][] A; // visibility matrix

	private Graph graph;
	private ArrayList Q = new ArrayList();
	private int[] v;
	private int[] r;   // for each robot, the index of the node (area) in which it is
	private int[][] x; // vector of coordinates (i, j)

	private int R;
	private int N;
	private int Np2;

	private Path[] path;
	
	public int Vbest;
	public Path Pbest;
	
	public PEalgorithmV2(Graph g, int[] robotPosition){
		graph = g;
		
		// Allocazione della memoria per tutte le strutture dati
		A = graph.adjacency;
		r = robotPosition;
		R = robotPosition.Length;
		
		N = graph.getnumnodes();
		Np2 = Mathf.FloorToInt(Mathf.Pow (2, N));

		v = new int[Np2];
		path = new Path[Np2];
		x = new int[Np2][];
		for (int i=0; i<Np2; i++) {
			path[i] = new Path();
			path[i].add (r);
			x[i] = new int[2];
		}
	}
	
	public void inizializzation() {
		// Assign first value to all data
		DirtySet d = new DirtySet (N);
		d.calculateDirtySet(r);
		d.updateVisibility (r, A);
		//d.print ();
		Q.Add (d);

		//for (int i = 0; i < Np2; i++) {
		x [0] [0] = r [0];
		x [0] [1] = r [1];

		//path[0].add (r);
		v[0] = d.cardinality();

		Vbest = v[0];
		Pbest = path[0];
		
	}
	
	public void PEOneStep() {
		int Qsize = Q.Count;
		//Debug.Log ("Q size "+Qsize);
		// for each configuratioQ.Countn of the robot
		for (int d = 0; d < Qsize; d++) {
			DirtySet D = (DirtySet) Q[d];
			
			// try to move one robot at a time
			for (int i=0; i<2; i++) {
				int[] Xhat = new int[2] {x[d][0], x[d][1]};
				if(debugOn) Debug.Log ("X hat ("+Xhat[0]+", "+Xhat[1]+")");

				// find the next node for the robot i
				for (int m = 0; m < N; m++) {
					if (x[d][i] != m && isVisibleFrom(x[d][i], m)) {
						Xhat[i] = m;
						
						int[][] Ahat = modifyAdjacency(A, Xhat);

						//Debug.Log("new robots: "+Xhat[0]+" "+Xhat[1]);
						/***************/
						/*
						string log = "";
						for (int k=0; k<N; k++) {
							for (int j=0; j<N; j++) {
								log += Ahat[k][j] + " ";
							}
							log += "\n";
						}
						Debug.Log(log);*/
						/*******/
						DirtySet Dhat = D.multiplication(Ahat);
						if(debugOn) Debug.Log("M: "+m+" Dhat: ");
						if(debugOn) Dhat.print();
						
						int Vhat = Dhat.cardinality();
						
						int Dindex = find(Dhat, Q);
						if (Dindex == -1) {
							// new dirty set
							x[Qsize] = Xhat;

							path[Qsize] = new Path(path[d]);
							path[Qsize].add(Xhat);

							//Debug.Log("Q: "+Qsize);
							//path[Qsize].print();
							v[Qsize] = Vhat;

							Q.Add(Dhat);
							Dindex = Qsize;

							
							if ( Vhat == Vbest && path[Dindex].path.Count < Pbest.path.Count) {
								Pbest = new Path(path[Dindex]);
								Vbest = Vhat;
							} else if ( Vhat < Vbest) {
								Pbest = new Path(path[Dindex]);
								Vbest = Vhat;
							}
						} else if ( Vhat < v[Dindex] || ( Vhat == v[Dindex] &&
								dist(Xhat, Dhat.dirtySet) < dist(x[d], Dhat.dirtySet)) ) {
							// update old dirty set with new info

							//Debug.Log(dist(Xhat, Dhat.dirtySet));
							x[Dindex] = Xhat;
							//Debug.Log("Before: ");
							//path[Qsize].print();
							path[Dindex].add(Xhat);
							//Debug.Log("Q: "+Dindex);
							//path[Qsize].print();
							v[Dindex] = Vhat;

							if ( Vhat == Vbest && path[Dindex].path.Count < Pbest.path.Count) {
								Pbest = new Path(path[Dindex]);
								Vbest = Vhat;
							} else if ( Vhat < Vbest) {
								Pbest = new Path(path[Dindex]);
								Vbest = Vhat;
							}
						}
						/*
						if ( Vhat == Vbest) {
								if (path[Dindex].path.Count < Pbest.path.Count) {
									Pbest = new Path(path[Dindex]);
									Vbest = Vhat;
								}
							} else if ( Vhat < Vbest) {
								Pbest = new Path(path[Dindex]);
								Vbest = Vhat;
							}*/
					}
				}
			}
		}
	}
	/*
	public Path PEstep(int MaxIteration) {
		inizializzation ();
		
		// ALGORITHM
		int iteration = 0;
		while (Vbest > 0 && iteration < MaxIteration) {
			iteration++;
			int Qsize = Q.Count;
			// for each dirty set find so far
			for (int d = 0; d < Qsize; d++) {
				DirtySet D = (DirtySet) Q[d];

				// try to move one robot at a time
				for (int i=0; i<2; i++) {
					int[] Xhat = new int[2] {x[d][0], x[d][1]};
					if(debugOn) Debug.Log ("X hat ("+Xhat[0]+", "+Xhat[1]+")");

					int[] U = new int[2] {x[d][0], x[d][1]};

					Path Phat = new Path(path[d]);
					Phat.add(x[d][0], x[d][1]);
					if(debugOn) Debug.Log ("P hat ("+(Phat.getLast())[0]+", "+(Phat.getLast())[1]+")");

					// find the next node for the robot i
					for (int m = 0; m < N; m++) {
						if (x[d][i] != m && isVisibleFrom(x[d][i], m)) {
							U[i] = m;
							Xhat[i] = m;
							(Phat.getLast())[i] = m;

							int[][] Ahat = modifyAdjacency(A, Xhat);
							string log = "";
							for (int k=0; k<N; k++) {
								for (int j=0; j<N; j++) {
									log += A[k][j] + " ";
								}
								log += "\n";
							}
							Debug.Log(log);
							Debug.Log("robots: "+Xhat[0]+" "+Xhat[1]);
							 log = "";
							for (int k=0; k<N; k++) {
								for (int j=0; j<N; j++) {
									log += Ahat[k][j] + " ";
								}
								log += "\n";
							}
							Debug.Log(log);
							DirtySet Dhat = D.multiplication(Ahat);
							if(debugOn) Debug.Log("M: "+m+" Dhat: ");
							if(debugOn) Dhat.print();

							int Vhat = Dhat.cardinality();

							int Dindex = find(Dhat, Q);
							if (Dindex == -1) {
								Q.Add(Dhat);
								x[Qsize] = Xhat;
								path[Qsize] = Phat;
								v[Qsize] = Vhat;

								Dindex = Qsize;
							} else if ( dist(Xhat, Dhat.dirtySet) < dist(x[d], Dhat.dirtySet) ) {
								Debug.Log(dist(Xhat, Dhat.dirtySet));
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
*/
	// DEBUGGED
	private int dist(int[] x, int[] d) {
		int distance = 0;
		for (int i=0; i<d.Length; i++) {
			if(d[i] == 1) {
				for (int j=0; j<x.Length; j++) {
					distance += graph.dist (x[j], i);
					//distance += 1;
				}
			}
		}
		return distance;
	}

	//DEBUGED
	private int find(DirtySet d, ArrayList list) {
		for(int i=0; i<list.Count; i++) {
			if (d.equal((DirtySet) list[i])) {
				return i;
			}
		}

		return -1;
	}

	// true if there is a connection between two node
	private bool isVisibleFrom(int node1, int node2) {
		return A[node1][node2] == 1;
	}

	//DEBUGED
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
				modify[j][node_index] = 0;
			}
		}
		
		return modify;
	}

}
