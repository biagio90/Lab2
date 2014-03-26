using UnityEngine;
using System.Collections;

public class Path {
	public ArrayList path = new ArrayList();

	public Path() {
	}

	public Path(Path p) {
		for (int i=0; i<p.path.Count; i++) {
			path.Add(p.path[i]);
		}
	}

	public void add(int[] r) {
		int[] cell = new int[2];
		cell [0] = r [0];
		cell [1] = r [1];
		
		path.Add (cell);
	}

	public void add(int r1, int r2) {
		int[] cell = new int[2];
		cell [0] = r1;
		cell [1] = r2;
		
		path.Add (cell);
	}

	public int[] getLast() {
		return (int[]) path[path.Count - 1];
	}

	public void print() {
		for (int i=0; i<path.Count; i++) {
			Debug.Log ("("+((int[])path[i])[0]+", "+((int[])path[i])[0]+") ; ");
		}
	}
}
