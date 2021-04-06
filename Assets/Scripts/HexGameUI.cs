using UnityEngine;
using UnityEngine.EventSystems;

public class HexGameUI : MonoBehaviour {

	public HexGrid grid;
	public int fixedSpeed;
	public int selectedFaction; //moved

	HexCell currentCell;
	HexUnit selectedUnit;
	HexUnit previouslySelectedUnit;

	public void SetEditMode (bool toggle) {
		enabled = !toggle;
		//grid.ShowUI(!toggle);
		grid.ClearPath();
	}

	public void SetFaction (int faction) {
		enabled = faction == 0;
		selectedFaction = faction;
	}

	void Awake () {
		fixedSpeed = 2;
	}

	void Update () {
		if (!EventSystem.current.IsPointerOverGameObject()) {
			if (Input.GetMouseButtonDown(0)) {
				DoSelection();
			}
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
		}
		if (Input.GetKeyDown(KeyCode.Space)) {
			DoTurn();
			Debug.Log(grid.getUnits(0)[0]);
			Debug.Log(grid.getUnits(1)[0]);

		}
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
			previouslySelectedUnit = selectedUnit;
			selectedUnit = currentCell.Unit;
		}
	}

	void DoPathfinding () {
		if (UpdateCurrentCell()) {
			if (currentCell && selectedUnit.IsValidDestination(currentCell)) {
				grid.FindPath(selectedUnit.Location, currentCell, fixedSpeed);
			}
			else {
				grid.ClearPath();
			}
		}
	}
	//Not used
	void DoMove () {
		if (grid.HasPath) {
			selectedUnit.Travel(grid.GetPath());
			grid.ClearPath();
		}
	}
	// Not used
	void InitiateTravel() {
		if (grid.HasPath) {
			//selectedUnit.Travel(grid.GetPath());
			selectedUnit.SetPath(grid.GetPath());
		}
	}

	void DoTurn () {
		if (selectedUnit.target) {
			int distance =
				selectedUnit.Location.coordinates.DistanceTo(selectedUnit.target.Location.coordinates);
			if (distance > selectedUnit.attackRange){
				DoStep();
			}
			else {
				Attack();
			}
		}
		else {
			Debug.Log("He dead");
		}
	}

	void DoStep () {
		if (selectedUnit.target && grid.HasPath) {

			selectedUnit.Location.SetLabel(null);
			selectedUnit.Location.DisableHighlight();

			selectedUnit.TravelStep(grid.GetPath());
			grid.currentPathFrom = selectedUnit.Location;

			selectedUnit.Location.SetLabel(null);
			selectedUnit.Location.EnableHighlight(Color.blue);
		}
	}

	void Attack () {
		int h0 = selectedUnit.target.health;
		int h1 = selectedUnit.target.TakeDamage(selectedUnit.attackDamage);
		int d = selectedUnit.attackDamage;
		Debug.Log(h0 + " - " + d + " = " + h1 + " health remaining");
		if (h1 <= 0) {
			Debug.Log("Dead");
		}
	}

}
