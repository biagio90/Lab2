using UnityEngine;
using System.Collections;

public class MyLine {

	public void clean () {
		LineRenderer[] lines = GameObject.FindObjectsOfType<LineRenderer> ();
		foreach (LineRenderer target in lines) {
			GameObject.Destroy(target);
		}
	}

	public void drawLine(Vector3 from, Vector3 to, Color color) {
		GameObject line = new GameObject ();

		LineRenderer render = line.AddComponent<LineRenderer> ();
		//Material mat = new Material (Shader.Find("Mobile/Particles/Additive"));
		//mat.SetColor("_TintColor", color);
		//render.SetColors (color, color);
		//Material mat = new Material (Shader.Find("Diffuse"));
		//render.material = mat;
		render.SetWidth (0.1f, 0.1f);
		render.SetPosition (0, from);
		render.SetPosition (1, to);
		render.castShadows = false;
		render.receiveShadows = false;

		GameObject.Instantiate (line, Vector3.zero, Quaternion.identity);
	}

	public void drawMultipleLines (ArrayList points, Color color) {
		for (int i = 0; i<points.Count-1; i++) {
			drawLine((Vector3)points[i], (Vector3)points[i+1], color);
		}
	}

	/*****    TO DO *****/
	public void drawCircle (Vector3 center, float radius, Color color, float density) {
		float circumference = 2 * Mathf.PI * radius;
		
		int numLine = Mathf.FloorToInt(circumference / density);
		float incRadius = (radius*2) / numLine;

		Vector3 point = new Vector3 ();
		ArrayList list = new ArrayList ();
		for (int i=0; i<numLine; i++) {
			point.x = radius - incRadius*i;
			point.z = Mathf.Sqrt( radius*radius + (point.x)*(point.x) );
			list.Add ( new Vector3(center.x+point.x, center.y+point.y, center.z+point.z) );
		}

		for (int i=0; i<numLine; i++) {
			point.x = radius - incRadius*i;
			point.z = Mathf.Sqrt( radius*radius + (point.x)*(point.x) );
			list.Add ( new Vector3(point.x, point.y, -point.z) );
		}

		drawMultipleLines (list, Color.blue);
	}

	/*****    TO DO *****/
	public void drawSin (Vector3 center, float radius, Color color, float density) {
		float circumference = 2 * Mathf.PI * radius;

		int numLine = Mathf.FloorToInt(circumference / density);
		float incAngle = 360f / numLine;
		float incRadius = (radius*2) / numLine;

		Vector3 prec = new Vector3 (0, 0, radius);
		Vector3 succ = new Vector3 (0, 0, 0);

		for (int i=0; i<numLine; i++) {
			succ.x = radius - incRadius*i;
			succ.z = radius * Mathf.Sin(incAngle*i);
			drawLine (center+prec, center+succ, color);
			prec.x = succ.x;
			prec.z = succ.z;
		}
	}
}
