using UnityEngine;
using System.Collections;

public class createTime : MonoBehaviour {
	
	public float left = 10, top = 10;
	public float width = 50, hight = 30;
	public bool run = true;

	private float timer = 0.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(run) timer += Time.deltaTime;
	}

	void FixedUpdate() {

	}

	void OnGUI() {
		float seconds = timer%60;
		GUI.Label(new Rect (left, top, width, hight), Mathf.RoundToInt(seconds)+" sec");
	}
}
