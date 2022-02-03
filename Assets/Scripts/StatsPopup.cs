using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsPopup : MonoBehaviour
{   
    Transform unitList;
    Image image;
    //Transform[] keysToDelete;

    Dictionary<Transform, HexUnit> units = new Dictionary<Transform, HexUnit>();

    int child;
    
    void Start() {

        child = 0;

        unitList = FindChild(
            FindChild(gameObject.transform, "Panel"), "UnitList");

        for (int i = 0; i < unitList.childCount; i++) {  
            unitList.GetChild(i).GetComponent<Image>().sprite = null;
            unitList.GetChild(i).GetChild(0).GetComponent<Image>().sprite = null;
            unitList.GetChild(i).GetChild(1).gameObject.SetActive(false);
            
        }
    }

    //from HexGrid
    public void AddUnit(HexUnit unit) {
        if (unitList.childCount <= child){
            return;
            //child = 0;
        }

        units.Add(unitList.GetChild(child), unit);
        //remove yadayada

        //Unit model
        unitList.GetChild(child).GetComponent<Image>().sprite = unit.unitModel;
        //Item
        if (unit.item != null) {
            unitList.GetChild(child).GetChild(0).GetComponent<Image>().sprite = unit.item.image;
        }
        //Health
        unitList.GetChild(child).GetChild(1).gameObject.SetActive(true);
        unitList.GetChild(child).GetChild(1).GetComponent<Text>().text = unit.health + " / " + unit.fullHealth;
       
        child += 1;

    }

    //from GameTurn
    public void UpdateHealth() {
        if (units.Count > 0) {

            List<Transform> keysToDelete = ListPool<Transform>.Get();

            foreach (KeyValuePair<Transform, HexUnit> item in units) { 
                
                item.Key.GetChild(1).GetComponent<Text>().text = item.Value.health + " / " + item.Value.fullHealth;
                //Remove from UI
                if (item.Value.health <= 0) {
                    item.Key.GetComponent<Image>().sprite = null;
                    item.Key.GetChild(0).GetComponent<Image>().sprite = null;
                    item.Key.GetChild(1).gameObject.SetActive(false);
                    keysToDelete.Add(item.Key);
                }
            }
            //keysToDelete = units.Keys.Where(x => x.GetComponent<Image>().sprite == null).ToArray();

            //Remove from Dictionary
            foreach (Transform key in keysToDelete) { 
                units.Remove(key);
            }
        }
    }
    
    public Transform FindChild(Transform parent, string name) {
        for (int i = 0; i < parent.childCount; i++)
        {
         Transform t = parent.GetChild(i);
         if (t.name == name)
             return t;
        }
        return null;
    }
}