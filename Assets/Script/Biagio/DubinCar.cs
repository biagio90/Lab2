using UnityEngine;
using System.Collections;

public class DubinCar {
	
	public float radius;
	
	private float numPointsCircle = 15.0f;
	private float precision = 1.5f;
	private float mainY = 0.5f;

	private bool debug = true;

	private MyLine draw = new MyLine();

	public DubinCar (){
		radius = 10;
	}

	public DubinCar(float radius){
		this.radius = radius;
	}
	
	public DubinCar(float radius, float numPointsCircle){
		this.radius = radius;
		this.numPointsCircle = numPointsCircle;
	}

	public DubinCar(float radius, bool debug){
		this.radius = radius;
		this.debug = debug;
	}
	
	public DubinCar(float radius, float numPointsCircle, bool debug){
		this.radius = radius;
		this.numPointsCircle = numPointsCircle;
		this.debug = debug;
	}

	public DubinCar(float radius, float numPointsCircle, float precision, bool debug){
		this.radius = radius;
		this.numPointsCircle = numPointsCircle;
		this.debug = debug;
		this.precision = precision;
	}

	public ArrayList calcolateWay (Vector3 pos, Vector3 fwd, Vector3 destination) {
		mainY = pos.y;
		//return LSL(pos, fwd, destination);
		return RSL(pos, fwd, destination);
	}

	public ArrayList RSR (Vector3 pos, Vector3 fwd, Vector3 destination) {
		mainY = pos.y;
		Vector3 c1 = Vector3.zero, c2 = Vector3.zero;
		
		ArrayList circleFrom = calculateCircle(pos,  -tangent(fwd),
		                                       radius, true, ref c1);
		circleFrom.Reverse ();
		Vector3 tan = tangent((pos-destination).normalized);
		ArrayList circleTo   = calculateCircle(destination, tan, radius, true, ref c2);
		
		Vector3 connOnCircles = tangent((c2-c1).normalized)*radius;
		Vector3 conn1 = connOnCircles+c1;
		Vector3 conn2 = connOnCircles+c2;
		
		if(debug) draw.drawLine(conn1, conn2, Color.blue);
		
		ArrayList way = createPath (circleFrom, circleTo, pos, destination, conn1, conn2);
		
		return way;
	}

	public ArrayList LSR (Vector3 pos, Vector3 fwd, Vector3 destination) {
		mainY = pos.y;
		Vector3 c1 = Vector3.zero, c2 = Vector3.zero;
		
		ArrayList circleFrom = calculateCircle(pos,  tangent(fwd),
		                                       radius, true, ref c1);
		Vector3 tan = tangent((pos-destination).normalized);
		ArrayList circleTo   = calculateCircle(destination, tan, radius, true, ref c2);
		
		Vector3 connOnCircles = tangent((c1-c2).normalized)*radius;
		Vector3 conn1 = connOnCircles+c1;
		Vector3 conn2 = connOnCircles+c2;
		
		if(debug) draw.drawLine(conn1, conn2, Color.blue);
		
		ArrayList way = createPath (circleFrom, circleTo, pos, destination, conn1, conn2);
		
		return way;
	}

	public ArrayList RSL (Vector3 pos, Vector3 fwd, Vector3 destination) {
		mainY = pos.y;
		Vector3 c1 = Vector3.zero, c2 = Vector3.zero;
		
		ArrayList circleFrom = calculateCircle(pos,  -tangent(fwd),
		                                       radius, true, ref c1);
		circleFrom.Reverse ();
		Vector3 tan = -tangent((pos-destination).normalized);
		ArrayList circleTo   = calculateCircle(destination, tan, radius, true, ref c2);
		
		Vector3 connOnCircles = tangent((c2-c1).normalized)*radius;
		Vector3 conn1 = connOnCircles+c1;
		Vector3 conn2 = connOnCircles+c2;
		
		if(debug) draw.drawLine(conn1, conn2, Color.blue);
		
		ArrayList way = createPath (circleFrom, circleTo, pos, destination, conn1, conn2);
		
		return way;
	}

	public ArrayList LSL (Vector3 pos, Vector3 fwd, Vector3 destination) {
		mainY = pos.y;
		Vector3 c1 = Vector3.zero, c2 = Vector3.zero;

		ArrayList circleFrom = calculateCircle(pos,  tangent(fwd),
		                                       radius, true, ref c1);
		Vector3 tan = -tangent((pos-destination).normalized);
		ArrayList circleTo   = calculateCircle(destination, tan, radius, true, ref c2);
		
		Vector3 connOnCircles = tangent((c1-c2).normalized)*radius;
		Vector3 conn1 = connOnCircles+c1;
		Vector3 conn2 = connOnCircles+c2;
		
		if(debug) draw.drawLine(conn1, conn2, Color.blue);

		ArrayList way = createPath (circleFrom, circleTo, pos, destination, conn1, conn2);

		return way;
	}

	private ArrayList createPath(ArrayList circleFrom, ArrayList circleTo,
	                           Vector3 pos,Vector3  destination, 
	                           Vector3 conn1, Vector3 conn2) {
		ArrayList way = new ArrayList ();
		bool found = false;
		int index = 0;
		for (int i=0; i<circleFrom.Count && !found; i++) {
			//Debug.Log(Vector3.Distance((Vector3)circleFrom[i], pos));
			if (Vector3.Distance((Vector3)circleFrom[i], pos) < precision) {
				way.Add(pos);
				found = true;
				index = i;
			} 
		}
		
		found = false;
		for (int i=index; i<circleFrom.Count && !found; i++) {
			if (Vector3.Distance((Vector3)circleFrom[i], conn1) < precision) {
				way.Add(conn1);
				found = true;
			} else {
				way.Add(circleFrom[i]);
			}
		}

		if (!found) {
			for (int i=0; i<circleFrom.Count && i<index && !found; i++) {
				if (Vector3.Distance((Vector3)circleFrom[i], conn1) < precision) {
					way.Add(conn1);
					found = true;
				} else {
					way.Add(circleFrom[i]);
				}
			}
		}

		/*
		way.Add(conn2);
		found = false;
		for (int i=0; i<circleTo.Count && !found; i++) {
			if (Vector3.Distance((Vector3)circleTo[i], destination) < precision) {
				//way.Add(destination);
				found = true;
			} else {
				way.Add(circleTo[i]);
			}
		}*/
		way.Add(destination);

		return way;
	}

	/*
	 * Tangent has in the direction of the radius
	 */
	private ArrayList calculateCircle (Vector3 point, Vector3 tangent, float radius, bool paint, ref Vector3 c) {
		Vector3 center = radius * tangent;
		
		ArrayList circle = new ArrayList ();
		for (float a = 0; a<Mathf.PI*2; a+=(Mathf.PI/numPointsCircle)) {
			float x = Mathf.Cos(a);
			float y = Mathf.Sin(a);
			Vector3 pc = new Vector3(x, 0, y) * radius + center + point;
			pc.y = mainY;
			circle.Add(pc);
		}
		if(debug) draw.drawMultipleLines (circle, Color.red);
		
		c = center+point;
		return circle;
	}
	
	private Vector3 tangent(Vector3 fwd) {
		return Vector3.Cross (fwd, Vector3.up);
	}


}
