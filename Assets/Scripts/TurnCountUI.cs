using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnCountUI : MonoBehaviour
{
    private int turn;
    public Text turnCountText;

    void Start(){
        turn = 0;
    }

    public void IncrementTurnCount() {
        turn += 1;
        turnCountText.text = "Turn: " + turn;
    }
}
