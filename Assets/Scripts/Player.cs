using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int faction {get;}
    public int balance {set; get;}

    public Player(int faction) {
        this.faction = faction;
        this.balance = 50;
    }
}
