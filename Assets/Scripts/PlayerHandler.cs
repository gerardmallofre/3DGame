using System.Xml.Serialization;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    private int health = 3;
    private float invulTime = 0f;
    private float hitCooldown = 0f;
    private float slimeCooldown = 0f;
    private float falltime = 0f;
    private bool isGodMode = false;
    [SerializeField] MovePlayer moveScript;
    [SerializeField] FallHandler fallScript;
    [SerializeField] DeathHandler dieScript;
    [SerializeField] CreateLevel levelScript;
    [SerializeField] float maxInvulTime = 1;
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
        if (dieScript.getState() == DeathState.ALIVE)
        {
            if (!fallScript.isFalling() && moveScript.getState() == PlayerState.STOP) fallScript.fallCheck();
            if (fallScript.isFalling()) fall();
            progressCooldowns();
            if (!fallScript.isFalling()) movement();
        }
        else if (dieScript.getState() == DeathState.DEAD) reset();
    }

    private void progressCooldowns()
    {
        if (invulTime >= 0) invulTime -= Time.deltaTime;
        if (hitCooldown >= 0) hitCooldown -= Time.deltaTime;
        if (slimeCooldown >= 0) slimeCooldown -= Time.deltaTime;
    }

    private void movement()
    {
        if (hitCooldown < 0 && slimeCooldown < 0)
        {
            bool b = false;
            if (Input.GetKey(KeyCode.UpArrow)) b = moveScript.tryMove(Direction.UP);
            else if (Input.GetKey(KeyCode.RightArrow)) b = moveScript.tryMove(Direction.RIGHT);
            else if (Input.GetKey(KeyCode.LeftArrow)) b = moveScript.tryMove(Direction.LEFT);
            else if (Input.GetKey(KeyCode.DownArrow)) b = moveScript.tryMove(Direction.DOWN);

            if (b)
            {
                GameObject obj = CheckForEnemy(moveScript.getVec());
                if (obj != null)
                {
                    anim?.SetTrigger("attack");
                }
            }
        }
    }

    private GameObject CheckForEnemy(Vector3 v)
    {
        if (v == Vector3.zero) return null;

        Vector3 targetTile = transform.position + v;
        Vector3 halfExtents = new Vector3(0.4f, 1.5f, 0.4f);

        Collider[] hits = Physics.OverlapBox(
            targetTile, halfExtents, Quaternion.identity, ~0, QueryTriggerInteraction.Collide);

        foreach (Collider hit in hits)
        {
            DeathHandler ds = hit.GetComponent<DeathHandler>() ?? hit.GetComponentInParent<DeathHandler>();
            IEnemy enemy = hit.GetComponent<IEnemy>() ?? hit.GetComponentInParent<IEnemy>();
            if (ds != null && enemy != null && ds.getState() == DeathState.ALIVE)
                return ds.gameObject;
        }
        return null;
    }

    private void fall()
    {
        falltime += Time.deltaTime;
        if (falltime > 2)
        {
            takeDamage(3, Direction.NONE, true);
        }
        else if (falltime > 0.5) {
            transform.localPosition -= new Vector3(0, (Time.deltaTime) * 10f, 0);
        }
    }

    public void takeDamage(int dmg, Direction d, bool ignoreGodMode = false)
    {
        if (isGodMode && !ignoreGodMode) return; 
        if (invulTime < 0 && dieScript.getState() == DeathState.ALIVE)
        {
            GetComponent<HitEffect>()?.PlayHitEffect(maxInvulTime);
            health -= dmg;
            HUDManager.Instance?.SetHealth(health);

            if (health > 0)
            {
                AudioManager.instance?.PlayDamage(); 
                invulTime = maxInvulTime;
            }
            else
            {
                die(d); 
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        MovePlayer omv = other.GetComponent<MovePlayer>()
                        ?? other.GetComponentInParent<MovePlayer>();
        MovePlayer pmv = GetComponent<MovePlayer>();
        if (omv != null && pmv.getState() == PlayerState.MOVE)
        {
            DeathHandler ds = other.GetComponent<DeathHandler>();
            if (ds != null && ds.getState() == DeathState.ALIVE)
            {
                pmv.undoMove();
                hitCooldown = maxHitCooldown;
                IEnemy enemy = other.GetComponent<IEnemy>()
                            ?? other.GetComponentInParent<IEnemy>();
                if (enemy != null) enemy.takeDamage(moveScript.getDir());
            }
        }
    }

    public void toggleGodMode()
    {
        isGodMode = !isGodMode;
    }

    void reset()
    {
        dieScript.Restore();
        levelScript.restart();
        health = 3;
        fallScript.setFalling(false);
        falltime = 0f;
        isGodMode = false;
        HUDManager.Instance?.ResetHUD();
    }

    void die(Direction d)
    {
        if (dieScript.getState() == DeathState.ALIVE) {
           dieScript.startDeath(d);
        }
    }

    public void slime()
    {
        slimeCooldown = maxSlimeCooldown;
    }
}
