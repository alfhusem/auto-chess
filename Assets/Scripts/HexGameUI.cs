using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class HexGameUI : MonoBehaviour {

	public HexGrid grid;
	public int fixedSpeed;

	[SerializeField] private Transform dmgPrefab;

	HexCell currentCell;
	HexUnit selectedUnit;
	HexUnit previouslySelectedUnit;
	bool turnStarted;

	public void SetEditMode (bool toggle) {
		enabled = !toggle;
		//grid.ShowUI(!toggle);
		grid.ClearPath();
	}

	void Awake () {
		fixedSpeed = 2;
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
		if (Input.GetKeyDown(KeyCode.Space) && !turnStarted) {
			/* HexUnit human = grid.GetUnits(0)[0];
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
			}*/
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
		grid.FindPath(unit.Location, unit.target.Location, fixedSpeed);
		unit.SetPath(grid.GetPath());
	}


	IEnumerator DoTurn () {

		WaitForSeconds delay = new WaitForSeconds(1f);
		turnStarted = true;

		TargetPhase();
		MovePhase();
		StartCoroutine(AttackPhase());
		yield return delay;
		turnStarted = false;

		/*
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
		*/
	}

	void TargetPhase() {
		int closestUnitDistance;
		Debug.Log("units: " +ListToString(grid.GetUnits()));
		foreach (HexUnit unit in grid.GetUnits()) {
			closestUnitDistance = 10000;
			if (grid.GetUnitsExcept(unit.faction).Count > 0) {
				foreach (HexUnit target in grid.GetUnitsExcept(unit.faction)) {
					if (unit.DistanceToUnit(target) < closestUnitDistance) {
						closestUnitDistance = unit.DistanceToUnit(target);
						unit.target = target;
					}
				}
			}
		}

	}

	void MovePhase() {
		foreach (HexUnit unit in grid.GetUnits()) {
			if (unit.target) {
				if (unit.DistanceToUnit(unit.target) > unit.attackRange){
					DoPathfinding(unit);
					TurnDirection(unit);
					DoStep(unit);
				}
			}
		}
	}

	IEnumerator AttackPhase() {
		WaitForSeconds delay = new WaitForSeconds(1/3f);
		foreach (HexUnit unit in SortUnitsBySpeed(grid.GetUnits())) {
			if (unit.target) {
				if ((unit.DistanceToUnit(unit.target) <= unit.attackRange) && unit.target) {
					StartCoroutine(Attack(unit));
					//Attack(unit);
					yield return delay;
				}
			}
		}
	}

	void DoStep (HexUnit unit) {
		if (unit.target && unit.HasPath) {
			unit.TravelStep(unit.GetPath());
		}
	}

	public IEnumerator Attack (HexUnit unit) {
		WaitForSeconds delay = new WaitForSeconds(1 / 6f);
		if (unit.animator){
			unit.animator.SetTrigger("Attack");
			yield return delay;
			unit.target.animator.SetTrigger("Hurt");
		}

		int h0 = unit.target.health;
		int h1 = unit.target.TakeDamage(unit.attackDamage);
		int d = unit.attackDamage;

		int offset = 0;
		//Vector3 pos = new Vector3(unit.target.Location.Position.x, 0,unit.target.Location.Position.y);
		//Vector3 pos = unit.target.Location.Position;
		//Vector3 pos1 = HexCoordinates.ToVector3(HexCoordinates.FromPosition(
			//unit.target.Location.Position));

		//DamagePopup!!!!
		//DamagePopup.Create(unit.target, d*-1);

		//Debug.Log("target " + unit.target.Location.Position.ToString());
		//Debug.Log("pos " + pos.ToString());
		//Debug.Log("pos1 " + pos1.ToString());


		//Debug.Log(h0 + " - " + d + " = " + h1 + " health remaining");
		if (unit.target.IsDead) {
			unit.SetPath(null);
			unit.target = null;
			grid.RemoveUnit(unit.target);
		}
		yield return delay;

	}

	public void AnimateAttack (HexUnit unit) {
		unit.animator.SetTrigger("Attack");
	}

	List<HexUnit> SortUnitsBySpeed (List<HexUnit> list) {
		list.Sort((x,y) => y.speed.CompareTo(x.speed));
		return list;
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

	void TurnDirection(HexUnit unit) {
		if ( unit.GetPath()[0].Position.x <
			unit.GetPath()[1].Position.x ) {
				if(!unit.FacingRight) {
					unit.Flip();
				}
		}
		else if (unit.FacingRight) {
			unit.Flip();
		}
	}



}
