using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;



public class BlobPowerup : MonoBehaviour
{

    public struct Hole2D
    {
        public Vector3Int cellLocation;
        public TileBase savedTile;
        public Hole2D(Vector3Int cellLocation, TileBase savedTile)
        {
            this.cellLocation = cellLocation;
            this.savedTile = savedTile;
        }


        // Override ToString for easy debugging
        public override string ToString()
        {
            return $"({cellLocation}, {savedTile})";
        }
    }

    private BlobPath blobPathScript;
    public GameObject Player;
    public Tilemap tileMap;
    private TilemapCollider2D tileMapCollider;
    private Hole2D currentHole;
    public GameObject tempTarget;

    void Start()
    {
        blobPathScript = GetComponent<BlobPath>();

        if (tileMap == null)
        {
            Debug.LogError("no TileMap assigned to" + this);
        }

        tileMapCollider = tileMap.GetComponent<TilemapCollider2D>();
       
        if (tileMapCollider == null)
        {
            Debug.LogError("TileMapCollider2D is missing on " + this);
        }
    }

    void Update()
    {



        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            UnsummonHole();
            GameObject newTarget = new GameObject("Jellybean");
            newTarget.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CallChase(newTarget);

        }

        if (tempTarget != null)
        {
            if (Vector2.Distance(transform.position, tempTarget.transform.position) < 1.0f)
            {
                print("success!");
                UnsummonHole();
                SummonHole(tempTarget.transform.position + Vector3.down);
            }
            if (blobPathScript.target == blobPathScript.Player.transform) { UnsummonHole(); tempTarget = null; print("trigger"); }

        }
    }

    void CallChase(GameObject target)
    {

        Destroy(tempTarget);
        blobPathScript.target = target.transform; 

        tempTarget = target;
    }

    void SummonHole(Vector3 worldPos)
    {
        GetComponent<Rigidbody2D>().simulated = false;
        GetComponent<SpriteRenderer>().enabled = false;


        Vector3Int cellPosition = tileMap.WorldToCell(worldPos);

        if (tileMap.HasTile(cellPosition) && !tileMap.HasTile(cellPosition + Vector3Int.down))
        {
           

            currentHole = new Hole2D(cellPosition, tileMap.GetTile(cellPosition));


            tileMap.SetTile(cellPosition, null);



            RefreshTileMapCollider();
        }
    }

    void UnsummonHole() {
        GetComponent<Rigidbody2D>().simulated = true;
        GetComponent<SpriteRenderer>().enabled = true;

        if (currentHole.cellLocation != null && currentHole.savedTile != null)
        {
            tileMap.SetTile(currentHole.cellLocation, currentHole.savedTile);

            RefreshTileMapCollider();
        }
    }

    void RefreshTileMapCollider()
    {// Refresh the collider by disabling and enabling the TilemapCollider2D
        tileMapCollider.enabled = false;
        tileMapCollider.enabled = true;
    }
}