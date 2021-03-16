using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class HexMapEditor : MonoBehaviour
{
    public Color[] colors;
	public HexGrid hexGrid;
	private Color activeColor;

	bool editMode;
	HexCell previousCell, searchFromCell, searchToCell;

	void Awake () {
		SelectColor(0);
	}

	void Update () {
		if (Input.GetMouseButton(0) &&
			!EventSystem.current.IsPointerOverGameObject()
		) {
			HandleInput();
		}
		else {
			previousCell = null;
		}

	}

	void HandleInput () {
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit)) {
			HexCell currentCell = hexGrid.GetCell(hit.point);
			//TODO editCells chunks
			if (editMode) {
					EditCell(currentCell); //TODO EditCells
			}
			else if (
				Input.GetKey(KeyCode.LeftShift) && searchToCell != currentCell
			) {
					if (searchFromCell) {
						searchFromCell.DisableHighlight();
					}
					searchFromCell = currentCell;
					searchFromCell.EnableHighlight(Color.blue);
					if (searchToCell) {
						hexGrid.FindPath(searchFromCell, searchToCell);
					}

			}
			else if (searchFromCell && searchFromCell != currentCell) {
				searchToCell = currentCell;
				hexGrid.FindPath(searchFromCell, searchToCell);
			}
			previousCell = currentCell;

		}
		else {
			previousCell = null;
		}

	}

	void EditCell (HexCell cell) {
		cell.color = activeColor;
		hexGrid.Refresh();
	}

	public void SelectColor (int index) {
		activeColor = colors[index];
		activeColor = Color.yellow;
	}

	public void SetEditMode (bool toggle) {
		editMode = toggle;
	}

}
