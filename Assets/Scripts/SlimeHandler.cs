using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeHandler : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] MovePlayer moveScript;
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

    // I'M THE REAL ONE!!!
    private void movement()
    {
        float r = Random.Range(0, 4);
        if (r < 1) moveScript.tryMove(Direction.UP);
        else if (r < 2) moveScript.tryMove(Direction.DOWN);
        else if (r < 3) moveScript.tryMove(Direction.LEFT);
        else moveScript.tryMove(Direction.RIGHT);
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerHandler p = other.GetComponent<PlayerHandler>();
        MovePlayer smv = GetComponent<MovePlayer>();
        if (p != null && smv.getState() == PlayerState.MOVE)
        {
            p.takeDamage(1);
            smv.undoMove();
        }
    }

    public void die()
    {
        Destroy(this.transform.gameObject);
    }
}
