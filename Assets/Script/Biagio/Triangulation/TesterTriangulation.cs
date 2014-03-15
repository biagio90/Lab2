using UnityEngine;
using System.Collections;

public class TesterTriangulation : MonoBehaviour {


	// Use this for initialization
	void Start () {
		GameObject[] walls = GameObject.FindGameObjectsWithTag ("wall");
		Vector3[] corners = getCorners(walls[0]);
		/*foreach (Vector3 v in corners) {
			Debug.Log (v);
		}*/

		Vector2[] vertices2D = new Vector2[walls.Length*corners.Length];
		//Debug.Log (walls.Length);
		for (int i=0; i<walls.Length; i++) {
			corners = getCorners(walls[i]);
			int j=0;
			foreach(Vector3 v in corners)  {
				Vector2 v2 = new Vector2(v.x, v.z);
				vertices2D[i*corners.Length+j] = v2;
				//Debug.Log (v2);
				j++;
			}
		}

		MyTriangulator tr = new MyTriangulator(vertices2D);
		Triangle[] triangles = tr.Triangulate();

		Vector3[] vertices = new Vector3[vertices2D.Length];
		for (int i=0; i<vertices.Length; i++) {
			//Debug.Log(vertices2D[i]);
			vertices[i] = new Vector3(vertices2D[i].x, 2.0f, vertices2D[i].y);
		}

		foreach (Triangle t in triangles) {
			//Debug.Log("Triangle: "+t.p1+" "+t.p2+" "+t.p3);
			printTrinagle(t);
		}
	}

	private void printTrinagle(Triangle t) {
		MyLine draw = new MyLine ();
		draw.drawMultipleLines (t.getVertex3DArray(), Color.red);
	}

	private Vector3[] getCorners (GameObject obj) {
		Vector3[] corners = new Vector3[4];
		Vector3 min = obj.renderer.bounds.min;
		Vector3 max = obj.renderer.bounds.max;

		corners [0] = new Vector3 (min.x, min.y, min.z);
		corners [1] = new Vector3 (max.x, max.y, max.z);
		corners [2] = new Vector3 (min.x, min.y, max.z);
		corners [3] = new Vector3 (max.x, max.y, min.z);

		return corners;
	}

	// Update is called once per frame
	void Update () {
		
	}
}
