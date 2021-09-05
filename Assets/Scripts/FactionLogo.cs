using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FactionLogo : MonoBehaviour
{

    public bool isVisable;
    public Image image;
    public GameLogic logic;

    void Awake() {
    try{
        if(logic.currentPlayer.faction == 0) {
            isVisable = true;
            image.enabled = true;
        }
        else {
            isVisable = false;
            image.enabled = false;
        }
    }
    catch {
    }


    }

    public void ToggleImage(bool toggle) {
        isVisable = toggle;
        image.enabled = toggle;

    }

}
