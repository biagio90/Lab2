using UnityEngine;
using System.Collections;

public class AstarCreator {

	private int dimGrid;
	private float y;
	private float sight = 2f;
	private float step = 1f;

	public AstarCreator (int dim) {
		dimGrid = dim;
	}

	public AstarCreator (int dim, float step) {
		dimGrid = dim;
		this.step = step;
	}

	public ArrayList getPath(Vector3 from, Vector3 to) {
		y = from.y;

		ArrayList open = new ArrayList();
		Queue close = new Queue();
		Vector2 source;
		Vector2 dest;
		Vector2[,] cameFrom;
		int[,] g_score;
		int[,] f_score;
		Vector2 current;
		
		ArrayList path = new ArrayList();

		cameFrom = new Vector2[dimGrid, dimGrid];
		g_score  = new int[dimGrid, dimGrid];
		f_score  = new int[dimGrid, dimGrid];
		
		source = floorVector (from);
		dest = floorVector (to);
		cameFrom [normalize(source.x), normalize(source.y)] = Vector2.zero;
		g_score [normalize(source.x), normalize(source.y)] = 0;
		f_score [normalize(source.x), normalize(source.y)] = g_score [normalize(source.x), normalize(source.y)] + heuristic(source, dest);
		open = new ArrayList();
		close = new Queue();
		open.Add (source);

		bool end = false;
		while (open.Count != 0 && !end) {
			current = pickUp(open, f_score);

			if (Vector2.Distance(current, dest) < 2*step) {
				cameFrom[normalize(dest.x), normalize(dest.y)] = current;
				path = findPath (cameFrom, current, source);
				end = true;
			}
			else {
				close.Enqueue(current);
				
				// for each neighbor
				ArrayList neighborhood = new ArrayList();
				if(isFree(current, up (current)))    neighborhood.Add (up (current));
				if(isFree(current, left (current)))  neighborhood.Add (left (current));
				if(isFree(current, down (current)))  neighborhood.Add (down (current));
				if(isFree(current, right (current))) neighborhood.Add (right (current));
				if(isFree(current, upLeft (current)))    neighborhood.Add (upLeft (current));
				if(isFree(current, downLeft (current)))  neighborhood.Add (downLeft (current));
				if(isFree(current, downRight (current))) neighborhood.Add (downRight (current));
				if(isFree(current, upRight (current))) 	 neighborhood.Add (upRight (current));

				foreach (Vector2 neighbor in neighborhood) {
					if (!isPresent(close, neighbor)) {
						int aux_g = g_score[normalize(current.x), normalize(current.y)] + distance(current, neighbor);
						
						bool notPresent = !isPresent(open, neighbor);
						if( notPresent  || aux_g < g_score[normalize(neighbor.x), normalize(neighbor.y)]) {
							cameFrom[normalize(neighbor.x), normalize(neighbor.y)] = current;
							g_score[normalize(neighbor.x), normalize(neighbor.y)] = aux_g;
							f_score[normalize(neighbor.x), normalize(neighbor.y)] = g_score[normalize(neighbor.x), normalize(neighbor.y)]
							+ heuristic(neighbor, dest);
							if (notPresent) {
								open.Add (neighbor);
							}
						}
					}
				}
			}
		}

		//path = optimizePath (path);
		return path;
	}

	private ArrayList optimizePath1 (ArrayList path) {
		ArrayList ret = new ArrayList();
		if (path == null || path.Count == 0)
						return ret;

		ret.Add(path[0]);
		ret.Add(path[1]);

		for (int i=2; i<path.Count; i++) {
			Vector3 last = (Vector3)ret[ret.Count-1];
			Vector3 current = (Vector3)path[i];
			if (last.x == current.x || last.z == current.z) {
				ret[ret.Count-1] = path[i];
			} else {
				ret.Add(path[i]);
			}
		}

		ret.Add (path[path.Count-1]);

		return ret;
	}

	private ArrayList optimizePath (ArrayList path) {
		ArrayList ret = new ArrayList();

		ret.Add(path[0]);
		ret.Add(path[1]);
		Vector3 lastDir = ((Vector3)path[1] - (Vector3)path[0]).normalized;

		for(int i=2; i<path.Count; i++) {
			Vector3 dir = ((Vector3)path[i] - (Vector3)ret[ret.Count-1]).normalized;

			if (dir == lastDir) {
				ret[ret.Count-1] = path[i];
			} else {
				//Debug.Log("last dir "+lastDir+" Dir "+dir);
				ret.Add(path[i]);
				lastDir = dir;
			}
		}

		//ret.Add (path[path.Count-1]);
		return ret;
	}

	private ArrayList findPath (Vector2[,] cameFrom, Vector2 current, Vector2 source) {
		ArrayList p = new ArrayList ();
		while (current.x != source.x || current.y != source.y) {
			p.Add(to3D(current));
			current = cameFrom[normalize (current.x), normalize (current.y)];
		}
		p.Add(to3D(source));
		return p;
	}

	private int normalize(float num) {
		return Mathf.FloorToInt (num + (dimGrid / 2));
	}
	
	private int distance (Vector2 from, Vector2 to) {
		return 1;
	}

	private Vector2 pickUp(ArrayList open, int[,] f_score) {
		int min = 100000;
		Vector2 res=new Vector2();
		foreach (Vector2 pos in open) {
			//Debug.Log(f_score[normalize(pos.x), normalize(pos.y)]);
			if(f_score[normalize(pos.x), normalize(pos.y)] < min ) {
				min = f_score[normalize(pos.x), normalize(pos.y)];
				res = pos;
			}
		}
		open.Remove (res);
		return res;
	}
	
	private bool isFree (Vector2 pos1, Vector2 pos2) {
		Vector3 p = new Vector3 (pos1.x, 1, pos1.y);
		Vector3 p2 = new Vector3 (pos2.x, 1, pos2.y);
		Vector3 dir = p2 - p;

		Ray ray = new Ray(p, dir);
		RaycastHit hit = new RaycastHit ();
		bool collision = Physics.Raycast (ray, out hit, dir.magnitude);

		//bool capsule = Physics.CheckCapsule(p, p2, step);
		//return (!collision && !capsule);

		return !collision;// && isFreeRightAndLeft(pos2);
	}

	private bool isFreeRightAndLeft(Vector2 pos) {
		Vector3 p = new Vector3 (pos.x, 1, pos.y);
		Vector2 r2 = right (p);
		Vector2 l2 = left (p);
		Vector3 r = new Vector3 (r2.x, 1, r2.y);
		Vector3 l = new Vector3 (l2.x, 1, l2.y);
		Vector3 dir = r - l;

		Ray ray = new Ray(l, dir);
		RaycastHit hit = new RaycastHit ();
		bool collision = Physics.Raycast (ray, out hit, dir.magnitude);

		return !collision;
	}

	private bool isFree (Vector2 pos) {
		Vector3 p = new Vector3 (pos.x, 1, pos.y);
		//return !Physics.CheckSphere (p, sight);

		Collider[] hitColliders = Physics.OverlapSphere(p, sight);
		int i = 0;
		bool collision = false;
		while (i < hitColliders.Length) {
			//hitColliders[i].SendMessage("AddDamage");
//			Debug.Log(hitColliders[i].name);
			if (hitColliders[i].tag == "wall") {
				collision = true;
				}
			i++;
		}

		return !collision;
	}
	
	private int heuristic (Vector2 pos, Vector2 dest) {
		float d = Vector2.Distance (pos, dest);
		return Mathf.FloorToInt(d*d);
	}

	private Vector3 to3D(Vector2 pos) {
		return new Vector3 (pos.x, y+1, pos.y);
	}
	
	private bool isPresent (Queue fifo, Vector2 pos) {
		bool prensent = false;
		foreach (Vector2 v in fifo){
			if (v.x == pos.x && v.y == pos.y) {
				prensent = true;
				break;
			}
		}
		return prensent;
	}
	
	private bool isPresent (ArrayList fifo, Vector2 pos) {
		bool prensent = false;
		foreach (Vector2 v in fifo){
			if (v.x == pos.x && v.y == pos.y) {
				prensent = true;
				break;
			}
		}
		return prensent;
	}
	
	private Vector2 floorVector(Vector3 pos) {
		return new Vector2 (Mathf.Floor (pos.x), Mathf.Floor (pos.z));
	}

	
	private Vector2 up   (Vector2 pos) {return new Vector2 (pos.x, pos.y+step);}
	private Vector2 down (Vector2 pos) {return new Vector2 (pos.x, pos.y-step);}
	private Vector2 right(Vector2 pos) {return new Vector2 (pos.x+step, pos.y);}
	private Vector2 left (Vector2 pos) {return new Vector2 (pos.x-step, pos.y);}
	private Vector2 upLeft   (Vector2 pos) {return new Vector2 (pos.x-step, pos.y+step);}
	private Vector2 downLeft (Vector2 pos) {return new Vector2 (pos.x-step, pos.y-step);}
	private Vector2 upRight	 (Vector2 pos) {return new Vector2 (pos.x+step, pos.y+step);}
	private Vector2 downRight (Vector2 pos) {return new Vector2 (pos.x+step, pos.y-step);}

}
