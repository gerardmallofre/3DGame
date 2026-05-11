using System.Collections;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    private int health = 3;
    private float invulTime = 0f;
    private float hitCooldown = 0f;
    private float slimeCooldown = 0f;
    private bool falling = false;
    private float falltime = 0f;
    [SerializeField] MovePlayer moveScript;
    [SerializeField] CreateLevel levelScript;
    [SerializeField] float maxInvulTime = 1;
    [SerializeField] public AudioClip hurtSound;
    [SerializeField] float maxHitCooldown = 0.5f;
    [SerializeField] private Animator anim;
    [SerializeField] float maxSlimeCooldown = 1f;

    void Start()
    {
        if (anim == null)
            anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (!falling && moveScript.getState()==PlayerState.STOP) fallCheck();
        if (falling) fall();
        progressCooldowns();
        if (!falling) movement();
    }

    private void progressCooldowns()
    {
        if (invulTime >= 0) invulTime -= Time.deltaTime;
        if (hitCooldown >= 0) hitCooldown -= Time.deltaTime;
        if (slimeCooldown >= 0) slimeCooldown -= Time.deltaTime;
    }

    private void movement()
    {
        if (hitCooldown < 0 && slimeCooldown<0)
        {
            if (Input.GetKey(KeyCode.UpArrow)) moveScript.tryMove(Direction.UP);
            else if (Input.GetKey(KeyCode.RightArrow)) moveScript.tryMove(Direction.RIGHT);
            else if (Input.GetKey(KeyCode.LeftArrow)) moveScript.tryMove(Direction.LEFT);
            else if (Input.GetKey(KeyCode.DownArrow)) moveScript.tryMove(Direction.DOWN);

            if (Input.GetKeyDown(KeyCode.Space))
                anim?.SetTrigger("attack");
        }
    }

    private GameObject CheckForGround()
    {
        float min = 0f; float max = 1.5f; Vector3 v = new Vector3(0, -1, 0); Vector3 P = transform.localPosition;
        P += new Vector3(0, 0.5f, 0);
        float closestDistance = max + 1.0f;
        GameObject obj = null;

        // Physics.RaycastAll returns all colliders in a given ray (P, v) within a given distance (max)
        RaycastHit[] hits = Physics.RaycastAll(P, v, max);
        foreach (RaycastHit hit in hits)
        {
            if ((hit.distance > min) && (hit.distance < max) && (hit.collider.gameObject.tag == "Ground"))
                if (hit.distance < closestDistance)
                {
                    closestDistance = hit.distance;
                    obj = hit.collider.gameObject;
                }
        }

        return obj;
    }

    private void fallCheck()
    {
        GameObject obj = CheckForGround();
        if (obj==null)
        {
            falling = true;
        }
    }

    private void fall()
    {
        falltime += Time.deltaTime;
        if (falltime > 2)
        {
            takeDamage(3);
        }
        else if (falltime > 0.5) {
            transform.localPosition -= new Vector3(0, (Time.deltaTime) * 10f, 0);
        }
    }
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

    void die()
    {
        levelScript.restart();
        health = 3;
        HUDManager.Instance?.SetHealth(health);
        HUDManager.Instance?.SetCoins(0);
        falling = false;
        falltime = 0f;
    }

    public void slime()
    {
        slimeCooldown = maxSlimeCooldown;
    }
}
