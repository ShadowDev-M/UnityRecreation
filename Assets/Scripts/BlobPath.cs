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
    
    public float speed = 3f; // Movement speed
    private Path path;
    private int currentWaypoint = 0;
    private bool isGrounded = false;
    private bool isFlying = false;
    private Seeker seeker;
    private Rigidbody2D rb;
    private Vector2 lastPosition;
    private float stuckTimer = 0f;
    private Collider2D blobCollider;

    void Start()
    {
        if (blobCollider == null) blobCollider = GetComponent<Collider2D>();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        InvokeRepeating("UpdatePath", 0f, 0.5f); // Update path periodically
        lastPosition = rb.position;

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


        if (Vector2.Distance(rb.position, target.position) > 5) { DetectStuck(); }


        
        
        if (isFlying)
        {
            BalloonMode();
            return;
            
        }
        
        // checking if touching ground
        isGrounded = Physics2D.OverlapCircle(transform.position, 0.3f, groundLayer);

        // If close to the current waypoint, move to the next one
        if (currentWaypoint >= path.vectorPath.Count) return;
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
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
                
                
                if(rb.velocity.x != 0)
                {
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

        // will stop flying when near target
        if (Vector2.Distance(rb.position, target.position) < 0.2)
        {
            blobCollider.enabled = true;
            rb.velocity = Vector2.zero;
            isFlying = false;
            
                Jump();
            
            stuckTimer = 0f;
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
}