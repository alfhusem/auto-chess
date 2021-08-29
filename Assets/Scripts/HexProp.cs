using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexProp : MonoBehaviour
{
	HexCell location;

	public int offset; // position in cell (sprite)

    public HexCell Location {
		get {
			return location;
		}
		set {
			if (location) {
				location.Prop = null;
			}
			location = value;
			value.Prop = this;
			transform.localPosition =
				new Vector3(value.Position.x, value.Position.z - offset, 0);
		}
	}

}
