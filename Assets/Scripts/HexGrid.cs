using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{
    public int width = 6;
    public int height = 6;
		public int scale = 15;

    public HexCell cellPrefab;
		public Text cellLabelPrefab;

		Canvas gridCanvas;
    HexCell[] cells;
		HexMesh hexMesh;

		public Color defaultColor = Color.white;
		public Color touchedColor = Color.magenta;


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

	void Update () {
		if (Input.GetMouseButton(0)) {
			HandleInput();
		}
	}

	void HandleInput () {
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit)) {
			TouchCell(hit.point);
		}
	}

	void TouchCell (Vector3 position) {
		position = transform.InverseTransformPoint(position); // * scale
		HexCoordinates coordinates = HexCoordinates.FromPosition(position);
		int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
		HexCell cell = cells[index];
		cell.color = touchedColor;
		hexMesh.Triangulate(cells);

		cell.obstacle = true;
		FindDistancesTo(cell);

		//Debug.Log("touched at " + coordinates.ToString());
	}

	public void FindDistancesTo (HexCell cell) {
		StopAllCoroutines();
		StartCoroutine(Search(cell));

		//TODO StopAllCoroutines when loading map

	}

	IEnumerator Search (HexCell cell) {
		for (int i = 0; i < cells.Length; i++) {
			cells[i].Distance = int.MaxValue;
		}
		WaitForSeconds delay = new WaitForSeconds(1 / 60f);
		Queue<HexCell> frontier = new Queue<HexCell>();
		cell.Distance = 0;
		frontier.Enqueue(cell);
		while (frontier.Count > 0) {
			yield return delay;
			HexCell current = frontier.Dequeue();
			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
				HexCell neighbor = current.GetNeighbor(d);
				if (neighbor == null || neighbor.Distance != int.MaxValue) {
					continue;
				}
				if (neighbor.obstacle) {
					// TODO more spesific
					continue;
				}
				neighbor.Distance = current.Distance + 1;
				frontier.Enqueue(neighbor);
			}
		}
	}


}
