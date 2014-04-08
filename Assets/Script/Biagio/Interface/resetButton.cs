using UnityEngine;
using System.Collections;

public class resetButton : MonoBehaviour {

	public float left = 10, top = 550;
	public float width = 100, hight = 30;

	void OnGUI() {
		if (GUI.Button (new Rect (left, top, width, hight), "Back to Menu")) {
			Debug.Log("Reset");
			Application.LoadLevel (0);
		}
	}
}
