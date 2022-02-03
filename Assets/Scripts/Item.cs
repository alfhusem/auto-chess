using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public int id;
    public string description;
    //TODO public List<Effect> effects;

    public Sprite image {
		get {
			return gameObject.transform.GetComponent<SpriteRenderer>().sprite;
		}
	}
}

