using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

	public Texture image;

	public float left = 350, top = 200;
	public float width = 250, hight = 30;
	public float offset = 40;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), image);

		if (GUI.Button (new Rect (left, top, width, hight), "Static Guard")) {
			Debug.Log("Static Guard");
			Application.LoadLevel (1);
		}
		if (GUI.Button (new Rect (left, top+offset, width, hight), "Coop Searching (with VRP)")) {
			Debug.Log("Coop Searching (with VRP)");
			Application.LoadLevel (2);
		}
		if (GUI.Button (new Rect (left, top+offset*2, width, hight), "Pursuit Evasion")) {
			Debug.Log("Pursuit Evasion");
			Application.LoadLevel (3);
		}
		if (GUI.Button (new Rect (left, top+offset*3, width, hight), "Credits")) {
			Debug.Log("Credits");
			Application.LoadLevel (4);
		}
	}
}
