using UnityEngine;
using System.Collections;

public class Area {

	private ArrayList vertex = new ArrayList();
	private Vector2 center;
	private float main_y = 1f;

	public Area(Vector3 center){
		this.center = new Vector2(center.x, center.z);
		main_y = center.y;
	}

	public Vector3 center3D
	{
		//set the person name
		//set { this.center = value; }
		//get the person name 
		get { return new Vector3(center.x, main_y, center.y); }
	}

	public ArrayList vertexs
	{
		//set the person name
		set { this.vertex = value; }
		//get the person name 
		get { return vertex; }
	}
}
