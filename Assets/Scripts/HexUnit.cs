using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class HexUnit : MonoBehaviour
{
	HexCell location;
	List<HexCell> pathToTravel;

	int offset = 3; // position in cell
	bool isDead;
	bool currentPathExists;
	bool facingRight;

	public HexUnit target { get; set; }

	// 0:human, 1:demon, 2:rat, 3:wild
	public int faction;
	public int health;
	public int attackDamage;
	public int attackRange;
	public int speed;
	public Animator animator;

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

	public bool IsDead {
		get {
			return isDead;
		}
	}

	public bool HasPath {
		get {
			return currentPathExists;
		}
	}

	public bool FacingRight {
		get {
			return facingRight;
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
		if(path != null) {
			currentPathExists = true;
		}
		else {
			currentPathExists = false;
		}
	}

	public List<HexCell> GetPath () {
		return pathToTravel;
	}

	public void TravelStep (List<HexCell> path) {

		Location = path[1];
		pathToTravel = path;
		pathToTravel.RemoveAt(0);

	}

	public void Die () {
		if (!isDead) {
			isDead = true;
			location.Unit = null;
			Destroy(gameObject);
			Location.SetLabel(null);
			Location.DisableHighlight();
		}
	}

	public bool IsValidDestination (HexCell cell) {
		return !cell.obstacle; //&& !cell.Unit;
	}

	public int DistanceToUnit(HexUnit targetUnit) {
		return this.Location.coordinates.DistanceTo(targetUnit.Location.coordinates);
	}


	public void Flip()
{
   // Switch the way the player is labelled as facing
   facingRight = !facingRight;
   // Multiply the player's x local scale by -1
   Vector3 theScale = transform.localScale;
   theScale.x *= -1;
   transform.localScale = theScale;
}


}
