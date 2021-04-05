using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class HexUnit : MonoBehaviour
{
	HexCell location;
	List<HexCell> pathToTravel;

	int offset = 3; // position in cell

	public HexUnit target { get; set; }

	// 0:human, 1:demon, 2:rat, 3:wild
	public int faction;
	public int health;
	public int attackDamage;
	public int attackRange;

  public HexCell Location {
		get {
			return location;
		}
		set {
			if (location) {
				location.Unit = null;
			}
			location = value;
			value.Unit = this;
			transform.localPosition =
				new Vector3(value.Position.x, value.Position.z - offset, 0);
		}
	}

	public int TakeDamage (int damage) {
		health -= damage;
		if (health <= 0) {
			Die();
		}
		return health;
	}

	// Teleport
	public void Travel (List<HexCell> path) {
		Location = path[path.Count - 1];
		pathToTravel = path;
	}

	public void SetPath (List<HexCell> path) {
		pathToTravel = path;
	}

	public void TravelStep (List<HexCell> path) {

		Location = path[1];
		pathToTravel = path;
		pathToTravel.RemoveAt(0);

	}

	public void Die () {
		location.Unit = null;
		Destroy(gameObject);
	}

	public bool IsValidDestination (HexCell cell) {
		return !cell.obstacle; //&& !cell.Unit;
	}


}
