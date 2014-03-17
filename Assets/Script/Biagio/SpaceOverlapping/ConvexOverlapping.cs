using UnityEngine;
using System.Collections;

public class ConvexOverlapping {

	static public Area[] divideSpaceIntoArea() {
		ArrayList aree = new ArrayList ();

		GameObject[] gos;
		gos = GameObject.FindGameObjectsWithTag("area");
		foreach (GameObject go in gos) {
			aree.Add(go.transform.position);
		}

		Area[] ret = new Area[aree.Count];
		for (int i=0; i<aree.Count; i++) {
			ret[i] = new Area((Vector3) aree[i]);
		}

		return ret;
	}

}
