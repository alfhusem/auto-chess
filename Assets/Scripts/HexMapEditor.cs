using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;


public class HexMapEditor : MonoBehaviour
{
    public Color[] colors;
	public HexGrid hexGrid;
	private Color activeColor;
	private Color defaultColor;

	bool editMode;
	HexCell previousCell, searchFromCell, searchToCell;

	void Awake () {
		SelectColor(0);
		defaultColor = Color.white;
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
				if (searchFromCell != currentCell) {
					if (searchFromCell) {
						searchFromCell.DisableHighlight();
					}
					searchFromCell = currentCell;
					searchFromCell.EnableHighlight(Color.blue);
					if (searchToCell) {
						hexGrid.FindPath(searchFromCell, searchToCell, 4);
					}
				}

			}
			else if (searchFromCell && searchFromCell != currentCell) {
				if (searchFromCell != currentCell) {
					searchToCell = currentCell;
					hexGrid.FindPath(searchFromCell, searchToCell, 4);
				}
			}
			previousCell = currentCell;

		}
		else {
			previousCell = null;
		}

	}

	void EditCell (HexCell cell) {
		//cell.color = activeColor;
		StopAllCoroutines();
		StartCoroutine(toggleObstacle(cell));
		if (cell.obstacle){
			cell.color = defaultColor;
		}
		else{
			cell.color = Color.red;
		}
		hexGrid.Refresh();
	}

	public void SelectColor (int index) {
		activeColor = colors[index];
		//activeColor = Color.yellow;
	}

	public void SetEditMode (bool toggle) {
		editMode = toggle;
	}

	IEnumerator toggleObstacle (HexCell cell) {
		//WaitForSeconds delay = new WaitForSeconds(1 / 60f);
		yield return null;

		if (cell.obstacle){
			cell.obstacle = false;
			//cell.color = defaultColor;
		}
		else{
			cell.obstacle = true;
			//cell.color = Color.red;
		}

	}

}
