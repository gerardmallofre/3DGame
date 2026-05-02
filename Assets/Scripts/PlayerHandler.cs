using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    private int coins = 0;
    private int health = 3;
    private float invulTime = 0;
    [SerializeField] MovePlayer moveScript;
    [SerializeField] float maxInvulTime = 1;
    [SerializeField] public AudioClip hurtSound;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        movement();
        if (invulTime >= 0) invulTime -= Time.deltaTime;
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
        if (invulTime < 0)
        {
            AudioSource.PlayClipAtPoint(hurtSound, Camera.main.transform.position);
            health -= dmg;
            if (health > 0)
                invulTime = maxInvulTime;
            else die();
        }
    }

    void die()
    {
        Destroy(this.transform.gameObject);
    }
}
