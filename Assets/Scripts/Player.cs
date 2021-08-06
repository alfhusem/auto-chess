using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int faction {get;}
    public int balance {set; get;}

    List<HexUnit> units = new List<HexUnit>();

    public Player(int faction) {
        this.faction = faction;
        this.balance = 50;
    }

    public HexUnit GetUnit() {
		//if (units.Count() > 0) {
			return units[0];
		//}
    }

    public void AddUnit (HexUnit unit) {
		//if(gameLogic.currentPlayer.balance >= unit.cost) {
			units.Add(unit);
		}
}
