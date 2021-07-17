using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BalanceUI : MonoBehaviour
{
    private int balance;
    public Text balanceText;
    public GameLogic gameLogic;

    // Update is called once per frame
    void Update()
    {
        balance = gameLogic.GetBalance();
        balanceText.text = "$" + balance;

    }
}
