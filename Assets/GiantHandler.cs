using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantHandler : MonoBehaviour, IEnemy
{
    [SerializeField] MovePlayer moveScript;
    [SerializeField] DeathHandler dieScript;
    private GameObject cl;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (dieScript.getState() == DeathState.ALIVE)
        {
        }
        else if (dieScript.getState() == DeathState.DEAD) destroy();
    }

    public void setLevelCreator(GameObject g)
    {
        cl = g;
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
