using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

/*
0: swordsman
1: demon warrior
2: rat soldier
3: archer
4: demon brute
5: posion rat
*/

public class HexMapEditor : MonoBehaviour
{
  public Color[] colors;
	public HexGrid hexGrid;
	private Color activeColor;
	private Color defaultColor;

	public HexUnit prefabSwordsman;
	public HexUnit prefabDemonWarrior;
	public HexUnit prefabRatSoldier;
	public HexUnit prefabArcher;
	public HexUnit prefabDemonBrute;
	public HexUnit prefabRatPoison;

	public HexProp prefabBarracks;

	public GameLogic gameLogic;
	public GameTurn turn;

	public int selectedFaction;

	HexCell previousCell;
	//bool enabled;

	void Awake () {
		SelectColor(0);
		defaultColor = Color.white;
		SetEditMode(true);
	}

	void Start () {
		//place barracks
		Vector3 spawnBarracks = new Vector3(0,0);
		CreateProp(0, hexGrid.GetCell(spawnBarracks));
	}

	void Update () {
		//Not real time
		if (Input.GetKeyDown(KeyCode.Space)) {

		}

		if (!EventSystem.current.IsPointerOverGameObject()) {
			if (Input.GetMouseButton(0)) {
				HandleInput();
				return;
			}
			//Debug
			if (UnitKeyPressed()) {
				if (Input.GetKey(KeyCode.LeftShift)) {
					DestroyUnit();
				}
				else {
					if(Input.GetKeyDown(KeyCode.U)) {
						CreateUnit(0);
					}
					else if (Input.GetKeyDown(KeyCode.I)) {
						CreateUnit(1);
					}
					else if (Input.GetKeyDown(KeyCode.O)) {
						CreateUnit(2);
					}
					else if (Input.GetKeyDown(KeyCode.P)) {
						CreateProp(0, GetCellUnderCursor());
					}
				}
				return;
			}
			//Turn based
			switch (gameLogic.currentPlayer.faction)
      {
          case 0: //human
							if(Input.GetKeyDown(KeyCode.Alpha1)) {
								CreateUnit(0); //swordman
							}
							else if (Input.GetKeyDown(KeyCode.Alpha2)) {
								CreateUnit(3); //archer
							}
              break;

          case 1: //demon
							if(Input.GetKeyDown(KeyCode.Alpha1)) {
								CreateUnit(1); //demonWarrior
							}
							else if (Input.GetKeyDown(KeyCode.Alpha2)) {
								CreateUnit(4); //demonBrute
							}
              break;

          case 2: //rat
							if(Input.GetKeyDown(KeyCode.Alpha1)) {
								CreateUnit(2); //ratSoldier
							}
							else if (Input.GetKeyDown(KeyCode.Alpha2)) {
								CreateUnit(5); //ratPoison
							}
              break;
      }
		}
		previousCell = null;

	}

	bool UnitKeyPressed() {
		if (Input.GetKeyDown(KeyCode.U) || Input.GetKeyDown(KeyCode.I)
		|| Input.GetKeyDown(KeyCode.O) || Input.GetKeyDown(KeyCode.P)) {
			return true;
		}
		return false;

	}

	void HandleInput () {
		HexCell currentCell = GetCellUnderCursor();
		if (currentCell) {
			//TODO editCells chunks
			previousCell = currentCell;

			/*

			// Pretty much just makes a cell red and an obstacle
			if (enabled) {
				EditCell(currentCell); //TODO EditCells
			}
			*/

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

	HexUnit UnitSwitch (int unit) {
		switch(unit) {
			case 0: return prefabSwordsman;
			case 1: return prefabDemonWarrior;
			case 2: return prefabRatSoldier;
			case 3: return prefabArcher;
			case 4: return prefabDemonBrute;
			case 5: return prefabRatPoison;
		}
		return null;
	}

	public void CreateUnit (int unit, HexCell cell) {
		if (cell && !cell.Unit) {
			hexGrid.AddToSummonQueue(Instantiate(UnitSwitch(unit)), cell);
		}
	}

	void CreateUnit (int unit) {
		HexCell cell = GetCellUnderCursor();
		CreateUnit(unit, cell);
	}


	void CreateProp (int prop, HexCell cell) {
		if (cell && !cell.Unit && !cell.Prop) {
			if (prop == 0) {
				hexGrid.AddProp( Instantiate(prefabBarracks), cell );
				//CreateBarracksPopup();
			}
			else if (prop == 1) {
				//TODO
			}

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

	public void SetFaction (int faction) {
		//enabled = faction == 1;
		selectedFaction = faction;
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
