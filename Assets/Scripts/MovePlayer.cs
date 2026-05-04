using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// The player will alternate between two distinct states, either stopped
// or executing a movement from one tile to an adjacent one.
public enum PlayerState { STOP, MOVE, UNDO };

// The player can move in four directions. Here they are listed in 
// clockwise order, so that taking differences of directions produces
// (multiplied by 90 degrees) the rotation needed to reorient the player
// from one direction to another (e.g. changing from DOWN to RIGHT requires
// a 90 * (RIGHT - DOWN) = -90 degree rotation.)
public enum Direction { UP = 0, RIGHT, DOWN, LEFT };


public class MovePlayer : MonoBehaviour
{
    public float speed = 1.0f;              // Speed of movement
    public float heightJump = 0.5f;         // Height of the jump during a move
    public AudioClip jumpSound;  // Sound for moving and pushing a box
    //[SerializeField] bool isPlayer=false;

    PlayerState state;               // Current player state
    Direction dir;                   // Current direction the player is facing
    Vector3 initialPosMove, vecMove; // For a movement, initial location and vector of movement
    float timeInMove;                // Time in a movement currently taking place
                                     // It should take values from 0 to 1/speed, for tiles of size 1

    // Start is called before the first frame update
    void Start()
    {
        // The player starts stopped and looking down.
        state = PlayerState.STOP;
        dir = Direction.DOWN;
    }

    // Suggestions:
    //   Check victory
    //   Better animation for player movement (compress vertically, jump with stretching at highest point, decompress on impact)
    //   More levels, audio, animations, effects
    //   Main menu

    // Update is called once per frame
    void Update()
    {
        if (state != PlayerState.STOP)
        {
            UpdateMovement();
        }
    }

    public void tryMove(Direction dirMove)
    {
        // We need behaviour for the two player states. If it is MOVE, we call UpdateMovement.
        // For STOP, we check if any of the arrow keys is pressed, and we check and prepare the
        // corresponding movement via the method PrepareMovement.
        if (state == PlayerState.STOP)
        {
            PrepareMovement(dirMove);
        }
    }

    // This method checks that the movement the player wants to make is valid and
    // initializes the necessary object attributes.
    private bool PrepareMovement(Direction dirMove)
    {
        // bMove will track if the player can really move, as walls and boxes may stop him.
        bool bMove = true;
        // angleMove is the rotation required to transform the Z axis into the direction 
        // encoded by dirMove, the direction of the movement.
        float angleMove = Mathf.PI * (int)dirMove / 2.0f;

        // Save the initial position of the player before moving
        initialPosMove = transform.localPosition;
        // From angleMove we compute the vector of the movement's displacement
        vecMove = new Vector3(Mathf.Sin(angleMove), 0.0f, Mathf.Cos(angleMove));

        // We look for a object tagged as "Wall" next to the player in the movement's direction 
        // If present, we cannot move
        GameObject obj = GetObjectInDirection(initialPosMove, vecMove, 0.0f, 1.0f);
        if (obj != null)
            bMove = false;
        if (bMove)
        {
            // Now that we know we will move, we initalize all that we need, and rotate
            // the player to face the direction of movement
            state = PlayerState.MOVE;
            timeInMove = 0;
            transform.Rotate(0.0f, 90.0f * ((int)dirMove - (int)dir), 0.0f);
            dir = dirMove;

            // We also play the corresponding sound, and add a sound for the box if one is being pushed.
            AudioSource.PlayClipAtPoint(jumpSound, Camera.main.transform.position);
        }

        return bMove;
    }

    // This method checks if an object with a collider is present looking from point P
    // in the direction v, at a distance between min and max. If so, it returns the closest one.
    private GameObject GetObjectInDirection(Vector3 P, Vector3 v, float min, float max)
    {
        float closestDistance = max + 1.0f;
        GameObject obj = null;

        // Physics.RaycastAll returns all colliders in a given ray (P, v) within a given distance (max)
        RaycastHit[] hits = Physics.RaycastAll(P, v, max);
        foreach (RaycastHit hit in hits)
        {
            if ((hit.distance > min) && (hit.distance < max) && (hit.collider.gameObject.tag == "Wall" || (hit.collider.gameObject.tag == "Door" && !hit.collider.gameObject.GetComponent<DoorHandler>().isOpen())))
                if (hit.distance < closestDistance)
                {
                    closestDistance = hit.distance;
                    obj = hit.collider.gameObject;
                }
        }

        return obj;
    }

    // This method plays the movement of the player and the box being pushed if there is one.
    private void UpdateMovement()
    {
        // Update time. Once we get to 1/speed the movement needs to end at the destination tile.
        timeInMove += Time.deltaTime;
        if (timeInMove > 1.0f / speed)
        {
            // End movement. Final position is initial one plus the vector of movement multiplied
            // by the size of one tile (in this case, 1).
            transform.localPosition = initialPosMove + vecMove;
            state = PlayerState.STOP;
        }
        else
        {
            // The movement includes a jump computed as the first half of the period of a sine function.
            Vector3 jumpMove = heightJump * Mathf.Sin(speed * timeInMove * Mathf.PI) * Vector3.up;
            // The movement is then the initial position plus the horizontal displacement plus the jump.
            transform.localPosition = initialPosMove + speed * timeInMove * vecMove + jumpMove;
        }

    }

    public PlayerState getState() { return state; }

    public void undoMove()
    {
        if (state == PlayerState.MOVE)
        {
            state = PlayerState.UNDO;
            initialPosMove = initialPosMove + vecMove;
            timeInMove = 1.0f / speed - timeInMove;
            vecMove = vecMove * -1;
        }
    }

    public void stopMove()
    {
        state = PlayerState.STOP;
        transform.localPosition = initialPosMove;
    }
}
