using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using System.Threading;
using System.IO;





public class IntVector2 {
	public int x, y;
	public IntVector2(int x, int y) {
		this.x = x;
		this.y = y;
	}
}


public static class TileMapConstants
{
	public const int kLevel0 = 0;
}




public class MapGen : MonoBehaviour {

	public GameObject tileGFXPrefab;

	public GameObject wallPrefab;


	public GameObject polygonTilePrefab;


	public class TileGraphSerilizable {
		public IntVector2 tileMapDim  ;
		public Dictionary<string, Tile> tileGraph = null;

		public bool ShouldSerializetileGraphAt() { return false; }
		public Tile tileGraphAt(Vector2 pos) 
		{
			string graphKey = Tile.MakeGraphKey ( pos );
			return tileGraph [graphKey];
		}

	}

	TileGraphSerilizable tgs;


	public Vector2 tileCursor = new Vector2(25, 25);
	public Vector2 tileCursorTarget = new Vector2(25, 25);




	// Use this for initialization
	void Start () {

		tgs = new TileGraphSerilizable ();



		CreateTileGraph ();

		BuildWallsGfx ();

	}

	
	// Update is called once per frame
	void Update() {


		GameObject tileCursorGFX = GameObject.Find("TileCursor");
		if (tileCursorGFX != null)
		{
			tileCursorGFX.transform.position = new Vector3( tileCursor.x + 0.5f, 0.5f, tileCursor.y + 0.5f);
		}
		if (tileCursor != tileCursorTarget) 
		{
			if (tileCursor.x != tileCursorTarget.x)
			{
				tileCursor.x  += (tileCursor.x > tileCursorTarget.x) ? -0.2f : 0.2f;
				tileCursor.x = Mathf.Round(tileCursor.x *100f)/100f;
			}

			if (tileCursor.y != tileCursorTarget.y)
			{
				tileCursor.y  += (tileCursor.y > tileCursorTarget.y) ? -0.2f : 0.2f;
				tileCursor.y = Mathf.Round(tileCursor.y * 100f)/100f;
			}
		}

	

	}

	void OnGUI() {



	}



	public void EditTile(float x, float y) 
	{
		Debug.Log ("EditTile:" + x + "," + y);
		if ( tgs != null && tgs.tileGraph.ContainsKey( Tile.MakeGraphKey( new Vector2(x, y)) ) == true)
		{

			tgs.tileGraphAt( new Vector2(x, y) ).EditTile(  new Vector2(x, y),  tgs.tileGraph,  tileGFXPrefab, wallPrefab, this.transform);
		}
	}


	public bool validDigLocation( Vector2 location)
	{

		if ( tgs != null && tgs.tileGraph.ContainsKey( Tile.MakeGraphKey( new Vector2(location.x, location.y)) ) == true)
		{

			return tgs.tileGraphAt( new Vector2(location.x, location.y) ).validDigLocation(  location,  tgs.tileGraph);
		}
		return false;
	}






	private void CreateTileGraph() 
	{
		BuildTileGraph();

		BuildTileRectangularAdjacencyGraph();


		BuildElevatedTiles();
		BuildElevatedTilesNavGraph ();
	}

	private void BuildTileGraph() 
	{
		int maxX = 60;
		int maxY = 60;

		
		tgs.tileMapDim = new IntVector2( maxX + 1, maxY + 1);

		tgs.tileGraph = new Dictionary<string, Tile> ();

		for (int x = 0; x < tgs.tileMapDim.x; x++) {
			for (int y = 0; y < tgs.tileMapDim.y; y++) {

				tgs.tileGraph.Add ( Tile.MakeGraphKey( new Vector2(x, y)), new Tile() );

				Tile thisGraphTile = tgs.tileGraphAt( new Vector2(x, y));

				thisGraphTile.pos.x = x;
				thisGraphTile.pos.y = y;
			}
		}
	}


	private void BuildTileRectangularAdjacencyGraph() 
	{
		foreach(KeyValuePair<string, Tile> tileGraphNodeKVP in tgs.tileGraph)
		{
			Tile thisGraphTile =  tileGraphNodeKVP.Value;
				
			string adjGraphKey = Tile.MakeGraphKey( thisGraphTile.pos - new Vector2(1.0f, 0.0f));
			if ( tgs.tileGraph.ContainsKey( adjGraphKey ) == true ) 
			{
				tileGraphNodeKVP.Value.adjlinks.Add( adjGraphKey, true );
			} 

			adjGraphKey = Tile.MakeGraphKey( thisGraphTile.pos + new Vector2(1.0f, 0.0f));
			if ( tgs.tileGraph.ContainsKey( adjGraphKey ) == true ) 
			{
				tileGraphNodeKVP.Value.adjlinks.Add( adjGraphKey, true );
			} 

			adjGraphKey = Tile.MakeGraphKey( thisGraphTile.pos - new Vector2(0.0f, 1.0f));
			if ( tgs.tileGraph.ContainsKey( adjGraphKey ) == true ) 
			{
				tileGraphNodeKVP.Value.adjlinks.Add( adjGraphKey, true );
			} 

			adjGraphKey = Tile.MakeGraphKey( thisGraphTile.pos + new Vector2(0.0f, 1.0f));
			if ( tgs.tileGraph.ContainsKey( adjGraphKey ) == true ) 
			{
				tileGraphNodeKVP.Value.adjlinks.Add( adjGraphKey, true );
			} 
		}
	}





	
	private void BuildElevatedTiles() 
	{
		//any tile which doesn't yet have a GFX is now assigned to be an elevated tile.
		foreach(KeyValuePair<string, Tile> tileGraphNodeKVP in tgs.tileGraph)
		{
			Tile thisGraphTile =  tileGraphNodeKVP.Value;
			
			if ( thisGraphTile.gfx == null) 
			{
				if (tileCursor == thisGraphTile.pos)
				{
					thisGraphTile.Init( thisGraphTile.pos, this.transform, TileMapConstants.kLevel0 + 1, Instantiate(tileGFXPrefab));
				}
				else
				{
					thisGraphTile.Init( thisGraphTile.pos, this.transform, TileMapConstants.kLevel0 + 1, Instantiate(tileGFXPrefab));
				}
			}
		}
	}


	private void BuildElevatedTilesNavGraph()
	{
		foreach(KeyValuePair<string, Tile> tileGraphNodeKVP in tgs.tileGraph)
		{
			Tile thisGraphTile =  tileGraphNodeKVP.Value;

			if ( thisGraphTile.elevation > 0 ) // 'blocked' tile
			{
				foreach(KeyValuePair<string, bool> tileAdjacencyNodeKVP in thisGraphTile.adjlinks)
				{
					if ( tgs.tileGraph[ tileAdjacencyNodeKVP.Key ].elevation == thisGraphTile.elevation )
					{
						thisGraphTile.navlinks.Add( tileAdjacencyNodeKVP.Key, new NavLink(true) );
					}
				}
			}
		}

	}






	private void BuildWallsGfx() 
	{
		foreach(KeyValuePair<string, Tile> tileGraphNodeKVP in tgs.tileGraph)
		{
			tileGraphNodeKVP.Value.BuildWallGfx( tgs.tileGraph, wallPrefab, this.transform );
		}
	}




					
















}
