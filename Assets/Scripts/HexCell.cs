using UnityEngine;
using UnityEngine.UI;

public class HexCell : MonoBehaviour {

	public HexCoordinates coordinates;
	public Color color;
	public RectTransform uiRect;

	public bool obstacle { get; set; }
	public int SearchHeuristic { get; set; }
	public HexCell PathFrom { get; set; }
	public HexCell NextWithSamePriority { get; set; }
	public int SearchPhase { get; set; }


	//TODO previousCell usage
	HexCell previousCell, searchFromCell;
	int distance;

	[SerializeField]
	HexCell[] neighbors;

	public int Distance {
		get {
			return distance;
		}
		set {
			distance = value;
			//UpdateDistanceLabel();
		}
	}

	public int SearchPriority {
		get {
			return distance + SearchHeuristic;
		}
	}

	/*void UpdateDistanceLabel () {
		Text label = uiRect.GetComponent<Text>();
		label.text = distance == int.MaxValue ? "" : distance.ToString();
	}*/
	public void SetLabel (string text) {
		UnityEngine.UI.Text label = uiRect.GetComponent<Text>();
		label.text = text;
	}

	public HexCell GetNeighbor (HexDirection direction) {
		return neighbors[(int)direction];
	}

	public void SetNeighbor (HexDirection direction, HexCell cell) {
		neighbors[(int)direction] = cell;
		cell.neighbors[(int)direction.Opposite()] = this;
	}

	public void DisableHighlight () {
		Image highlight = uiRect.GetChild(0).GetComponent<Image>();
		highlight.enabled = false;
	}

	public void EnableHighlight (Color color) {
		Image highlight = uiRect.GetChild(0).GetComponent<Image>();
		highlight.color = color;
		highlight.enabled = true;
	}

}
