using UnityEngine;
using System;
using System.Collections;

public class CameraControl : MonoBehaviour {

	private Vector3 cameraOffset = new Vector3();
	private Vector3 targetCameraOffset = new Vector3();
	private float targetRotationIncrement = 0.0f;

	Plane floorPlane;


	// Use this for initialization
	void Start () {
	
		floorPlane = new Plane(Vector3.up, new Vector3(0.0f, 0.0f));
		cameraOffset = new Vector3 (0f, 0f, -4f);
		targetCameraOffset = cameraOffset;
	}
	
	// Update is called once per frame
	void Update () {
	

		MapGen mapgen = GameObject.Find("Map").GetComponent<MapGen>();
		if (mapgen != null) 
		{
			transform.position = new Vector3( mapgen.tileCursor.x + 0.5f, 0, mapgen.tileCursor.y + 0.5f) + cameraOffset + new Vector3 (0f, 4f, 0f);
			transform.LookAt(new Vector3( mapgen.tileCursor.x + 0.5f, 1.0f, mapgen.tileCursor.y + 0.5f ));

			if (Input.GetKeyDown(KeyCode.W) ) 
			{
				Vector2 targetPos = mapgen.tileCursorTarget + new Vector2((cameraOffset.x == 0f) ? 0f : -Mathf.Sign(cameraOffset.x), (cameraOffset.z == 0f) ? 0f :-Mathf.Sign(cameraOffset.z));
				if ( mapgen.validDigLocation(targetPos))
				{
					mapgen.tileCursorTarget = targetPos;
				}
			}
		}


		if (Input.GetKeyDown(KeyCode.E) && (cameraOffset == targetCameraOffset)) {
			targetCameraOffset = Quaternion.Euler(0, +90, 0) * targetCameraOffset;
			targetRotationIncrement = 10;
		}

		if (Input.GetKeyDown(KeyCode.Q) && (cameraOffset == targetCameraOffset) ) {
			targetCameraOffset = Quaternion.Euler(0, -90, 0) * targetCameraOffset;
			targetRotationIncrement = -10;
		}

		if (cameraOffset != targetCameraOffset) {
			//float sign = Mathf.Sign(Vector3.Dot(cameraOffset, targetCameraOffset));
			//Debug.Log (sign);
			cameraOffset = Quaternion.Euler (0, targetRotationIncrement, 0) * cameraOffset;
		} 
		else 
		{
			cameraOffset.x = Mathf.Round(cameraOffset.x * 100f)/100f;
			cameraOffset.z = Mathf.Round(cameraOffset.z * 100f)/100f;
			targetCameraOffset = cameraOffset;
		}





	}
}
