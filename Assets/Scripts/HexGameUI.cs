using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class HexGameUI : MonoBehaviour {

	public HexGrid grid;
	public int fixedSpeed;

	HexCell currentCell;
	HexUnit selectedUnit;
	HexUnit previouslySelectedUnit;

	public void SetEditMode (bool toggle) {
		enabled = !toggle;
		//grid.ShowUI(!toggle);
		grid.ClearPath();
	}

	void Awake () {
		fixedSpeed = 2;
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
		if (Input.GetKeyDown(KeyCode.Space)) {
			HexUnit human = grid.GetUnits(0)[0];
			HexUnit demon = grid.GetUnits(1)[0];

			int closestUnitDistance = 1000;
			foreach (HexUnit target in grid.GetUnits(1)) {
				if (human.DistanceToUnit(target) < closestUnitDistance) {
					closestUnitDistance = human.DistanceToUnit(target);
					human.target = target;
				}
			}

		  closestUnitDistance = 1000;
			foreach (HexUnit target in grid.GetUnits(0)) {
				if (demon.DistanceToUnit(target) < closestUnitDistance) {
					closestUnitDistance = demon.DistanceToUnit(target);
					demon.target = target;
				}
			}
			StartCoroutine(DoTurn());
			//DoTurn();
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

	void DoPathfinding (HexUnit unit) {
		/*if (UpdateCurrentCell()) {
			if (currentCell && selectedUnit.IsValidDestination(currentCell)) {
				grid.FindPath(selectedUnit.Location, currentCell, fixedSpeed);
			}
			else {
				grid.ClearPath();
			}
		}*/
		grid.FindPath(unit.Location, unit.target.Location, fixedSpeed);
	}
	//Not used
	void DoMove () {
		if (grid.HasPath) {
			selectedUnit.Travel(grid.GetPath());
			grid.ClearPath();
		}
	}

	IEnumerator DoTurn () {

		WaitForSeconds delay = new WaitForSeconds(1f);
		foreach (HexUnit unit in grid.GetUnits()) {
			if (unit.target) {
				DoPathfinding(unit);
				if (unit.DistanceToUnit(unit.target) > unit.attackRange){
					StopAllCoroutines();
					DoStep(unit);
				}
				else {
					StartCoroutine(Attack(unit));
					//Attack(unit);
					yield return delay;
				}
			}
		}
	}

	void DoStep (HexUnit unit) {
		if (unit.target && grid.HasPath) {

			unit.Location.SetLabel(null);
			unit.Location.DisableHighlight();

			unit.TravelStep(grid.GetPath());
			grid.currentPathFrom = unit.Location;

			unit.Location.SetLabel(null);
			unit.Location.EnableHighlight(Color.red);
		}
	}

	public IEnumerator Attack (HexUnit unit) {
		WaitForSeconds delay = new WaitForSeconds(1 / 4f);
		unit.animator.SetTrigger("Attack");
		yield return delay;
		unit.target.animator.SetTrigger("Hurt");
		int h0 = unit.target.health;
		int h1 = unit.target.TakeDamage(unit.attackDamage);
		int d = unit.attackDamage;
		Debug.Log(h0 + " - " + d + " = " + h1 + " health remaining");



	}

	public void AnimateAttack (HexUnit unit) {
		unit.animator.SetTrigger("Attack");
	}





}
