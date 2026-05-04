using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    private int coins = 0;
    private int health = 3;
    private float invulTime = 0;
    private float hitCooldown = 0;
    [SerializeField] MovePlayer moveScript;
    [SerializeField] float maxInvulTime = 1;
    [SerializeField] public AudioClip hurtSound;
    [SerializeField] float maxHitCooldown = 0.5f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        progressCooldowns();
        movement();
    }

    void progressCooldowns()
    {
        if (invulTime >= 0) invulTime -= Time.deltaTime;
        if (hitCooldown >= 0) hitCooldown -= Time.deltaTime;
    }

    void movement()
    {
        if (hitCooldown < 0)
        {
            if (Input.GetKey(KeyCode.UpArrow)) moveScript.tryMove(Direction.UP);
            else if (Input.GetKey(KeyCode.RightArrow)) moveScript.tryMove(Direction.RIGHT);
            else if (Input.GetKey(KeyCode.LeftArrow)) moveScript.tryMove(Direction.LEFT);
            else if (Input.GetKey(KeyCode.DownArrow)) moveScript.tryMove(Direction.DOWN);
        }
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
            HUDManager.Instance?.SetHealth(health);
            if (health > 0)
                invulTime = maxInvulTime;
            else die();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        MovePlayer omv = other.GetComponent<MovePlayer>();
        MovePlayer pmv = GetComponent<MovePlayer>();
        if (omv != null && pmv.getState()==PlayerState.MOVE)
        {
            pmv.undoMove();
            hitCooldown = maxHitCooldown;

            GameObject oth = other.transform.gameObject;
            if (oth.tag == "Slime")
            {
                oth.GetComponent<SlimeHandler>().die();
            }
        }
    }

    void die()
    {
        Destroy(this.transform.gameObject);
    }
}
