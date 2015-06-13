using UnityEngine;
using System;
using System.Collections;

public class CameraControl : MonoBehaviour {

	private Vector3 cameraOffset = new Vector3();
	private Vector3 targetCameraOffset = new Vector3();
	private float targetRotationIncrement = 0.0f;

	Plane floorPlane;


	Vector3 mouseButtonDownScreenPos;

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
		
	
	
	}

	void OnGUI() {

		GUI.skin.label.fontSize = 64;


		MapGen mapgen = GameObject.Find("Map").GetComponent<MapGen>();
		if (mapgen != null) 
		{
			if (GUI.Button (new Rect (Screen.width * .2f, Screen.height * 0.85f, Screen.width * .19f, Screen.height * 0.09f), "<")) {
				targetCameraOffset = Quaternion.Euler(0, -90, 0) * targetCameraOffset;
				targetRotationIncrement = -10;
			}
			if (GUI.Button (new Rect (Screen.width * .4f, Screen.height * 0.85f, Screen.width * .19f, Screen.height * 0.09f), "X")) {
				mapgen.EditTile (mapgen.tileCursor.x, mapgen.tileCursor.y);
			}
			if (GUI.Button (new Rect (Screen.width * .6f, Screen.height * 0.85f, Screen.width * .19f, Screen.height * 0.09f), ">")) {
				targetCameraOffset = Quaternion.Euler(0, +90, 0) * targetCameraOffset;
				targetRotationIncrement = 10;
			}

			/*
			if (GUI.Button (new Rect (Screen.width * .4f, Screen.height * 0.75f, Screen.width * .19f, Screen.height * 0.09f), "^")) 
			{
				Vector2 targetPos = mapgen.tileCursorTarget + new Vector2((cameraOffset.x == 0f) ? 0f : -Mathf.Sign(cameraOffset.x), (cameraOffset.z == 0f) ? 0f :-Mathf.Sign(cameraOffset.z));
				if ( mapgen.validDigLocation(targetPos))
				{
					mapgen.tileCursorTarget = targetPos;
				}
			}
			*/
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



		if (Input.GetMouseButtonDown (0)) 
		{
			mouseButtonDownScreenPos = Input.mousePosition;
			roomScalerGFX.gameObject.SetActive(true);
		}

		roomScalerGFX.transform.position = new Vector3 (mapgen.tileCursorTarget.x + 0.5f, 1.1f, mapgen.tileCursorTarget.y + 0.5f);

		if (Input.GetMouseButton (0)) 
		{
			Vector3 mouseButtonDownScreenPosVector = Input.mousePosition - mouseButtonDownScreenPos;
			mouseButtonDownScreenPosVector.x = Mathf.Abs(mouseButtonDownScreenPosVector.x);
			mouseButtonDownScreenPosVector.y = Mathf.Abs(mouseButtonDownScreenPosVector.y);

			Vector3 widthScaler = new Vector3( Mathf.RoundToInt((mouseButtonDownScreenPosVector.x / Screen.width) * 8), 0, 0);
			Vector3 heightScaler = new Vector3( 0, 0, Mathf.RoundToInt((mouseButtonDownScreenPosVector.y / Screen.height) * 10));

			Vector3[] vertices = new Vector3[]
			{
				new Vector3 (-0.5f, 0, -0.5f),
				new Vector3 (-0.5f, 0, +0.5f),
				new Vector3 (+0.5f, 0, -0.5f),
				new Vector3 (+0.5f, 0, +0.5f)
			};

			vertices[0] -= widthScaler;
			vertices[1] -= widthScaler;

			vertices[2] += widthScaler;
			vertices[3] += widthScaler;

			vertices[1] += heightScaler;

			vertices[3] += heightScaler;

			
			roomScalerGFX.gameObject.transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
			roomScalerGFX.GetComponent<MeshFilter> ().mesh.vertices = vertices;
		} 
		else 
		{
			roomScalerGFX.gameObject.SetActive(false);
		}

		if (Input.GetMouseButtonUp (0)) 
		{
			Vector3 mouseButtonDownScreenPosVector = Input.mousePosition - mouseButtonDownScreenPos;
			mouseButtonDownScreenPosVector.x = Mathf.Abs(mouseButtonDownScreenPosVector.x);
			mouseButtonDownScreenPosVector.y = Mathf.Abs(mouseButtonDownScreenPosVector.y);
			
			Vector3 widthScaler = new Vector3( Mathf.RoundToInt((mouseButtonDownScreenPosVector.x / Screen.width) * 8), 0, 0);
			Vector3 heightScaler = new Vector3( 0, 0, Mathf.RoundToInt((mouseButtonDownScreenPosVector.y / Screen.height) * 10));
			
			Vector3[] vertices = new Vector3[]
			{
				new Vector3 (-0.5f, 0, -0.5f),
				new Vector3 (-0.5f, 0, +0.5f),
				new Vector3 (+0.5f, 0, -0.5f),
				new Vector3 (+0.5f, 0, +0.5f)
			};
			
			vertices[0] -= widthScaler;
			vertices[1] -= widthScaler;
			vertices[2] += widthScaler;
			vertices[3] += widthScaler;
			vertices[1] += heightScaler;
			vertices[3] += heightScaler;
			
			
			roomScalerGFX.gameObject.transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
			roomScalerGFX.GetComponent<MeshFilter> ().mesh.vertices = vertices;

		
			for ( float x =  vertices[0].x; x <=  vertices[2].x; x += 1.0f)
			{
				for ( float z =  vertices[0].z; z <= vertices[1].z; z += 1.0f)
				{

					Vector3 tile = new Vector3(x, 0, z);
					tile = Quaternion.Euler(0, roomScalerGFX.gameObject.transform.rotation.eulerAngles.y, 0) * tile;
					tile.x = (Mathf.Round(tile.x * 10f)/10f) *.95f;
					tile.z = (Mathf.Round(tile.z * 10f)/10f) *.95f;

					mapgen.EditTile (Mathf.Floor(roomScalerGFX.gameObject.transform.position.x + tile.x), Mathf.Floor(roomScalerGFX.gameObject.transform.position.z  + tile.z));
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
