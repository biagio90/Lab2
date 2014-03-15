using UnityEngine;
using System.Collections;

public class Triangle {
	public Vector2 p1, p2, p3;
	Vector2 center;

	public Triangle(Vector2 p1,Vector2 p2,Vector2 p3) {
		this.p1 = p1;
		this.p2 = p2;
		this.p3 = p3;

	}

	static public bool freeTriangle(Triangle t) {
		return (free(t.p1, t.p2) && free(t.p2, t.p3) && free(t.p3, t.p1));
	}

	public Vector2 getCenter () {
		return center;
	}

	public Vector2[] getVertex() {
		Vector2[] v = new Vector2[3];
		v [0] = p1;
		v [1] = p2;
		v [2] = p3;

		return v;
	}

	public Vector3 getCenter3D () {
		return getCenter3D (1f);
	}

	public Vector3 getCenter3D (float y) {
		return new Vector3 (center.x, y, center.y);
	}

	public ArrayList getVertex3DArray() {
		Vector3[] v = getVertex3D (2f);
		ArrayList ret = new ArrayList ();
		foreach (Vector3 v3 in v)
						ret.Add (v3);
		return ret;
	}

	public Vector3[] getVertex3D() {
		return getVertex3D (1f);
	}

	public Vector3[] getVertex3D(float y) {
		Vector3[] v = new Vector3[3];
		v [0] = new Vector3(p1.x, y, p1.y);
		v [1] = new Vector3(p2.x, y, p2.y);
		v [2] = new Vector3(p3.x, y, p3.y);
		
		return v;
	}


	static private bool free (Vector3 p1, Vector3 p2) {
		Vector3 direction = (p2 - p1).normalized;
		float sight = (p2 - p1).magnitude;
		
		Ray ray = new Ray(p1, direction);
		RaycastHit hit = new RaycastHit ();
		bool free = true;
		
		if (Physics.Raycast (ray, out hit, sight)) {
			free = false;
			//Debug.Log("Collision: "+hit.collider.name);
		}
		return free;
	}
}
