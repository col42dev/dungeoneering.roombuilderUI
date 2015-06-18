using UnityEngine;
using System;
using System.Collections;
using System.Timers;

public class CameraControl : MonoBehaviour {

	private Vector3 cameraOffset = new Vector3();
	private Vector3 targetCameraOffset = new Vector3();
	private float targetRotationIncrement = 0.0f;

	Plane floorPlane;


	Vector3 mouseButtonDownScreenPos;
	Vector3 pinchDelta = new Vector3( 0, 0, 0);

	Touch touchZero;
	Touch touchOne;
	Touch touchZeroPan;

	bool bTouchSizing = false;
	bool bTouchPanning = false;
	bool bTouchPanningDirForward = false;
	bool bTouchRotate = false;

	GameObject roomScalerGFX;



	// Use this for initialization
	void Start () {
	
		floorPlane = new Plane(Vector3.up, new Vector3(0.0f, 0.0f));
		cameraOffset = new Vector3 (0f, 0f, -4f);
		targetCameraOffset = cameraOffset;


		roomScalerGFX = new GameObject( "rs"); //

		roomScalerGFX.transform.position = new Vector3(25, 1.2f, 25);
		
		Vector3[] vertices = new Vector3[]
		{
			new Vector3(0, 1.2f, 0),
			new Vector3(0, 1.2f, 1),
			new Vector3(1, 1.2f, 0),
			new Vector3(1, 1.2f, 1)
		};

		roomScalerGFX.AddComponent<MeshFilter>();
		
		roomScalerGFX.AddComponent<MeshRenderer>();
		
		
		roomScalerGFX.GetComponent<MeshFilter> ().mesh.name = "wallCollision";
		roomScalerGFX.GetComponent<MeshFilter> ().mesh.vertices = vertices;
		roomScalerGFX.GetComponent<MeshFilter> ().mesh.uv = new [] {
			new Vector2 (0, 0),
			new Vector2 (1, 0),
			new Vector2 (0, 1),
			new Vector2 (1, 1)
		};
		

		roomScalerGFX.GetComponent<MeshFilter> ().mesh.triangles = new [] {0, 1, 2, 2, 1, 3};

		roomScalerGFX.GetComponent<MeshFilter>().mesh.RecalculateNormals();
		roomScalerGFX.GetComponent<MeshFilter>().mesh.RecalculateBounds();
		
	

		Material mat = Resources.Load ("Materials/RoomScalarCursor", typeof(Material)) as Material; 

		Material [] mats = new Material[1];
		mats[0] = mat;
		roomScalerGFX.GetComponent<Renderer> ().materials = mats;

		roomScalerGFX.gameObject.SetActive (false);



	}

	private void OnTimedEvent_PanCamera()
	{
		if (bTouchPanning == true ) 
		{
			MapGen mapgen = GameObject.Find ("Map").GetComponent<MapGen> ();
			if (mapgen != null) {

				if ( bTouchPanningDirForward)
				{
					Vector2 targetPos = mapgen.tileCursorTarget + new Vector2 ((cameraOffset.x == 0f) ? 0f : -Mathf.Sign (cameraOffset.x), (cameraOffset.z == 0f) ? 0f : -Mathf.Sign (cameraOffset.z));
					if (mapgen.validDigLocation (targetPos)) {
						mapgen.tileCursorTarget = targetPos;
					}
				}
				else{
					Vector2 targetPos = mapgen.tileCursorTarget -  new Vector2 ((cameraOffset.x == 0f) ? 0f : -Mathf.Sign (cameraOffset.x), (cameraOffset.z == 0f) ? 0f : -Mathf.Sign (cameraOffset.z));
					if (mapgen.validDigLocation (targetPos)) {
						mapgen.tileCursorTarget = targetPos;
					}
				}
			}
		}
	}

	void OnGUI() {

		GUI.skin.label.fontSize = 64;

		MapGen mapgen = GameObject.Find("Map").GetComponent<MapGen>();
		if (mapgen != null) 
		{
			/*
			if (GUI.Button (new Rect (Screen.width * .4f, Screen.height * 0.85f, Screen.width * .2f, Screen.height * 0.12f), "DIG")) {
				mapgen.EditTile (mapgen.tileCursor.x, mapgen.tileCursor.y);
			}
			*/


		

			if ((Application.platform != RuntimePlatform.Android) && (Application.platform !=  RuntimePlatform.IPhonePlayer)) 
			{
				if (cameraOffset == targetCameraOffset) {
					if (GUI.Button (new Rect (Screen.width * .05f, Screen.height * 0.01f, Screen.width * .2f, Screen.height * 0.11f), "<")) {
						targetCameraOffset = Quaternion.Euler(0, -90, 0) * targetCameraOffset;
						targetRotationIncrement = -10;
					}
				}

				if (GUI.Button (new Rect (Screen.width * .4f, Screen.height * 0.01f, Screen.width * .2f, Screen.height * 0.11f), "^")) 
				{
					Vector2 targetPos = mapgen.tileCursorTarget + new Vector2((cameraOffset.x == 0f) ? 0f : -Mathf.Sign(cameraOffset.x), (cameraOffset.z == 0f) ? 0f :-Mathf.Sign(cameraOffset.z));
					if ( mapgen.validDigLocation(targetPos))
					{
						mapgen.tileCursorTarget = targetPos;
					}
				}

				if (cameraOffset == targetCameraOffset) {
					if (GUI.Button (new Rect (Screen.width * .75f, Screen.height * 0.01f, Screen.width * .2f, Screen.height * 0.11f), ">")) {
						targetCameraOffset = Quaternion.Euler(0, +90, 0) * targetCameraOffset;
						targetRotationIncrement = 10;
					}
				}
			}

	




		}

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

		roomScalerGFX.transform.position = new Vector3 (mapgen.tileCursorTarget.x + 0.5f, 1.1f, mapgen.tileCursorTarget.y + 0.5f);

		if ((Application.platform == RuntimePlatform.Android) || (Application.platform ==  RuntimePlatform.IPhonePlayer)) 
		{
			if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
			{
				touchZeroPan = Input.GetTouch(0);
				bTouchRotate = true;
			}


			if (Input.touchCount == 1)
			{
				// pan
				if (bTouchPanning == false)
				{
					if ( Mathf.Abs ( Input.GetTouch(0).position.y - touchZeroPan.position.y ) > Mathf.Abs ( Input.GetTouch(0).position.x - touchZeroPan.position.x )) // forward/backward quadrant
					{
						if ( Mathf.Abs ( Input.GetTouch(0).position.y - touchZeroPan.position.y) > Screen.height * .1f) // min pan threshhold
						{
							bTouchPanning = true;
							bTouchPanningDirForward = (Input.GetTouch(0).position.y  > touchZeroPan.position.y) ? true : false;
							InvokeRepeating("OnTimedEvent_PanCamera", 0.0f, 0.4f);
						}
					}
				}

				// rotate
				if ( bTouchRotate == true)
				{
					if ( Mathf.Abs ( Input.GetTouch(0).position.y - touchZeroPan.position.y ) < Mathf.Abs ( Input.GetTouch(0).position.x - touchZeroPan.position.x )) // forward/backward quadrant
					{
						if ( Mathf.Abs ( Input.GetTouch(0).position.x - touchZeroPan.position.x) > Screen.width * .15f) // min rotate threshhold
						{
							bTouchRotate = false;

							if ( Input.GetTouch(0).position.x < touchZeroPan.position.x)
							{
								targetCameraOffset = Quaternion.Euler(0, -90, 0) * targetCameraOffset;
								targetRotationIncrement = -10;
							}
							else{
								targetCameraOffset = Quaternion.Euler(0, +90, 0) * targetCameraOffset;
								targetRotationIncrement = +10;
							}
						}
					}
				}
			}

			if ( Input.touchCount != 1)
			{
				bTouchRotate = false;
				bTouchPanning=false;

				CancelInvoke("OnTimedEvent_PanCamera");
			}


			if (Input.touchCount == 2 && Input.GetTouch(1).phase == TouchPhase.Began)
			{
				roomScalerGFX.gameObject.SetActive (true);
				// Store both touches.
				touchZero = Input.GetTouch(0);
				touchOne = Input.GetTouch(1);

			}

			if (Input.touchCount == 2)
			{
				bTouchSizing = true;
				pinchDelta.x = Mathf.Abs ( Input.GetTouch(0).position.x - Input.GetTouch(1).position.x ) - Mathf.Abs( touchZero.position.x - touchOne.position.x);
				pinchDelta.y = Mathf.Abs ( Input.GetTouch(0).position.y - Input.GetTouch(1).position.y ) - Mathf.Abs( touchZero.position.y - touchOne.position.y);

				if ( pinchDelta.x < 0) pinchDelta.x = 0;
				if ( pinchDelta.y < 0) pinchDelta.y = 0;

				Vector3 widthScaler = new Vector3 (Mathf.RoundToInt ((pinchDelta.x / Screen.width) * 6), 0, 0);
				Vector3 heightScaler = new Vector3 (0, 0, Mathf.RoundToInt ((pinchDelta.y / Screen.height) * 10));
				
				Vector3[] vertices = new Vector3[]
				{
					new Vector3 (-0.5f, 0, -0.5f),
					new Vector3 (-0.5f, 0, +0.5f),
					new Vector3 (+0.5f, 0, -0.5f),
					new Vector3 (+0.5f, 0, +0.5f)
				};
				
				vertices [0] -= widthScaler;
				vertices [1] -= widthScaler;
				
				vertices [2] += widthScaler;
				vertices [3] += widthScaler;
				
				vertices [1] += heightScaler;
				
				vertices [3] += heightScaler;
				
				
				roomScalerGFX.gameObject.transform.rotation = Quaternion.Euler (0, transform.rotation.eulerAngles.y, 0);
				roomScalerGFX.GetComponent<MeshFilter> ().mesh.vertices = vertices;
			}


			if ( (bTouchSizing == true) && (Input.touchCount == 0))
			{
				//mapgen.EditTile (mapgen.tileCursorTarget.x, mapgen.tileCursorTarget.y);
				//mapgen.EditTile (mapgen.tileCursor.x, mapgen.tileCursor.y);

		
				bTouchSizing = false;

	
				Vector3 widthScaler = new Vector3 (Mathf.RoundToInt ((pinchDelta.x / Screen.width) * 6), 0, 0);
				Vector3 heightScaler = new Vector3 (0, 0, Mathf.RoundToInt ((pinchDelta.y / Screen.height) * 10));
				
				Vector3[] vertices = new Vector3[]
				{
					new Vector3 (-0.5f, 0, -0.5f),
					new Vector3 (-0.5f, 0, +0.5f),
					new Vector3 (+0.5f, 0, -0.5f),
					new Vector3 (+0.5f, 0, +0.5f)
				};
				
				vertices [0] -= widthScaler;
				vertices [1] -= widthScaler;
				vertices [2] += widthScaler;
				vertices [3] += widthScaler;
				vertices [1] += heightScaler;
				vertices [3] += heightScaler;

				roomScalerGFX.gameObject.transform.rotation = Quaternion.Euler (0, transform.rotation.eulerAngles.y, 0);
				roomScalerGFX.GetComponent<MeshFilter> ().mesh.vertices = vertices;

				for (float x =  vertices[0].x; x <=  vertices[2].x; x += 1.0f) {
					for (float z =  vertices[0].z; z <= vertices[1].z; z += 1.0f) {
						
						Vector3 tile = new Vector3 (x, 0, z);
						tile = Quaternion.Euler (0, roomScalerGFX.gameObject.transform.rotation.eulerAngles.y, 0) * tile;
						tile.x = (Mathf.Round (tile.x * 10f) / 10f) * .95f;
						tile.z = (Mathf.Round (tile.z * 10f) / 10f) * .95f;
						
						mapgen.EditTile (Mathf.Floor (roomScalerGFX.gameObject.transform.position.x + tile.x), Mathf.Floor (roomScalerGFX.gameObject.transform.position.z + tile.z));
					}
					
				}

	
				roomScalerGFX.gameObject.SetActive (false);
			}


		} else {

			if (Input.GetMouseButtonDown (0)) 
			{
				mouseButtonDownScreenPos = Input.mousePosition;
				roomScalerGFX.gameObject.SetActive (true);
			}

			if (Input.GetMouseButton (0)) 
			{
				Vector3 mouseButtonDownScreenPosVector = Input.mousePosition - mouseButtonDownScreenPos;
				mouseButtonDownScreenPosVector.x = Mathf.Abs (mouseButtonDownScreenPosVector.x);
				mouseButtonDownScreenPosVector.y = Mathf.Abs (mouseButtonDownScreenPosVector.y);

				Vector3 widthScaler = new Vector3 (Mathf.RoundToInt ((mouseButtonDownScreenPosVector.x / Screen.width) * 8), 0, 0);
				Vector3 heightScaler = new Vector3 (0, 0, Mathf.RoundToInt ((mouseButtonDownScreenPosVector.y / Screen.height) * 10));

				Vector3[] vertices = new Vector3[]
				{
					new Vector3 (-0.5f, 0, -0.5f),
					new Vector3 (-0.5f, 0, +0.5f),
					new Vector3 (+0.5f, 0, -0.5f),
					new Vector3 (+0.5f, 0, +0.5f)
				};

				vertices [0] -= widthScaler;
				vertices [1] -= widthScaler;

				vertices [2] += widthScaler;
				vertices [3] += widthScaler;

				vertices [1] += heightScaler;

				vertices [3] += heightScaler;

				roomScalerGFX.gameObject.transform.rotation = Quaternion.Euler (0, transform.rotation.eulerAngles.y, 0);
				roomScalerGFX.GetComponent<MeshFilter> ().mesh.vertices = vertices;
			} else {
				roomScalerGFX.gameObject.SetActive (false);
			}

			if (Input.GetMouseButtonUp (0)) {
				Vector3 mouseButtonDownScreenPosVector = Input.mousePosition - mouseButtonDownScreenPos;
				mouseButtonDownScreenPosVector.x = Mathf.Abs (mouseButtonDownScreenPosVector.x);
				mouseButtonDownScreenPosVector.y = Mathf.Abs (mouseButtonDownScreenPosVector.y);
			
				Vector3 widthScaler = new Vector3 (Mathf.RoundToInt ((mouseButtonDownScreenPosVector.x / Screen.width) * 8), 0, 0);
				Vector3 heightScaler = new Vector3 (0, 0, Mathf.RoundToInt ((mouseButtonDownScreenPosVector.y / Screen.height) * 10));
			
				Vector3[] vertices = new Vector3[]
				{
				new Vector3 (-0.5f, 0, -0.5f),
				new Vector3 (-0.5f, 0, +0.5f),
				new Vector3 (+0.5f, 0, -0.5f),
				new Vector3 (+0.5f, 0, +0.5f)
				};
			
				vertices [0] -= widthScaler;
				vertices [1] -= widthScaler;
				vertices [2] += widthScaler;
				vertices [3] += widthScaler;
				vertices [1] += heightScaler;
				vertices [3] += heightScaler;
			
			
				roomScalerGFX.gameObject.transform.rotation = Quaternion.Euler (0, transform.rotation.eulerAngles.y, 0);
				roomScalerGFX.GetComponent<MeshFilter> ().mesh.vertices = vertices;

		
				for (float x =  vertices[0].x; x <=  vertices[2].x; x += 1.0f) {
					for (float z =  vertices[0].z; z <= vertices[1].z; z += 1.0f) {

						Vector3 tile = new Vector3 (x, 0, z);
						tile = Quaternion.Euler (0, roomScalerGFX.gameObject.transform.rotation.eulerAngles.y, 0) * tile;
						tile.x = (Mathf.Round (tile.x * 10f) / 10f) * .95f;
						tile.z = (Mathf.Round (tile.z * 10f) / 10f) * .95f;

						mapgen.EditTile (Mathf.Floor (roomScalerGFX.gameObject.transform.position.x + tile.x), Mathf.Floor (roomScalerGFX.gameObject.transform.position.z + tile.z));
					}

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
