using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeHandler : MonoBehaviour, IEnemy
{
    // Start is called before the first frame update
    [SerializeField] MovePlayer moveScript;
    [SerializeField] DeathHandler dieScript;
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
                if (b) placeslime = true;
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

    public void setLevelCreator(GameObject g)
    {
        cl = g;
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject oobj = other.transform.gameObject;
        MovePlayer smv = GetComponent<MovePlayer>();
        if (smv.getState() == PlayerState.MOVE && oobj.tag!="Coin" && oobj.tag!="Ground" && oobj.tag!="SlimeTile")
        {
            smv.undoMove();
            PlayerHandler p = oobj.GetComponent<PlayerHandler>();
            if (p != null)
            {
                p.takeDamage(1);
            }
        }
    }

    public void die()
    {
        dieScript.startDeath(moveScript.getDir());
    }

    public void destroy()
    {
        if (cl != null) cl.GetComponent<CreateLevel>().enemyKilled();
        Destroy(this.transform.gameObject);
    }
}
