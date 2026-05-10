using System.Collections;
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
    [SerializeField] private Animator anim;

    void Start()
    {
        if (anim == null)
            anim = GetComponentInChildren<Animator>();
    }

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

            if (Input.GetKeyDown(KeyCode.Space))
                anim?.SetTrigger("attack");
        }
    }

    public void addCoin() { coins++; }

    public void takeDamage(int dmg)
    {
        if (invulTime < 0)
        {
            GetComponent<HitEffect>()?.PlayHitEffect(maxInvulTime);
            AudioSource.PlayClipAtPoint(hurtSound, Camera.main.transform.position);
            health -= dmg;
            HUDManager.Instance?.SetHealth(health);
            if (health > 0) invulTime = maxInvulTime;
            else die();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        MovePlayer omv = other.GetComponent<MovePlayer>()
                      ?? other.GetComponentInParent<MovePlayer>();
        MovePlayer pmv = GetComponent<MovePlayer>();
        if (omv != null && pmv.getState() == PlayerState.MOVE)
        {
            pmv.undoMove();
            hitCooldown = maxHitCooldown;
            IEnemy enemy = other.GetComponent<IEnemy>()
                        ?? other.GetComponentInParent<IEnemy>();
            if (enemy != null) enemy.die();
        }
    }

    void die() { Destroy(gameObject); }
}