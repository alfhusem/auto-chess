using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class HexGameUI : MonoBehaviour {

	public HexGrid grid;
	public GameTurn turn;
	public GameObject prefabBarracksPopup;

	[SerializeField] private Transform dmgPrefab;

	HexCell currentCell;
	HexUnit selectedUnit;
	HexUnit previouslySelectedUnit;

	public void SetEditMode (bool toggle) {
		enabled = !toggle;
		//grid.ShowUI(!toggle);
		grid.ClearPath();
	}

	void Start() {
		//DamagePopUp.Create(Vector3.zero, 300);
	}

	void Update () {
		foreach (HexUnit unit in grid.GetUnits()) {
			if (unit.IsDead) {
				grid.RemoveUnit(unit);
			}
		}


		if (!EventSystem.current.IsPointerOverGameObject()) {
			if (Input.GetMouseButtonDown(0)) {
				DoSelection();
			}
			/*
			else if (selectedUnit && !selectedUnit.target) {
				if (Input.GetMouseButtonDown(1)) {
					//DoMove();
					HexCell targetCell =
						grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));

					if (targetCell.Unit != selectedUnit && selectedUnit) {
						selectedUnit.target = targetCell.Unit;
						//InitiateTravel();
						//TODO
					}
				}
				else {
					DoPathfinding();
				}

			}
		*/
		}
		/*if (Input.GetKeyDown(KeyCode.Space) && !turn.turnStarted) {
			StartCoroutine(turn.DoTurn());
			//DoTurn();
		}
		*/
	}

	bool UpdateCurrentCell () {
		HexCell cell =
			grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
		if (cell != currentCell) {
			currentCell = cell;
			return true;
		}
		return false;
	}

	void DoSelection () {
		grid.ClearPath();
		UpdateCurrentCell();
		if (selectedUnit) {
			if (selectedUnit.target) {
				selectedUnit.target = null;
			}
		}
		if (currentCell) {
			//previouslySelectedUnit = selectedUnit;
			//selectedUnit = currentCell.Unit;

			// Select barracks
			if( currentCell.Prop) {
				//CreateBarracksPopup().Init(0);
				prefabBarracksPopup.SetActive(true);

			}
			else {
				prefabBarracksPopup.SetActive(false);
			}
		}
	}


	public static string ListToString(List<HexCell> list) {
		string print = "";
		foreach (HexCell cell in list) {
			print += cell.Position + " ";
		}
		return print;
	}

	public static string ListToString(List<HexUnit> list) {
		string print = "";
		foreach (HexUnit unit in list) {
			print += unit.faction + "-" + unit.Location.Position + " ";
		}
		return print;
	}

	public BarracksPopup CreateBarracksPopup() {
		GameObject barracks = Instantiate(Resources.Load("BarracksPopup") as GameObject);
		//UnityEditor.Selection.activeObject = barracks;
		//BarracksPopup barracks = Instantiate(prefabBarracksPopup);
		return barracks.GetComponent<BarracksPopup>();
	}





}
