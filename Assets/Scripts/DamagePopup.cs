using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class DamagePopup : MonoBehaviour
{

    public static DamagePopup Create(Vector3 position, int damageAmount) {
        Transform damagePopupTransform =
         Instantiate(GameAssets.i.pfDamagePopup, position, Quaternion.identity);
        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(damageAmount);

        return damagePopup;
    }

    public static DamagePopup Create(HexUnit unit, int damageAmount) {
        Transform damagePopupTransform =
         Instantiate(GameAssets.i.pfDamagePopup, unit.Location.Position, Quaternion.identity);
        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(damageAmount);

        return damagePopup;
    }

    private static int sortingOrder;
    private const float DISAPPEAR_TIMER_MAX = 1f;

    private TextMeshPro textMesh;
    private float disappearTimer;
    private Color textColor;

    private void Awake() {
        textMesh = transform.GetComponent<TextMeshPro>();
    }

    public void Setup(int damageAmount) {
        textMesh.SetText(damageAmount.ToString());
        textColor = textMesh.color;
        disappearTimer = DISAPPEAR_TIMER_MAX;
        sortingOrder++;
        textMesh.sortingOrder = sortingOrder;


    }

    public void Update() {
        float moveYSpeed = 20f;
        //transform.position += new Vector3(0, moveYSpeed) * Time.deltaTime;
        disappearTimer -= Time.deltaTime;

        if(disappearTimer > DISAPPEAR_TIMER_MAX * .5f) {
            //First half
            float increaseScaleAmount = 3f;
            //transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
        }
        else{
            //Second half
            float decreaseScaleAmount = 3f;
            //  transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
        }

        if(disappearTimer < 0) {
            //Start disappearing
            float disappearSpeed = 3f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if(textColor.a < 0) {
                Destroy(gameObject);
            }
        }
    }
}
