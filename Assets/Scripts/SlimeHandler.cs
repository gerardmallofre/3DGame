using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeHandler : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] MovePlayer moveScript;

    void Start()
    {
        Random.InitState(3);
    }

    // Update is called once per frame
    void Update()
    {
        movement();
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
        if (p != null)
        {
            p.takeDamage(1);
        }
    }
}
