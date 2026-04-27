using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    private int coins = 0;
    private int health = 3;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addCoin()
    {
        coins++;
    }

    public void takeDamage(int dmg)
    {
        health-=dmg;
    }
}
