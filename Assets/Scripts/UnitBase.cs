using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : MonoBehaviour
{
    private Vector3 dir;
    private Vector3[] dirArr;
    private List<Vector3> directions;
    private float counter = 0;
    private float cellSize = 0.76f;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("I am alive!");
        //dirArr = { new Vector3(-1, 0, 0), new Vector3(1, 0, 0), new Vector3(0.5f, 0.5f), new Vector3(-0.5f, 0.5f), new Vector3(0.5f, -0.5f), new Vector3(-0.5f, -0.5f) };
        directions = new List<Vector3>();
        directions.Add(new Vector3(-1, 0, 0));
        directions.Add(new Vector3(1, 0, 0));
        directions.Add(new Vector3(0.5f, 0.5f));
        directions.Add(new Vector3(-1, 0, 0));
        directions.Add(new Vector3(0.5f, 0.5f));
        directions.Add(new Vector3(0.5f, -0.5f));
        directions.Add(new Vector3(-0.5f, 0.5f));
        directions.Add(new Vector3(-0.5f, 0.5f));
        directions.Add(new Vector3(-0.5f, -0.5f));
        directions.Add(new Vector3(0.5f, -0.5f));

    }

    // Update is called once per frame
    void Update()
    {
        counter += 1;
        StartCoroutine(HoldUp(counter));

    }

    IEnumerator HoldUp(float delay)
    {
      yield return new WaitForSeconds(delay);
      Debug.Log(directions.Count);
      /*foreach (Vector3 d in directions)
      {
          transform.position += d;
          break;
      }
      */
      if (directions.Count > 0) {
          transform.position += directions[0];
          directions.RemoveAt(0);
      }

      //Tilemap tilemap = transform.parent.GetComponent<Tilemap>();
      //Vector3Int cellPosition = tilemap.WorldToCell(transform.position);
      //transform.position = tilemap.GetCellCenterWorld(cellPosition);


    }


}
