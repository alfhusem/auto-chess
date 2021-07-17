using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{

    public HexGameUI hexGameUI;
    public HexMapEditor hexMapEditor;
    List<Player> players = new List<Player>();

    public FactionLogo humanMask;
    public FactionLogo demonMask;
    public FactionLogo ratMask;

    int currentPlayerInt = 0;
    public Player currentPlayer {set; get;}
    int numberOfPlayers ;

    void Awake() {
        numberOfPlayers = 3;
        players.Add(new Player(0));
        players.Add(new Player(1));
        players.Add(new Player(2));
        currentPlayer = players[0];
    }

    public void EndTurn() {
        if(currentPlayerInt < numberOfPlayers-1) {
            currentPlayerInt ++;
            currentPlayer = players[currentPlayerInt];
        }
        else {
            currentPlayerInt = 0;
            currentPlayer = players[0];
        }

        toggleMask(currentPlayer.faction);
    }

    void toggleMask(int faction) {
        if(currentPlayer.faction == faction) {
            humanMask.ToggleImage(faction == 0);
            demonMask.ToggleImage(faction == 1);
            ratMask.ToggleImage(faction == 2);
        }
    }

    public void BuyPhase() {

    }

    public int GetBalance() {
        return currentPlayer.balance;
    }

    public void SetBalance(int b) {
        currentPlayer.balance = b;
    }

}
