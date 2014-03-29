using UnityEngine;
using System.Collections;

public class Node {
	public Area area;
	public ArrayList connection = new ArrayList();
	public int numconnection;

	public Node (Area input) {
		this.area = input;
	}

	public void addConnection(Node to) {
		connection.Add (to);
	}

	public static bool operator  == (Node n1, Node n2) {
		return (n1.area.center3D.x == n2.area.center3D.x &&
		        n1.area.center3D.z == n2.area.center3D.z);
	}

	public static bool operator  != (Node n1, Node n2) {
		return !(n1.area.center3D.x == n2.area.center3D.x &&
		        n1.area.center3D.z == n2.area.center3D.z);
	}

	//public void countconnections(){
	//	numconnection = connection.Length;
	//}
}
