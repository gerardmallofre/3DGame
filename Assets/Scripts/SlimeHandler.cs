using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeHandler : MonoBehaviour, IEnemy
{
    // Start is called before the first frame update
    [SerializeField] MovePlayer moveScript;
    [SerializeField] DeathHandler dieScript;
    [SerializeField] FallHandler fallScript;
    private float falltimer = 0f;
    private GameObject cl;
    [SerializeField] float maxjumpwait = 1;
    private float jumpwait = 0;
    private bool placeslime = false;

    void Start()
    {
        cl.GetComponent<CreateLevel>().slimeTile(transform.localPosition);
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
                if (placeslime && moveScript.getState() == PlayerState.STOP)
                {
                    placeslime = false;
                    cl.GetComponent<CreateLevel>().slimeTile(transform.localPosition);
                }
                if (jumpwait >= 0) jumpwait -= Time.deltaTime;
                else
                {
                    bool b = movement();
                    jumpwait = maxjumpwait;
                    if (b)
                    {
                        placeslime = true;
                    }
                }
            }
        }
        else if (dieScript.getState() == DeathState.DEAD) destroy();
    }

    private bool movement()
    {
        float r = Random.Range(0, 4);
        if (r < 1) return moveScript.tryMove(Direction.UP);
        else if (r < 2) return moveScript.tryMove(Direction.DOWN);
        else if (r < 3) return moveScript.tryMove(Direction.LEFT);
        else return moveScript.tryMove(Direction.RIGHT);
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

    bool inLine(Direction d1, Direction d2)
    {
        if ((d1 == Direction.UP || d1 == Direction.DOWN) && (d2 == Direction.UP || d2 == Direction.DOWN)) return true;
        if ((d1 == Direction.RIGHT || d1 == Direction.LEFT) && (d2 == Direction.RIGHT || d2 == Direction.LEFT)) return true;
        return false;
    }

    void OnTriggerEnter(Collider other) { HandleCollision(other); }
    void OnTriggerStay(Collider other) { HandleCollision(other); }

    void HandleCollision(Collider other)
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
        dieScript.startDeath(d);
    }

    public void destroy()
    {
        if (cl != null) cl.GetComponent<CreateLevel>().enemyKilled();
        Destroy(this.transform.gameObject);
    }
}
