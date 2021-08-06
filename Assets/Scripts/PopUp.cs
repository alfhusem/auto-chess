using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUp : MonoBehaviour
{
    public GameLogic logic;
	HexCell location;
	RectTransform uiRect;

	int offset = 0;

    public Vector3 Position {
        get {
            return transform.localPosition;
        }
    }

	public HexCell Location {
		  get {
			  return location;
		  }
		  set {
			  location = value;
			  transform.localPosition =
				  new Vector3(value.Position.x, value.Position.z - offset, 0);
		  }
	  }

    public void SetLabel (string text) {
		UnityEngine.UI.Text label = uiRect.GetComponent<Text>();
		label.text = text;
	}

    void Update()
    {

    }
}
