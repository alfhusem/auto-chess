using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BarracksPopup : MonoBehaviour
{
    public List<int> summonQueue = new List<int>();

    public Button unit1;
    public Button unit2;
    public Button unit3;
    public Text unit1cost;
    public Text unit2cost;
    public Text unit3cost;
    public Text unit1text;
    public Text unit2text;
    public Text unit3text;
    public Button close;

    //public HexGameUI gameUI;
    public HexGrid grid;
    //public HexMapEditor mapEditor;
    //public GameObject grid;

    Transform queue;

    void Start() {
        gameObject.SetActive(false);

        queue = FindChild(FindChild(gameObject.transform, "Panel"),"SummonQueue");
        for (int i = 0; i < queue.childCount; i++) {
            for (int j = 0; j < queue.GetChild(i).childCount; j++) {
                queue.GetChild(i).GetChild(j).gameObject.SetActive(false);
            }
        }
    }


    //Not used
    public void Destroy() {
        GameObject.Destroy(this.gameObject);
    }

    public void SummonUnit(int unit) {
        //HexCell cell = grid.GetProps()[0].Location;
        //mapEditor.CreateUnit(unit, cell.GetNeighbor(pos));
        summonQueue.Add(unit);
        if(summonQueue.Count < 6) {
            UpdateSummonQueue();
        }
    }

    public void UpdateSummonQueue() {

        for (int i = 0; i < queue.childCount; i++) {
            for (int j = 0; j < queue.GetChild(i).childCount; j++) {
                queue.GetChild(i).GetChild(j).gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < queue.GetChild(0).childCount; i++) { //assumes all summon queues are same length
            if (summonQueue.Count > i) {
                if (summonQueue[i] == 0) {
                    int j = i + 1;
                    FindChild(
                        FindChild(queue, "Swordsman"),
                    "unit" + j).gameObject.SetActive(true);
                }
                if (summonQueue[i] == 3) {
                    int j = i + 1;
                    FindChild(
                        FindChild(queue, "Archer"),
                    "unit" + j).gameObject.SetActive(true);
                }
            }
            /*else {
                int j = i + 1;
                FindChild(
                    FindChild(queue, "Swordsman"),
                "unit" + j).gameObject.SetActive(false);
                FindChild(
                    FindChild(queue, "Archer"),
                "unit" + j).gameObject.SetActive(false);

            }*/

        }
    }

    /*public void UpdateSummonQueue() {

        for (int i = 0; i < summonQueue.Count; i++) {
            if (summonQueue[i] == 0) {
                int j = i + 1;
                FindChild(
                    FindChild(queue, "Swordsman"),
                "unit" + j).gameObject.SetActive(true);
            }
            if (summonQueue[i] == 3) {
                int j = i + 1;
                FindChild(
                    FindChild(queue, "Archer"),
                "unit" + j).gameObject.SetActive(true);
            }
        }
    }*/

    public Transform FindChild(Transform parent, string name) {
        for (int i = 0; i < parent.childCount; i++)
        {
         Transform t = parent.GetChild(i);
         if (t.name == name)
             return t;
        }
        return null;
    }

    /*void Start() {
        Button btn = unit1.GetComponent<Button>();
        btn.onClick.AddListener(() => {
            //BarracksPopup barracks = gameUI.CreateBarracksPopup();
            //Init(0);
            Debug.Log("hehehheeh");
        });
    }*/


    /*
    public void Init(int faction) {

        grid = GameObject.Find("HexGrid").GetComponent<HexGrid>();
        transform.SetParent(grid.gridCanvas.transform);
        transform.localScale = new Vector3(0.4f, 0.4f);
        GetComponent<RectTransform>().offsetMin = Vector2.zero;
        //GetComponent<RectTransform>().offsetMax = Vector2.one;

        if (faction == 0) {
            unit1cost.text = "$10";
            unit2cost.text = "$15";
            unit3cost.text = "$20";
        }

        //Button closeButton = close.GetComponent<Button>();
        close.onClick.AddListener(() => {
            GameObject.Destroy(this.gameObject);
        });


        unit1.onClick.AddListener(() => {
            Debug.Log("yeeeee");
        });

    }
    */


}
