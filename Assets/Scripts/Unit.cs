using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{
    public enum Alliance
    {
        human,
        demon,
        rat,
        none,
    };

    private Vector2Int pos;
    private Alliance alliance;
    private Texture2D texture; // = Resources.Load<Texture2D>("Sprites/hex-sliced_113");
    private GameObject gameObject;

    public Unit(Vector2Int pos, int alliance = (int)Alliance.none)
    {
        this.pos = pos;
        /*switch (alliance)
        {
            case Alliance.human:
                this.alliance = Alliance.human;
                break;
            case "demon":
                this.alliance = Alliance.demon;
                break;
            case "rat":
                this.alliance = Alliance.rat;
                break;
        }*/
        this.alliance = (Alliance)alliance;
    }

    public Vector2Int getPos()
    {
        return this.pos;
    }

    public int getAlliance()
    {
        return (int)this.alliance;
    }

    public GameObject getGameObject()
    {
        return this.gameObject;
    }

    public void setPos(Vector2Int pos)
    {
        this.pos = pos;
    }

    public void setGameObject(GameObject go)
    {
        this.gameObject = go;
    }

}
