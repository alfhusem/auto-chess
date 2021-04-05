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
	public HexUnit unitPrefab;

	HexCell previousCell;
	bool enabled;

	void Awake () {
		SelectColor(0);
		defaultColor = Color.white;
		SetEditMode(true);
	}

	void Update () {
		if (!EventSystem.current.IsPointerOverGameObject()) {
			if (Input.GetMouseButton(0)) {
				HandleInput();
				return;
			}
			if (Input.GetKeyDown(KeyCode.U)) {
				if (Input.GetKey(KeyCode.LeftShift)) {
					DestroyUnit();
				}
				else {
					CreateUnit();
				}
				return;
			}
		}
		previousCell = null;

	}

	void HandleInput () {
		HexCell currentCell = GetCellUnderCursor();
		if (currentCell) {
			//TODO editCells chunks
			previousCell = currentCell;

			if (enabled) {
				EditCell(currentCell); //TODO EditCells
			}

		}
		else {
			previousCell = null;
		}

	}

	HexCell GetCellUnderCursor () {
		/*Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit)) {
			return hexGrid.GetCell(hit.point);
		}
		return null;*/
		return
			hexGrid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
	}


	void CreateUnit () {
		HexCell cell = GetCellUnderCursor();
		if (cell && !cell.Unit) {
			hexGrid.AddUnit( Instantiate(unitPrefab), cell );
		}
	}

	void DestroyUnit () {
		HexCell cell = GetCellUnderCursor();
		if (cell && cell.Unit) {
			hexGrid.RemoveUnit(cell.Unit);
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
		enabled = toggle;
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
