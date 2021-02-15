using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        Unit human = new Unit(new Vector2Int(5, 5));
        Debug.Log("human " + human.getPos());
        human.setPos(new Vector2Int(7, 6));
        Debug.Log("human now " + human.getPos());
    }


}
