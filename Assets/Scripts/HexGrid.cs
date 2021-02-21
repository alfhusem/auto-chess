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

		Text label = Instantiate<Text>(cellLabelPrefab);
		label.rectTransform.SetParent(gridCanvas.transform, false);
		label.rectTransform.anchoredPosition = new Vector3(position.x, position.z);
		label.text = cell.coordinates.ToStringOnSeparateLines();
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
			Debug.Log("hit " + hit.point);
			TouchCell(hit.point);
		}
	}

	void TouchCell (Vector3 position) {
		position = transform.InverseTransformPoint(position); // * scale
		Debug.Log("new pos " + position);
		HexCoordinates coordinates = HexCoordinates.FromPosition(position);
		int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
		HexCell cell = cells[index];
		cell.color = touchedColor;
		hexMesh.Triangulate(cells);

		Debug.Log("touched at " + coordinates.ToString());
	}


}
