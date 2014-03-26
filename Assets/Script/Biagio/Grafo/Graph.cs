﻿using UnityEngine;
using System.Collections;

public class Graph {
	private Node[] nodes;
	public int[][] adjacency;

	public Graph(Area[] aree) {
		int size = aree.Length;
		nodes = new Node[size];
		adjacency = new int[size][];
		for (int i=0; i<size; i++) {
			nodes[i] = new Node(aree[i]);
			adjacency[i] = new int[size];
		}

		constructgraph ();
	}

	public int findInGraph (Vector3 pos) {
		for (int i=0; i<nodes.Length; i++) {
			Vector3 pos2 = nodes[i].area.center3D;
			Vector3 dir = (pos2 - pos).normalized;

			if(freePath(pos2, pos+dir*3)) {
				return i;
			}
		}
		return -1;
	}


	private void constructgraph(){ //also fill the djacency matrix
		int size = nodes.Length;
		int numconnections = 0;
		for (int i=0; i<size; i++) { //for every node 
			Vector3 nodeipos = nodes[i].area.center3D;
			for (int j=0; j<size; j++) {
				Vector3 nodejpos = nodes[j].area.center3D;
				
				adjacency[i][j] = 0;

				//check all the other nodes in the graph
				if ( freePath(nodeipos,nodejpos) ){
					nodes[i].addConnection(nodes[j]);
					adjacency[i][j] = 1; //if it's visible there is a 1 and a connection
					numconnections++;
					//(new MyLine()).drawLine(nodeipos, nodejpos, Color.red);
				}
			}
			nodes[i].numconnection = numconnections; //store in each node how many other nodes can he see 
			numconnections = 0;
		}
	}


	 public bool freePath (Vector3 p1, Vector3 p2) {
		Vector3 direction = (p2 - p1).normalized;
		float sight = (p2 - p1).magnitude;
		//RaycastHit hit;
		//return Rigidbody.SweepTest (direction, out hit, sight);
		
		Ray ray = new Ray(p1, direction);
		RaycastHit hit = new RaycastHit ();
		bool collision = true;
		
		if (Physics.Raycast (ray, out hit, sight)) {
			collision = false;
			//Debug.Log("Collision: "+hit.collider.name);
		}
		return collision;
	}

	public int numberNodes() {
		return nodes.Length;
	}

	public void printNodes() {
		for (int i=0; i<nodes.Length; i++) {
			Debug.Log(i+": "+nodes[i].area.center3D);
		}
	}

	public void drawConnections() {
		MyLine draw = new MyLine ();

		for (int i=0; i<nodes.Length-1; i++) {
			for (int j=i; j<nodes.Length; j++) {
				if (adjacency[i][j] == 1) {
					draw.drawLine(nodes[i].area.center3D, nodes[j].area.center3D, Color.red);
				}
			}
		}
	}
}