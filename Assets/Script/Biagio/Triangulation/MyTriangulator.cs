using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MyTriangulator {

	private Vector2[] m_points;
	
	public MyTriangulator (Vector2[] points) {
		m_points = new Vector2[points.Length];
		for (int i=0; i<points.Length; i++) {
			m_points [i] = points [i];
			//m_points [i].y = 1f;
		}
	}

	public Triangle[] Triangulate() {
		ArrayList output = new ArrayList();

		for (int i=0; i<m_points.Length-2; i++) {
			for (int j=i+1; j<m_points.Length-1; j++) {
				for (int k=j+1; j<m_points.Length; j++) {
					Triangle t = new Triangle (m_points[i], m_points[j], m_points[k]);
					if (Triangle.freeTriangle(t)) {
						output.Add(t);
					}
				}
			}
		}

		Triangle[] ret = new Triangle[output.Count];
		for (int i=0; i< output.Count; i++) {
			ret[i] = (Triangle) output[i];
		}

		return ret;
	}


}
