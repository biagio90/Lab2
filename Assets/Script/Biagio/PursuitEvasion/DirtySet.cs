using UnityEngine;
using System.Collections;

public class DirtySet {

	public int[] dirtySet;
	private int N;

	public DirtySet(int N) {
		dirtySet = new int[N];
		this.N = N;
	}

	public DirtySet(int[] input) {
		N = input.Length;
		dirtySet = new int[N];
		for (int i=0; i<N; i++) {
			dirtySet[i] = input[i];
		}
	}

	public bool equal(DirtySet cmp) {
		bool e = true;
		for (int n=0; n<N && e; n++) {
			if (dirtySet[n] != cmp.dirtySet[n])
				e=false;
		}

		return e;
	}

	public void calculateDirtySet(int[] robPos) {
		for (int n=0; n<N; n++) {
			dirtySet[n] = 1;
		}
		for (int n=0; n<robPos.Length; n++) {
			dirtySet[robPos[n]] = 0;
		}
	}

	//DEBUGED
	public void updateVisibility(int[] robPos, int[][] A) {
		int R = robPos.Length;

		for (int n=0; n<N; n++) {
			if (dirtySet[n] == 1)
			for (int r=0; r<R; r++) {
				int node = robPos[r];
				if(A[node][n] == 1){
					dirtySet[n] = 0;
				}
			}
		}
	}


	public int cardinality () {
		int count = 0;
		for (int n=0; n<N; n++) {
			count += dirtySet [n];
		}
		return count;
	}

	//DEBUGED
	// MIN-MAX multiplication
	public DirtySet multiplication(int[][] B) {
		int[] A = dirtySet;
		int[] D = new int[N];
		
		for (int i=0; i<N; i++) {
			int[] min = new int[N];
			for (int m=0; m<N; m++) {
				if (A[m] < B[m][i]) { min[m] = A[m]; } else {min[m] = B[m][i];}
			}
			int max = -1;
			for (int m=0; m<N; m++) {
				if(min[m] > max) max = min[m];
			}
			
			D[i] = max;
		}

		DirtySet ret = new DirtySet(D);

		return ret;
	}

	//DEBUGED
	// MIN-MAX multiplication
	public DirtySet multiplication(int[,] B) {
		int[] A = dirtySet;
		int[] D = new int[N];
		
		for (int i=0; i<N; i++) {
			int[] min = new int[N];
			for (int m=0; m<N; m++) {
				if (A[m] < B[m,i]) { min[m] = A[m]; } else {min[m] = B[m,i];}
			}
			int max = -1;
			for (int m=0; m<N; m++) {
				if(min[m] > max) max = min[m];
			}
			
			D[i] = max;
		}
		
		DirtySet ret = new DirtySet(D);
		
		return ret;
	}

	public void print() {
		string log = "[";
		for (int i=0; i<N; i++) {
			log += (dirtySet[i]+ ",");
		}
		Debug.Log (log+"]");
	}
}
