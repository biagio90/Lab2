using UnityEngine;
using System.Collections;

public class ConvexOverlapping : MonoBehaviour {

	private MyLine draw = new MyLine ();
	private float mainY = 1f;

	// Use this for initialization
	void Start () {
		GameObject[] walls = GameObject.FindGameObjectsWithTag ("wall");

		for (int i=0; i<walls.Length; i++) {
			Vector3[] corners = getCorners(walls[i]);

			printLine(corners[0], corners[1]);
			printLine(corners[1], corners[2]);
			printLine(corners[2], corners[3]);
			printLine(corners[3], corners[0]);

			printLine(corners[1], corners[0]);
			printLine(corners[2], corners[1]);
			printLine(corners[3], corners[2]);
			printLine(corners[0], corners[3]);
		}
	}

	private void printLine (Vector3 p1, Vector3 p2) {
		p1.y = mainY;
		p2.y = mainY;
		Vector3 dir = (p2 - p1);

		Ray ray = new Ray(p1, dir);
		RaycastHit hit = new RaycastHit ();
		if (Physics.Raycast (ray, out hit, 200)) {
			draw.drawLine (p1, hit.point, Color.red);
			Debug.Log("Collision: "+hit.collider.name);
		}

	}

	private void printLineOld (Vector3 p1, Vector3 p2) {
		Vector3 dir = (p2 - p1).normalized * 100;
		dir = p1 + dir;
		dir = new Vector3 (dir.x, mainY, dir.z);
		p1.y = mainY;
		draw.drawLine (p1, dir, Color.red);
	}

	private Vector3[] getCorners (GameObject obj) {
		Vector3[] corners = new Vector3[4];
		Vector3 min = obj.renderer.bounds.min;
		Vector3 max = obj.renderer.bounds.max;
		
		corners [0] = new Vector3 (min.x, mainY, min.z);
		corners [1] = new Vector3 (min.x, mainY, max.z);
		corners [2] = new Vector3 (max.x, mainY, max.z);
		corners [3] = new Vector3 (max.x, mainY, min.z);
		
		return corners;
	}

	// Update is called once per frame
	void Update () {
	
	}
}
