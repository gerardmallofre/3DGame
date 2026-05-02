using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    private int coins = 0;
    private int health = 3;
    [SerializeField] MovePlayer moveScript;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        movement();
    }

    void movement()
    {
        if (Input.GetKey(KeyCode.UpArrow)) moveScript.tryMove(Direction.UP);
        else if (Input.GetKey(KeyCode.RightArrow)) moveScript.tryMove(Direction.RIGHT);
        else if (Input.GetKey(KeyCode.LeftArrow)) moveScript.tryMove(Direction.LEFT);
        else if (Input.GetKey(KeyCode.DownArrow)) moveScript.tryMove(Direction.DOWN);
    }

    public void addCoin()
    {
        coins++;
    }

    public void takeDamage(int dmg)
    {
        health -= dmg;
    }
}
