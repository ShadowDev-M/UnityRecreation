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
    public GameObject holeJellybeanTarget;
    public GameObject ladderJellybeanTarget;
    public GameObject ladderPrefab;
    private bool blobTransformed;

    private Animator blobAnimator;

    void Start()
    {
        blobAnimator = GetComponent<Animator>();

        blobTransformed = false;
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
            UnsummonLadder();

            GameObject newTarget = new GameObject("Jellybean");
            newTarget.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CallChase(newTarget, 0);

        }
        if (Input.GetMouseButtonDown(1)) // Left mouse button
        {
            UnsummonLadder();
            UnsummonHole();

            GameObject newTarget = new GameObject("Jellybean");
            newTarget.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CallChase(newTarget, 1);

        }

        if (holeJellybeanTarget != null && blobTransformed == false)
        {
            if (Vector2.Distance(transform.position, holeJellybeanTarget.transform.position) < 0.25f)
            {
               // print("success!");
               
                
                SummonHole(holeJellybeanTarget.transform.position + Vector3.down);
            }
           // if (blobPathScript.target == blobPathScript.Player.transform) { UnsummonHole(); UnsummonLadder(); holeJellybeanTarget = null; }

        }

        if (ladderJellybeanTarget != null && blobTransformed == false)
        {
            if (Vector2.Distance(transform.position, ladderJellybeanTarget.transform.position) < 1.0f)
            {
                print("success!");
                


                SummonLadder(ladderJellybeanTarget.transform.position);
            }
           //if (blobPathScript.target == blobPathScript.Player.transform) { UnsummonLadder(); UnsummonHole(); ladderJellybeanTarget = null; }

        }
        if (blobPathScript.target == blobPathScript.Player.transform) { UnsummonLadder(); UnsummonHole(); ladderJellybeanTarget = null; holeJellybeanTarget = null; }


    }

    void CallChase(GameObject target, int beanType)
    {

        Destroy(holeJellybeanTarget);
        Destroy(ladderJellybeanTarget);

        blobPathScript.target = target.transform; 

        if (beanType == 0) holeJellybeanTarget = target;
        else if (beanType == 1) ladderJellybeanTarget = target;
    }


    void SummonLadder(Vector3 worldPos) {
        blobTransformed = true;

        GetComponent<Rigidbody2D>().simulated = false;
        GetComponent<SpriteRenderer>().enabled = false;

        Vector3Int cellPosition = tileMap.WorldToCell(worldPos);

        Vector3 cellWorldPos = tileMap.CellToWorld(cellPosition);

        Instantiate(ladderPrefab, cellWorldPos + new Vector3(0.0f,ladderPrefab.transform.localScale.y / 2, 0.0f), Quaternion.identity);
    }

    void UnsummonLadder()
    {
        blobTransformed = false;

        GetComponent<Rigidbody2D>().simulated = true;
        GetComponent<SpriteRenderer>().enabled = true;

        GameObject[] allLadders = GameObject.FindGameObjectsWithTag("Ladder");
        foreach (GameObject go in allLadders)
            Destroy(go.transform.parent.gameObject);
    }




    void SummonHole(Vector3 worldPos)
    {
     
        Vector3Int cellPosition = tileMap.WorldToCell(worldPos);

        if (tileMap.HasTile(cellPosition) && !tileMap.HasTile(cellPosition + Vector3Int.down))
        {
           

            currentHole = new Hole2D(cellPosition, tileMap.GetTile(cellPosition));


            tileMap.SetTile(cellPosition, null);



            RefreshTileMapCollider();            

            GetComponent<Rigidbody2D>().simulated = false;
            //GetComponent<SpriteRenderer>().enabled = false;

            blobAnimator.SetBool("BlobHole", true);
            transform.position = tileMap.CellToWorld(cellPosition) + new Vector3(0.5f, 0.9f, 0);

            blobTransformed = true;

        }
    }

    void UnsummonHole() {
        blobTransformed = false;

        GetComponent<Rigidbody2D>().simulated = true;
        //GetComponent<SpriteRenderer>().enabled = true;
        blobAnimator.SetBool("BlobHole", false);


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