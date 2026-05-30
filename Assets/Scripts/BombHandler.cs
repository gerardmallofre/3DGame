using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum BombState { PATROL, CHASE, WAIT};

public class BombHandler : MonoBehaviour, IEnemy
{
    [SerializeField] MovePlayer moveScript;
    [SerializeField] DeathHandler dieScript;
    [SerializeField] FallHandler fallScript;
    [SerializeField] int chaseRange = 4;
    [SerializeField] int explodeRange = 2;
    [SerializeField] float explosionRange = 3f;
    [SerializeField] GameObject explosion;
    GameObject cl;
    float falltimer = 0f;
    GameObject player;
    BombState state = BombState.PATROL;
    [SerializeField] float maxMoveCooldown = 1f;
    [SerializeField] float maxExplodeDelay = 1f;
    float moveCooldown=0f;
    float explodeDelay=0f;
    // Start is called before the first frame update
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
                if (state == BombState.WAIT)
                {
                    explodeDelay -= Time.deltaTime;
                    if (explodeDelay < 0)
                    {
                        explode();
                    }
                }
                else if (state == BombState.CHASE && Mathf.Abs(player.transform.position.x - transform.position.x) < explodeRange && Mathf.Abs(player.transform.position.z - transform.position.z) < explodeRange)
                {
                    state = BombState.WAIT;
                    explodeDelay = maxExplodeDelay;
                    GetComponent<HitEffect>().PlayHitEffect(maxExplodeDelay);
                }
                else
                {
                    moveCooldown -= Time.deltaTime;
                    if (state != BombState.WAIT && moveCooldown < 0)
                    {
                        if (state == BombState.PATROL && Mathf.Abs(player.transform.position.x - transform.position.x) < chaseRange && Mathf.Abs(player.transform.position.z - transform.position.z) < chaseRange)
                        {
                            state = BombState.CHASE;
                        }
                        if (state == BombState.CHASE)
                        {
                            moveCooldown = maxMoveCooldown / 2;
                            moveScript.tryMove(searchPlayer(player.transform.position));
                        }
                        else    // Patrol
                        {
                            moveCooldown = maxMoveCooldown;
                            bool b = false;
                            int tries = 0;
                            while (!b && tries<10)
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
        }
        else if (dieScript.getState() == DeathState.DEAD) destroy();
    }

    public void setPlayer(GameObject p) { player = p; }

    private void fall()
    {
        falltimer += Time.deltaTime;
        transform.localPosition -= new Vector3(0, Time.deltaTime * 10f, 0);
        if (falltimer > 1f) takeDamage(Direction.NONE);
    }

    void destroy()
    {
        if (cl != null) cl.GetComponent<CreateLevel>().enemyKilled();
        Destroy(this.transform.gameObject);
    }

    public void takeDamage(Direction d) { if (dieScript.getState() == DeathState.ALIVE) dieScript.startDeath(d); }

    public void setLevelCreator(GameObject g) { cl = g; }

    struct item
    {
        public Vector3 pos;
        public Direction initialDir;
    }
    Direction searchPlayer(Vector3 targetpos)
    {
        Queue<item> q=new Queue<item>();
        item i;
        i.pos = transform.position;
        i.initialDir = Direction.NONE;
        q.Enqueue(i);
        List<Vector3> visited = new List<Vector3>();
        visited.Add(i.pos);

        while (q.Count!=0)
        {
            i = q.Dequeue();
            if (Mathf.Abs(i.pos.x - targetpos.x) < 1 && Mathf.Abs(i.pos.z - targetpos.z) < 1) return i.initialDir;
            if (!visited.Contains(i.pos + new Vector3(1, 0, 0)) && checkWall(i.pos, new Vector3(1, 0, 0), 0f, 1f) == null && CheckForGround(i.pos + new Vector3(1, 0, 0)) != null)
            {
                item newit;
                newit.pos = i.pos + new Vector3(1, 0, 0);
                if (i.initialDir == Direction.NONE) newit.initialDir = Direction.RIGHT;
                else newit.initialDir = i.initialDir;
                q.Enqueue(newit);
                visited.Add(newit.pos);
            }
            if (!visited.Contains(i.pos + new Vector3(-1, 0, 0)) && checkWall(i.pos, new Vector3(-1, 0, 0), 0f, 1f) == null && CheckForGround(i.pos + new Vector3(-1, 0, 0)) != null)
            {
                item newit;
                newit.pos = i.pos + new Vector3(-1, 0, 0);
                if (i.initialDir == Direction.NONE) newit.initialDir = Direction.LEFT;
                else newit.initialDir = i.initialDir;
                q.Enqueue(newit);
                visited.Add(newit.pos);
            }
            if (!visited.Contains(i.pos + new Vector3(0, 0, 1)) && checkWall(i.pos, new Vector3(0, 0, 1), 0f, 1f) == null && CheckForGround(i.pos + new Vector3(0, 0, 1)) != null)
            {
                item newit;
                newit.pos = i.pos + new Vector3(0, 0, 1);
                if (i.initialDir == Direction.NONE) newit.initialDir = Direction.UP;
                else newit.initialDir = i.initialDir;
                q.Enqueue(newit);
                visited.Add(newit.pos);
            }
            if (!visited.Contains(i.pos + new Vector3(0, 0, -1)) && checkWall(i.pos, new Vector3(0, 0, -1), 0f, 1f) == null && CheckForGround(i.pos + new Vector3(0, 0, -1)) != null)
            {
                item newit;
                newit.pos = i.pos + new Vector3(0, 0, -1);
                if (i.initialDir == Direction.NONE) newit.initialDir = Direction.DOWN;
                else newit.initialDir = i.initialDir;
                q.Enqueue(newit);
                visited.Add(newit.pos);
            }
        }
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
            if ((hit.distance > min) && (hit.distance < max) && (hit.collider.gameObject.tag == "Wall" || hit.collider.gameObject.tag == "Hole" || (hit.collider.gameObject.tag == "Door" && !hit.collider.gameObject.GetComponent<DoorHandler>().isOpen())))
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

    void explode()
    {
        AudioManager.instance?.PlayExplosion();

        GameObject e = Instantiate(explosion, transform.position, transform.rotation);
        e.GetComponent<ExplosionHandler>().setRadius(explosionRange);
        destroy();
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject oobj = other.transform.gameObject;
        MovePlayer smv = GetComponent<MovePlayer>();
        if (dieScript.getState() == DeathState.ALIVE && smv.getState() == PlayerState.MOVE && oobj.tag != "Coin" && oobj.tag != "Ground" && oobj.tag != "SlimeTile")
        {
            DeathHandler ds = other.GetComponent<DeathHandler>();
            if (ds != null && ds.getState() == DeathState.ALIVE)
            {
                smv.undoMove();
            }
        }
    }
}
