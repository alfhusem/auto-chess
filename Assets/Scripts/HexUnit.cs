using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexUnit : MonoBehaviour
{
	HexCell location;
	int offset = 3;

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

	public void Die () {
		location.Unit = null;
		Destroy(gameObject);
	}

	public bool IsValidDestination (HexCell cell) {
		return !cell.obstacle && !cell.Unit;
	}


}
