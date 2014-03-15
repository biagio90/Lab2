using UnityEngine;
using System.Collections;
using System;

public class rrtNode {

	private Vector3 _point;
	private rrtNode _back;
	private ArrayList _connection = new ArrayList();
	public Vector3 forward;

	public rrtNode (Vector3 p, rrtNode father) {
		this.point = p;
		this.back = father;
	}

	public void addConnection (rrtNode point2) {
		connection.Add (point2);
	}

	public bool hasConnection ()
	{
		return connection.Count != 0;
	}
	
	public ArrayList connection
	{
		//set the person name
		set { this._connection = value; }
		//get the person name 
		get { return this._connection; }
	}

	public Vector3 point
	{
		//set the person name
		set { this._point = value; }
		//get the person name 
		get { return this._point; }
	}

	public rrtNode back
	{
		//set the person name
		set { this._back = value; }
		//get the person name 
		get { return this._back; }
	}
}
