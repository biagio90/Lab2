using UnityEngine;
using System.Collections;

public class Node {
	public Area area;
	public ArrayList connection = new ArrayList();
	public int numconnection = 0;

	public Node (Area input) {
		this.area = input;
	}

	public void addConnection(Node to) {
		connection.Add (to);
	}
}
