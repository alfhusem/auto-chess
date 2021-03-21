﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{
    public int width = 6;
    public int height = 6;
		public int scale = 15;
		int searchFrontierPhase;

    public HexCell cellPrefab;
		public Text cellLabelPrefab;

		HexCell currentPathFrom, currentPathTo;
		bool currentPathExists;

		Canvas gridCanvas;
    HexCell[] cells;
		HexMesh hexMesh;
		HexCellPriorityQueue searchFrontier;


		public Color defaultColor = Color.green;

    void Awake()
    {

        cells = new HexCell[height * width];
				gridCanvas = GetComponentInChildren<Canvas>();
				hexMesh = GetComponentInChildren<HexMesh>();

				this.transform.localScale *= scale;

        for (int z = 0, i = 0; z < height; z++)
				{
					for (int x = 0; x < width; x++)
					{
						CreateCell(x, z, i++);
					}
				}
			}

		void Start ()
		{
				hexMesh.Triangulate(cells);
		}

	void CreateCell (int x, int z, int i)
	{
		Vector3 position;
		position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
		position.y = 0f;
		position.z = z * (HexMetrics.outerRadius * 1.5f);

		HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
		//cell.transform.localScale *= 10;
		cell.transform.SetParent(transform, false);
		cell.transform.localPosition = position;
		cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
		cell.color = defaultColor;

		if (x > 0) {
			cell.SetNeighbor(HexDirection.W, cells[i - 1]);
		}
		if (z > 0) {
			if ((z & 1) == 0) {
				cell.SetNeighbor(HexDirection.SE, cells[i - width]);
				if (x > 0) {
					cell.SetNeighbor(HexDirection.SW, cells[i - width - 1]);
				}
			}
			else {
				cell.SetNeighbor(HexDirection.SW, cells[i - width]);
				if (x < width - 1) {
					cell.SetNeighbor(HexDirection.SE, cells[i - width + 1]);
				}
			}
		}

		Text label = Instantiate<Text>(cellLabelPrefab);
		label.rectTransform.SetParent(gridCanvas.transform, false);
		label.rectTransform.anchoredPosition = new Vector3(position.x, position.z);
		//label.text = cell.coordinates.ToStringOnSeparateLines();

		cell.uiRect = label.rectTransform;
  }


	//Temp
	/*
	void Update () {
		if (Input.GetMouseButton(0)) {
			HandleInput();
		}
	}

	void HandleInput () {
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit)) {
			ColorCell(hit.point, Color.magenta);
		}
	}
	*/




	public HexCell GetCell (Vector3 position) {
		position = transform.InverseTransformPoint(position); // * scale
		HexCoordinates coordinates = HexCoordinates.FromPosition(position);
		int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;

		//cell.obstacle = true;
		//FindDistancesTo(cell);

		return cells[index];
	}

	public void FindPath (HexCell fromCell, HexCell toCell, int speed) {
		ClearPath();
		currentPathFrom = fromCell;
		currentPathTo = toCell;
		currentPathExists = Search(fromCell, toCell, speed);
		ShowPath(speed);

	}

	bool Search (HexCell fromCell, HexCell toCell, int speed) {
		searchFrontierPhase += 2;

		if (searchFrontier == null) {
			searchFrontier = new HexCellPriorityQueue();
		}
		else {
			searchFrontier.Clear();
		}

		fromCell.SearchPhase = searchFrontierPhase;
		fromCell.Distance = 0;
		searchFrontier.Enqueue(fromCell);
		while (searchFrontier.Count > 0) {
			HexCell current = searchFrontier.Dequeue();
			current.SearchPhase += 1;

			if (current == toCell) {
				return true;
			}

			int currentTurn = current.Distance / speed;

			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
				HexCell neighbor = current.GetNeighbor(d);
				if (
					neighbor == null ||
					neighbor.SearchPhase > searchFrontierPhase
				) {
						continue;
				}
				if (neighbor.obstacle) {
					// TODO more spesific
					continue;
				}
				int moveCost = 1;
				//TODO different movecost per terrain

				int distance = current.Distance + moveCost;
				int turn = distance / speed;
				if (turn > currentTurn) {
					distance = turn * speed + moveCost;
				}

				if (neighbor.SearchPhase < searchFrontierPhase) {
					neighbor.SearchPhase = searchFrontierPhase;
					neighbor.Distance = distance;
					neighbor.PathFrom = current;
					neighbor.SearchHeuristic =
							neighbor.coordinates.DistanceTo(toCell.coordinates);
					searchFrontier.Enqueue(neighbor);
				}
				else if (distance < neighbor.Distance) {
					int oldPriority = neighbor.SearchPriority;
					neighbor.Distance = distance;
					neighbor.PathFrom = current;
					searchFrontier.Change(neighbor, oldPriority);

				}

			}
		}
		return false;
	}

	void ShowPath (int speed) {
		if (currentPathExists) {
			HexCell current = currentPathTo;
			while (current != currentPathFrom) {
				int turn = current.Distance / speed;
				current.SetLabel(turn.ToString());
				current.EnableHighlight(Color.white);
				current = current.PathFrom;
			}
		}
		currentPathFrom.EnableHighlight(Color.blue);
		currentPathTo.EnableHighlight(Color.red);
	}

	void ClearPath () {
		//TODO clear path when creating or loading map
		if (currentPathExists) {
			HexCell current = currentPathTo;
			while (current != currentPathFrom) {
				current.SetLabel(null);
				current.DisableHighlight();
				current = current.PathFrom;
			}
			current.DisableHighlight();
			currentPathExists = false;
		}
		else if (currentPathFrom) {
			currentPathFrom.DisableHighlight();
			currentPathTo.DisableHighlight();
		}
		currentPathFrom = currentPathTo = null;
	}

	public void Refresh () {
		hexMesh.Triangulate(cells);
	}


}
