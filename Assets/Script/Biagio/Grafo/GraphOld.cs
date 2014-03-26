using UnityEngine;
using System.Collections;

public class GraphOld {
	private Node[] nodes;

	public GraphOld(Area[] aree) {
		int size = aree.Length;
		nodes = new Node[size];
		for (int i=0; i<size; i++) {
			nodes[i] = new Node(aree[i]);
		}
	}
}
