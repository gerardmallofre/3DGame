using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GoblinState { PATROL, CHASE };

public class GoblinHandler : MonoBehaviour, IEnemy
{
    // Start is called before the first frame update
    [SerializeField] MovePlayer moveScript;
    [SerializeField] DeathHandler dieScript;
    [SerializeField] int chaseRange = 4;
    [SerializeField] FallHandler fallScript;
    GameObject player;
    GoblinState state;
    private float falltimer = 0f;
    private GameObject cl;
    [SerializeField] float maxMoveCooldown = 1;
    private float moveCooldown = 0;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (dieScript.getState() == DeathState.ALIVE)
        {
            if (!fallScript.isFalling() && moveScript.getState() == PlayerState.STOP) fallScript.fallCheck();
            if (fallScript.isFalling()) fall();
            else
            {
                moveCooldown -= Time.deltaTime;
                if (moveCooldown < 0)
                {
                    if (state == GoblinState.PATROL && Mathf.Abs(player.transform.position.x - transform.position.x) < chaseRange && Mathf.Abs(player.transform.position.z - transform.position.z) < chaseRange)
                    {
                        state = GoblinState.CHASE;
                    }
                    if (state == GoblinState.CHASE)
                    {
                        moveCooldown = maxMoveCooldown;
                        Direction d = searchPlayer(player.transform.position);
                        //Debug.Log(d);
                        moveScript.tryMove(d);
                    }
                    else    // Patrol
                    {
                        moveCooldown = maxMoveCooldown;
                        bool b = false;
                        int tries = 0;
                        while (!b && tries < 10)
                        {
                            float r = Random.Range(0, 4);
                            if (r < 1) b = moveScript.tryMove(Direction.UP);
                            else if (r < 2) b = moveScript.tryMove(Direction.DOWN);
                            else if (r < 3) b = moveScript.tryMove(Direction.LEFT);
                            else b = moveScript.tryMove(Direction.RIGHT);
                            ++tries;    // Failsafe to avoid weird crashes
                        }
                    }
                }
            }
        }
        else if (dieScript.getState() == DeathState.DEAD) destroy();
    }

    private void fall()
    {
        falltimer += Time.deltaTime;
        transform.localPosition -= new Vector3(0, Time.deltaTime * 10f, 0);
        if (falltimer > 1f) takeDamage(Direction.NONE);
    }

    public void setLevelCreator(GameObject g)
    {
        cl = g;
    }
    public void setPlayer(GameObject p) { player = p; }

    void OnTriggerEnter(Collider other)
    {
        GameObject oobj = other.transform.gameObject;
        if (dieScript.getState() == DeathState.ALIVE && moveScript.getState() == PlayerState.MOVE
            && oobj.tag != "Coin" && oobj.tag != "Ground" && oobj.tag != "SlimeTile")
        {
            DeathHandler ds = other.GetComponent<DeathHandler>() ?? other.GetComponentInParent<DeathHandler>();
            if (ds != null && ds.getState() == DeathState.ALIVE)
            {
                moveScript.undoMove();
                PlayerHandler p = oobj.GetComponent<PlayerHandler>() ?? oobj.GetComponentInParent<PlayerHandler>();
                if (p != null)
                {
                    MovePlayer pmv = p.GetComponent<MovePlayer>();
                    if (pmv.getState() != PlayerState.MOVE || movingTowardsPlayer(p.gameObject))
                        p.takeDamage(1, moveScript.getDir());
                }
            }
        }
    }

    bool movingTowardsPlayer(GameObject player)
    {
        float minx, maxx, minz, maxz;

        if (moveScript.getDir() == Direction.UP)
        {
            minx = transform.position.x - 0.5f;
            maxx = minx + 1f;
            minz = transform.position.z;
            maxz = minz + 1.5f;
        }
        else if (moveScript.getDir() == Direction.DOWN)
        {
            minx = transform.position.x - 0.5f;
            maxx = minx + 1f;
            maxz = transform.position.z;
            minz = maxz - 1.5f;
        }
        else if (moveScript.getDir() == Direction.RIGHT)
        {
            minx = transform.position.x;
            maxx = minx + 1.5f;
            minz = transform.position.z - 0.5f;
            maxz = minz + 1f;
        }
        else
        {
            maxx = transform.position.x;
            minx = maxx - 1.5f;
            minz = transform.position.z - 0.5f;
            maxz = minz + 1f;
        }

        Vector3 playerpos = player.transform.position;
        return (playerpos.x <= maxx && playerpos.x >= minx) && (playerpos.z <= maxz && playerpos.z >= minz);
    }

    public void takeDamage(Direction d)
    {
        if (dieScript.getState()==DeathState.ALIVE) dieScript.startDeath(d);
    }

    public void destroy()
    {
        if (cl != null) cl.GetComponent<CreateLevel>().enemyKilled();
        Destroy(this.transform.gameObject);
    }

    struct item
    {
        public Vector3 pos;
        public Direction initialDir;
    }
    Direction searchPlayer(Vector3 targetpos)
    {
        Queue<item> q = new Queue<item>();
        item i;
        i.pos = transform.position;
        i.initialDir = Direction.NONE;
        q.Enqueue(i);
        List<Vector3> visited = new List<Vector3>();
        visited.Add(i.pos);

        //int DEBUGCOUNT = 0;

        while (q.Count != 0)
        {
            //++DEBUGCOUNT;
            i = q.Dequeue();
            //Debug.Log(i.pos + " " + i.initialDir);
            if (Mathf.Abs(i.pos.x - targetpos.x) < 1 && Mathf.Abs(i.pos.z - targetpos.z) < 1) return i.initialDir;

            //Debug.Log("x+ ground: " + CheckForGround(i.pos + new Vector3(0, 0, -1)) != null);

            if (!visited.Contains(i.pos + new Vector3(1, 0, 0)) && checkWall(i.pos, new Vector3(1, 0, 0), 0f, 0.7f) == null && CheckForGround(i.pos + new Vector3(1, 0, 0)) != null)
            {
                item newit;
                newit.pos = i.pos + new Vector3(1, 0, 0);
                if (i.initialDir == Direction.NONE) newit.initialDir = Direction.RIGHT;
                else newit.initialDir = i.initialDir;
                q.Enqueue(newit);
                visited.Add(newit.pos);
                //Debug.Log("x+");
            }
            if (!visited.Contains(i.pos + new Vector3(-1, 0, 0)) && checkWall(i.pos, new Vector3(-1, 0, 0), 0f, 0.7f) == null && CheckForGround(i.pos + new Vector3(-1, 0, 0)) != null)
            {
                item newit;
                newit.pos = i.pos + new Vector3(-1, 0, 0);
                if (i.initialDir == Direction.NONE) newit.initialDir = Direction.LEFT;
                else newit.initialDir = i.initialDir;
                q.Enqueue(newit);
                visited.Add(newit.pos);
                //Debug.Log("x-");
            }
            if (!visited.Contains(i.pos + new Vector3(0, 0, 1)) && checkWall(i.pos, new Vector3(0, 0, 1), 0f, 0.7f) == null && CheckForGround(i.pos + new Vector3(0, 0, 1)) != null)
            {
                item newit;
                newit.pos = i.pos + new Vector3(0, 0, 1);
                if (i.initialDir == Direction.NONE) newit.initialDir = Direction.UP;
                else newit.initialDir = i.initialDir;
                q.Enqueue(newit);
                visited.Add(newit.pos);
                //Debug.Log("z+");
            }
            if (!visited.Contains(i.pos + new Vector3(0, 0, -1)) && checkWall(i.pos, new Vector3(0, 0, -1), 0f, 0.7f) == null && CheckForGround(i.pos + new Vector3(0, 0, -1)) != null)
            {
                item newit;
                newit.pos = i.pos + new Vector3(0, 0, -1);
                if (i.initialDir == Direction.NONE) newit.initialDir = Direction.DOWN;
                else newit.initialDir = i.initialDir;
                q.Enqueue(newit);
                visited.Add(newit.pos);
                //Debug.Log("z-");
            }
        }
        //Debug.Log(DEBUGCOUNT);
        return Direction.NONE;
    }

    private GameObject checkWall(Vector3 P, Vector3 v, float min, float max)
    {
        float closestDistance = max + 1.0f;
        GameObject obj = null;

        // Physics.RaycastAll returns all colliders in a given ray (P, v) within a given distance (max)
        RaycastHit[] hits = Physics.RaycastAll(P, v, max);
        foreach (RaycastHit hit in hits)
        {
            if ((hit.distance > min) && (hit.distance < max) && (hit.collider.gameObject.tag == "Wall" || hit.collider.gameObject.tag=="Hole" || (hit.collider.gameObject.tag == "Door" && !hit.collider.gameObject.GetComponent<DoorHandler>().isOpen())))
                if (hit.distance < closestDistance)
                {
                    closestDistance = hit.distance;
                    obj = hit.collider.gameObject;
                }
        }

        return obj;
    }

    private GameObject CheckForGround(Vector3 P)
    {
        float min = 0f; float max = 1.5f; Vector3 v = new Vector3(0, -1, 0);
        P += new Vector3(0, 0.5f, 0);
        float closestDistance = max + 1.0f;
        GameObject obj = null;

        // Physics.RaycastAll returns all colliders in a given ray (P, v) within a given distance (max)
        RaycastHit[] hits = Physics.RaycastAll(P, v, max);
        foreach (RaycastHit hit in hits)
        {
            if ((hit.distance > min) && (hit.distance < max) && (hit.collider.gameObject.tag == "Ground"))
                if (hit.distance < closestDistance)
                {
                    closestDistance = hit.distance;
                    obj = hit.collider.gameObject;
                }
        }

        return obj;
    }
}
