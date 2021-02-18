using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{
    public int width = 6;
    public int height = 6;
		public int scale = 20;

    public HexCell cellPrefab;
		public Text cellLabelPrefab;

		Canvas gridCanvas;
    HexCell[] cells;



    void Awake()
    {
        cells = new HexCell[height * width];
				gridCanvas = GetComponentInChildren<Canvas>();
				this.transform.localScale *= scale;

        for (int y = -height/2, i = 0; y < height/2; y++)
				{
					for (int x = -width/2; x < width/2; x++)
					{
						CreateCell(x, y, i++);
					}
				}
			}

	void CreateCell (int x, int y, int i)
	{
		Vector2 position;
		position.x = x * 10f;
		position.y = y * 10f;

		HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
		//cell.transform.localScale *= 10;
		cell.transform.SetParent(transform, false);
		cell.transform.localPosition = position;

		Text label = Instantiate<Text>(cellLabelPrefab);
		label.rectTransform.SetParent(gridCanvas.transform, false);
		label.rectTransform.anchoredPosition = new Vector2(position.x, position.y);
		label.text = x.ToString() + "\n" + y.ToString();
    }

}
