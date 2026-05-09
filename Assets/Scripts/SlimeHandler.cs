using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeHandler : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] MovePlayer moveScript;
    private GameObject cl;
    [SerializeField] float maxjumpwait = 1;
    float jumpwait = 0;

    void Start()
    {
        Random.InitState(3);
    }

    // Update is called once per frame
    void Update()
    {
        if (jumpwait >= 0) jumpwait -= Time.deltaTime;
        else
        {
            movement();
            jumpwait = maxjumpwait;
        }
    }

    private void movement()
    {
        float r = Random.Range(0, 4);
        if (r < 1) moveScript.tryMove(Direction.UP);
        else if (r < 2) moveScript.tryMove(Direction.DOWN);
        else if (r < 3) moveScript.tryMove(Direction.LEFT);
        else moveScript.tryMove(Direction.RIGHT);
    }

    public void setLevelCreator(GameObject g)
    {
        cl = g;
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject oobj = other.transform.gameObject;
        MovePlayer smv = GetComponent<MovePlayer>();
        if (smv.getState() == PlayerState.MOVE && oobj.tag!="Coin" && oobj.tag!="Ground")
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
        cl.GetComponent<CreateLevel>().enemyKilled();
        Destroy(this.transform.gameObject);
    }
}
