using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GiantState { MOVE, CHARGE, STOMP, RECOVER }

public class GiantHandler : MonoBehaviour, IEnemy
{
    [SerializeField] MovePlayer moveScript;
    [SerializeField] DeathHandler dieScript;
    [SerializeField] float maxMoveCooldown=1f;
    [SerializeField] float stompRecoverTime=1.5f;
    [SerializeField] float chargeRecoverTime = 2f;
    [SerializeField] float chargeSpeed = 8f;
    [SerializeField] float chargeStartup = 1f;
    [SerializeField] float stompUpswing = 0.3f;
    [SerializeField] float stompHeight = 2f;
    [SerializeField] float stompSuspend = 0.5f;
    [SerializeField] float stompDownswing = 0.2f;
    [SerializeField] float maxInvulTime = 1f;
    [SerializeField] float shakeMagnitude = 0.1f;
    [SerializeField] float shakeDuration = 0.1f;
    [SerializeField] GameObject explosion;
    Vector3 chargevec;
    float time = 0f;
    bool invulnerable = false;
    float invulTime = 0f;
    bool collidedWithPlayer = false;
    GameObject player;
    float moveCooldown;
    float recovering = 0f;
    private GameObject cl;
    GiantState state = GiantState.MOVE;
    [SerializeField] int health = 5;

    // Start is called before the first frame update
    void Start()
    {
        moveCooldown = maxMoveCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        if (dieScript.getState() == DeathState.ALIVE)
        {
            progressCooldowns();
            

            if (state==GiantState.MOVE && moveCooldown < 0)
            {
                moveCooldown = maxMoveCooldown;
                bool specialAttack = Random.Range(0, 2)==1;
                if (specialAttack) {
                    if (Mathf.Abs(player.transform.position.x - transform.position.x) < 2 && Mathf.Abs(player.transform.position.z - transform.position.z) < 2)
                    {
                        time = 0f;
                        state = GiantState.STOMP;
                        invulnerable = true;
                    }
                    else if (Mathf.Abs(player.transform.position.x - transform.position.x) < 2 && Mathf.Abs(player.transform.position.z - transform.position.z) > 3)
                    {
                        time = 0f;
                        state = GiantState.CHARGE;
                        collidedWithPlayer = false;
                        if (player.transform.position.z - transform.position.z > 0)
                        {
                            moveScript.setDir(Direction.UP);
                            chargevec = new Vector3(0, 0, 1);
                        }
                        else
                        {
                            moveScript.setDir(Direction.DOWN);
                            chargevec = new Vector3(0, 0, -1);
                        }
                    }
                    else if (Mathf.Abs(player.transform.position.z - transform.position.z) < 2 && Mathf.Abs(player.transform.position.x - transform.position.x) > 3)
                    {
                        time = 0f;
                        state = GiantState.CHARGE;
                        collidedWithPlayer = false;
                        if (player.transform.position.x - transform.position.x > 0)
                        {
                            moveScript.setDir(Direction.RIGHT);
                            chargevec = new Vector3(1, 0, 0);
                        }
                        else
                        {
                            moveScript.setDir(Direction.LEFT);
                            chargevec = new Vector3(-1, 0, 0);
                        }
                    }
                    else
                    {
                        moveScript.tryMove(searchPlayer(player.transform.position));
                    }
                }
                else {
                    moveScript.tryMove(searchPlayer(player.transform.position));
                }
            }


            else if (state == GiantState.CHARGE)
            {
                time += Time.deltaTime;
                if (time > chargeStartup)
                {
                    if (!invulnerable) invulnerable = true;
                    transform.position = transform.position + chargevec * Time.deltaTime * chargeSpeed;
                    if (checkWall(transform.position - chargevec * 0.5f, chargevec, 0, 1f)!=null)
                    {
                        recovering = chargeRecoverTime;
                        invulTime = 0.35f;
                        invulnerable = false;
                        state = GiantState.RECOVER;
                        correctPosition();
                        StartCoroutine(ShakeCoroutine());
                        GameObject obj = Instantiate(explosion, transform.position, transform.rotation);
                        ExplosionHandler eh = obj.GetComponent<ExplosionHandler>();
                        eh.setRadius(1.5f);
                        eh.setDuration(0.3f);
                    }
                    else if (collidedWithPlayer)
                    {
                        time = 0f;
                        invulTime = 0.35f;
                        invulnerable = false;
                        state = GiantState.MOVE;
                        moveCooldown = maxMoveCooldown;
                        player.GetComponent<MovePlayer>().stopMove();
                        correctPosition();
                        StartCoroutine(ShakeCoroutine());
                        GameObject obj = Instantiate(explosion, transform.position, transform.rotation);
                        ExplosionHandler eh = obj.GetComponent<ExplosionHandler>();
                        eh.setRadius(2.5f);
                        eh.setDuration(0.3f);
                    }
                }
            }


            else if (state == GiantState.STOMP)
            {
                time += Time.deltaTime;
                if (time < stompUpswing)
                {
                    transform.position += new Vector3(0, stompHeight, 0) * Time.deltaTime / stompUpswing;
                }
                else if (time > stompUpswing + stompSuspend + stompDownswing)
                {
                    transform.position = new Vector3(transform.position.x, (int)transform.position.y, transform.position.z);
                    state = GiantState.RECOVER;
                    recovering = stompRecoverTime;
                    invulTime = 0.35f;
                    invulnerable = false;
                    StartCoroutine(ShakeCoroutine());
                    shovePlayer();
                    GameObject obj = Instantiate(explosion, transform.position, transform.rotation);
                    ExplosionHandler eh = obj.GetComponent<ExplosionHandler>();
                    eh.setRadius(2.5f);
                    eh.setDuration(0.2f);
                    eh.setScale(new Vector3(1, 0.3f, 1));
                }
                else if (time > stompUpswing + stompSuspend)
                {
                    transform.position += new Vector3(0, -stompHeight, 0) * Time.deltaTime / stompDownswing;
                }
            }


            else if (state == GiantState.RECOVER)
            {
                if (recovering < 0) state = GiantState.MOVE;
            }
        }
        else if (dieScript.getState() == DeathState.DEAD) destroy();
    }

    void shovePlayer()
    {
        player.GetComponent<MovePlayer>().stopMove();
        Direction[] dirs ={ Direction.UP, Direction.DOWN, Direction.RIGHT, Direction.LEFT};
        Vector3[] vecs ={ new Vector3(0, 0, -1), new Vector3(0, 0, 1), new Vector3(-1, 0, 0), new Vector3(1, 0, 0)};
        if (Mathf.Abs((player.transform.position - transform.position).magnitude) < 0.2f)
        {
            bool found = false;
            while (!found)
            {
                int r = Random.Range(0, 4);
                if (checkWall(player.transform.position - 0.5f * vecs[r], vecs[r], 0, 1.5f) == null)
                {
                    player.transform.position += vecs[r];
                    player.GetComponent<MovePlayer>().setDir(dirs[r]);
                    found = true;
                }
            }
        }
    }

    void correctPosition()
    {
        if (chargevec.x!=0)
            transform.position = new Vector3((int)transform.position.x, transform.position.y, transform.position.z);
        else
            transform.position = new Vector3(transform.position.x, transform.position.y, (int)transform.position.z);
        RaycastHit[] hits = Physics.RaycastAll(transform.position-chargevec, chargevec, 1.1f);
        foreach (RaycastHit hit in hits)
        {
            if ((hit.collider.gameObject.tag == "Wall" || (hit.collider.gameObject.tag == "Door" && !hit.collider.gameObject.GetComponent<DoorHandler>().isOpen()) || hit.collider.gameObject.tag=="Player"))
            {
                transform.position -= chargevec;
                break;
            }
        }
    }

    void progressCooldowns()
    {
        float time = Time.deltaTime;
        moveCooldown -= time;
        if (recovering >= 0) recovering -= time;
        if (invulTime >= 0) invulTime -= time;
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

        while (q.Count != 0)
        {
            i = q.Dequeue();
            if (Mathf.Abs(i.pos.x - targetpos.x) < 1 && Mathf.Abs(i.pos.z - targetpos.z) < 1) return i.initialDir;
            if (!visited.Contains(i.pos + new Vector3(1, 0, 0)) && checkWall(i.pos, new Vector3(1, 0, 0), 0f, 1f) == null && CheckForGround(i.pos + new Vector3(0, 0, -1)) != null)
            {
                item newit;
                newit.pos = i.pos + new Vector3(1, 0, 0);
                if (i.initialDir == Direction.NONE) newit.initialDir = Direction.RIGHT;
                else newit.initialDir = i.initialDir;
                q.Enqueue(newit);
                visited.Add(newit.pos);
            }
            if (!visited.Contains(i.pos + new Vector3(-1, 0, 0)) && checkWall(i.pos, new Vector3(-1, 0, 0), 0f, 1f) == null && CheckForGround(i.pos + new Vector3(0, 0, -1)) != null)
            {
                item newit;
                newit.pos = i.pos + new Vector3(-1, 0, 0);
                if (i.initialDir == Direction.NONE) newit.initialDir = Direction.LEFT;
                else newit.initialDir = i.initialDir;
                q.Enqueue(newit);
                visited.Add(newit.pos);
            }
            if (!visited.Contains(i.pos + new Vector3(0, 0, 1)) && checkWall(i.pos, new Vector3(0, 0, 1), 0f, 1f) == null && CheckForGround(i.pos + new Vector3(0, 0, -1)) != null)
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
            if ((hit.distance > min) && (hit.distance < max) && (hit.collider.gameObject.tag == "Wall" || (hit.collider.gameObject.tag == "Door" && !hit.collider.gameObject.GetComponent<DoorHandler>().isOpen())))
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

    public void setLevelCreator(GameObject g)
    {
        cl = g;
    }

    public void setPlayer(GameObject p) { player = p; }

    public void takeDamage(Direction d)
    {
        if (!invulnerable && invulTime < 0)
        {
            GetComponent<HitEffect>()?.PlayHitEffect(maxInvulTime);
            health -= 1;
            if (health == 0) if (dieScript.getState() == DeathState.ALIVE) dieScript.startDeath(d);
            else invulTime = maxInvulTime;
        }
    }

    public void destroy()
    {
        if (cl != null) cl.GetComponent<CreateLevel>().enemyKilled();
        Destroy(this.transform.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject oobj = other.transform.gameObject;
        if (dieScript.getState() == DeathState.ALIVE && oobj.tag != "Coin" && oobj.tag != "Ground" && oobj.tag != "SlimeTile") {
            if (moveScript.getState() == PlayerState.MOVE)
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
            else if (state == GiantState.CHARGE)
            {
                DeathHandler ds = other.GetComponent<DeathHandler>() ?? other.GetComponentInParent<DeathHandler>();
                if (ds != null && ds.getState() == DeathState.ALIVE)
                {
                    PlayerHandler p = oobj.GetComponent<PlayerHandler>() ?? oobj.GetComponentInParent<PlayerHandler>();
                    if (p != null)
                    {
                        MovePlayer pmv = p.GetComponent<MovePlayer>();
                        p.takeDamage(1, moveScript.getDir());
                        collidedWithPlayer = true;
                    }
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

    IEnumerator ShakeCoroutine()
    {
        Transform cam = Camera.main.transform;
        Vector3 originalPos = cam.localPosition;
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;
            cam.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cam.localPosition = originalPos;
    }
}
