using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMapCamera : MonoBehaviour {

	Transform stick;

	float zoom = 1f;
	float j = 0f;

	public float stickMinZoom, stickMaxZoom;

	void Awake () {
		stick = transform.GetChild(0);
	}

	void Update () {
		//float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
		/*float zoomDelta = Input.GetAxis("Vertical");
		if (zoomDelta != 0f) {
			AdjustZoom(zoomDelta);
		}*/
		AdjustZoom(j);
		j += 0.1f;
	}

	void AdjustZoom (float delta) {
		zoom = Mathf.Clamp01(zoom + delta);

		float distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);
		stick.localPosition = new Vector3(0f, 0f, distance);
	}
}
