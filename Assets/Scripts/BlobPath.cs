using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding; // Required for A* Pathfinding

[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(Rigidbody2D))]
public class BlobPath : MonoBehaviour
{
    public Transform target; // The target the AI should move towards
    public float maxSpeed = 3f; // Movement speed cap
    public float jumpForce = 5f; // Force applied when jumping
    public float nextWaypointDistance = 1f; // Distance to consider waypoint reached
    public float gapDetectionDistance = 1.5f; // Distance to detect gaps
    public LayerMask groundLayer; // Layer for ground detection
    public float stuckDetectionTime = 2f; // Time to detect if the AI is stuck
    public float stuckThreshold = 0.1f; // Threshold to detect if the AI isn't moving
    public float speedBoost = 0.0f;
    public GameObject Player;
    
    public float speed = 3f; // Movement speed
    private Path path;
    private int currentWaypoint = 0;
    public bool isGrounded = false;
    private bool isFlying = false;
    private Seeker seeker;
    private Rigidbody2D rb;
    private Vector2 lastPosition;
    private float stuckTimer = 0f;
    private Collider2D blobCollider;

    private float lastTime;
    private Animator blobAnimtor;


    void Start()
    {
        if (blobCollider == null) blobCollider = GetComponent<Collider2D>();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        InvokeRepeating("UpdatePath", 0f, 0.5f); // Update path periodically
        lastPosition = rb.position;

        blobAnimtor = GetComponent<Animator>();
    }

    void UpdatePath()
    {
        if (seeker.IsDone() && target != null)
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    void FixedUpdate()
    {
        

        if (speedBoost > 0) { speedBoost -= Time.fixedDeltaTime*2; } else { speedBoost = 0; }

        speed = maxSpeed + speedBoost;
      
        if (path == null || target == null) return;


        if (path != null && PathObstruction() && target == Player.transform)
        { DetectStuck(); 
        }
        


        if (Input.GetKeyDown(KeyCode.E))
        {
            target = Player.transform;


        }

        if (!Input.GetKey(KeyCode.E)) lastTime = Time.time;
        else if (Time.time - lastTime >= 1)
        {
            target = Player.transform;

            DetectStuck();
        }


            if (isFlying)
        {
            BalloonMode();
            return;
            
        }
        
        // checking if touching ground
        isGrounded = Physics2D.OverlapCircle(transform.position, 0.3f, groundLayer);

        blobAnimtor.SetBool("blobHop", !isGrounded);

        // If close to the current waypoint, move to the next one
        if (currentWaypoint >= path.vectorPath.Count) return;
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance ||  ((target == Player.transform) && distance < nextWaypointDistance * 4))
        {
            
            
            currentWaypoint++;
            return;
        }

        // direction to target
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;


        // checking for gap and if blob wants to move before jumping 
        if (DetectGap() && isGrounded && rb.velocity.x != 0)
        {
            Jump();

        }
        else
        {

            if (isGrounded)
            {
                

                // Horizontal movement
                Vector2 force = new Vector2(direction.x * (speed + speedBoost), rb.velocity.y);
                rb.velocity = force;

                // Flip sprite based on movement direction

                if (force.x > 0.01f)
                    transform.localScale = new Vector3(1, 1, 1);
                else if (force.x < -0.01f)
                    transform.localScale = new Vector3(-1, 1, 1);


                if (rb.velocity.x != 0)
                {
                    if (distance > 1)
                    Hop();

                }
            }
        }
       
    }

    bool DetectGap()
    {
        // Cast a ray downward in front of the AI to detect gaps
        Vector2 rayOrigin = new Vector2(transform.position.x + (transform.localScale.x > 0 ? gapDetectionDistance : -gapDetectionDistance), transform.position.y);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 2f, groundLayer);

        Debug.DrawRay(rayOrigin, Vector2.down * 2f, Color.red); // Debug line for the raycast

        return hit.collider == null; // True if no ground is detected
    }

    void Hop() {
        if (isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, Random.Range(1.3f, 2.1f));
            if (Vector2.Distance(transform.position, Player.transform.position) < 1.2) rb.velocity = new Vector2(rb.velocity.x, Random.Range(1.3f, 6.1f));



            // blobAnimtor.SetBool("blobHop", !isGrounded);
        }
    }

    void Jump()
    {
        if (isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            speedBoost = 2.0f;
        }
    }

    void DetectStuck()
    {
        if (!isFlying) { rb.velocity = Vector2.zero; }

        float distanceMoved = Vector2.Distance(rb.position, lastPosition);
        lastPosition = rb.position;

        if (distanceMoved < stuckThreshold)
        {
            stuckTimer += Time.fixedDeltaTime;
        }
        else
        {
            stuckTimer = 0f;
        }

        if (stuckTimer >= stuckDetectionTime)
        {
            isFlying = true; // Switch to flying mode if stuck
        }
    }

    void BalloonMode()
    {
        Vector2 direction = ((Vector2)target.position - rb.position).normalized;
        rb.velocity = direction * speed*2;
        blobCollider.enabled = false;

        blobAnimtor.SetBool("Balloon", true);

        // will stop flying when near target
        if (Vector2.Distance(rb.position, target.position) < 0.2)
        {
            blobCollider.enabled = true;
            rb.velocity = Vector2.zero;
            isFlying = false;
            
                Jump();
            
            stuckTimer = 0f;

            blobAnimtor.SetBool("Balloon", false);
        }
    }

    void OnDrawGizmos()
    {
        if (path == null) return;

        // for seeing path in the editor
        for (int i = currentWaypoint; i < path.vectorPath.Count; i++)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(path.vectorPath[i], 0.2f);

            if (i == currentWaypoint)
            {
                Gizmos.DrawLine(transform.position, path.vectorPath[i]);
            }
            else
            {
                Gizmos.DrawLine(path.vectorPath[i - 1], path.vectorPath[i]);
            }
        }
    }

    bool PathObstruction()
    {
        if (path == null || path.vectorPath == null || path.vectorPath.Count < 2) return true;

        for (int i = currentWaypoint; i < path.vectorPath.Count - 1; i++)
        {
            Vector2 start = path.vectorPath[i];
            Vector2 end = path.vectorPath[i + 1];

            

           /* float heightDifference = end.y - start.y;
            if (heightDifference > jumpForce)  
            {
                print("To High");

                return true; 
            }*/

            // Collision in the Path
            RaycastHit2D hit = Physics2D.Linecast(start, end, groundLayer);
            if (hit.collider != null)
            {
                Debug.DrawLine(start, end, Color.red); // debug line for editor
                print("Obstructed Wall");
                return true;
            }
        }


        //path clear
        return false; 
    }
}