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

    [Header("Efecte slime")]
    [SerializeField] Renderer slimeRenderer;        
    [SerializeField] Color slimeColor = new Color(0.35f, 0.85f, 0.25f);
    [SerializeField, Range(0, 1)] float slimeTintStrength = 0.65f;
    [SerializeField] float slimeWobbleAngle = 5f;  // oscilacio
    [SerializeField] float slimeWobbleSpeed = 22f;

    [Header("Caiguda al buit")]
    [SerializeField] Transform visualRoot;
    private Vector3 visualRootInitialPos;
    [SerializeField] float fallDuration = 1.5f;     
    [SerializeField] float fallTeeter = 0.25f;          //tremolor abans de caure
    [SerializeField] float fallDropDepth = 0.5f;    
    [SerializeField] float fallSpinSpeed = 600f;
    [SerializeField] float fallShrinkTo = 0.05f;    // desapareix dins el forat

    [Header("Idle")]
    [SerializeField] float idleBobSpeed = 3f;
    [SerializeField] float idleBobHeight = 0.03f;

    MaterialPropertyBlock mpb;
    Color slimeOgColor;
    bool slimeTinted = false;

    [SerializeField] MovePlayer moveScript;
    [SerializeField] FallHandler fallScript;
    [SerializeField] DeathHandler dieScript;
    [SerializeField] CreateLevel levelScript;
    [SerializeField] float maxInvulTime = 1;
    [SerializeField] float maxHitCooldown = 0.5f;
    [SerializeField] private Animator anim;
    [SerializeField] float maxSlimeCooldown = 1f;
    private bool canControl = true;

    void Start()
    {
        if (anim == null) anim = GetComponentInChildren<Animator>();
        moveScript.setDir(Direction.UP);

        mpb = new MaterialPropertyBlock();
        if (slimeRenderer == null) slimeRenderer = GetComponentInChildren<Renderer>();
        if (slimeRenderer != null) slimeOgColor = slimeRenderer.sharedMaterial.color;

        if (visualRoot != null) visualRootInitialPos = visualRoot.localPosition;
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

        UpdateVisualEffects();   
    }

    private void UpdateVisualEffects()
    {
        if (visualRoot == null) return;

        if (dieScript.getState() != DeathState.ALIVE) { SetSlimeTint(false); return; }
        if (fallScript.isFalling()) return;

        bool slimed = slimeCooldown > 0f;
        SetSlimeTint(slimed);

        if (slimed)
        {
            visualRoot.localPosition = visualRootInitialPos;
            visualRoot.localRotation = Quaternion.Euler(0, 0,
                Mathf.Sin(Time.time * slimeWobbleSpeed) * slimeWobbleAngle);
        }
        else if (moveScript.getState() == PlayerState.STOP)
        {
            float bob = Mathf.Sin(Time.time * idleBobSpeed) * idleBobHeight;
            visualRoot.localPosition = visualRootInitialPos + new Vector3(0, bob, 0);
            visualRoot.localRotation = Quaternion.identity;
        }
        else
        {
            visualRoot.localPosition = visualRootInitialPos;
            visualRoot.localRotation = Quaternion.identity;
        }
    }

    private void SetSlimeTint(bool on)
    {
        if (slimeRenderer == null || on == slimeTinted) return;
        slimeTinted = on;
        if (on)
        {
            slimeRenderer.GetPropertyBlock(mpb);
            Color c = Color.Lerp(slimeOgColor, slimeColor, slimeTintStrength);
            mpb.SetColor("_Color", c);
            mpb.SetColor("_BaseColor", c);
            slimeRenderer.SetPropertyBlock(mpb);
        }
        else slimeRenderer.SetPropertyBlock(null);
    }

    private void progressCooldowns()
    {
        if (invulTime >= 0) invulTime -= Time.deltaTime;
        if (hitCooldown >= 0) hitCooldown -= Time.deltaTime;
        if (slimeCooldown >= 0) slimeCooldown -= Time.deltaTime;
    }

    private void movement()
    {
        if (canControl && hitCooldown < 0 && slimeCooldown < 0)
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

        if (visualRoot != null)
        {
            if (falltime < fallTeeter)
            {
                float w = Mathf.Sin(falltime / fallTeeter * Mathf.PI * 2f) * 20f;
                visualRoot.localRotation = Quaternion.Euler(0f, 0f, w);
            }
            else
            {
                float t = Mathf.Clamp01((falltime - fallTeeter) / (fallDuration - fallTeeter));
                float e = t * t;
                float spin = fallSpinSpeed * (falltime - fallTeeter);

                visualRoot.localPosition = visualRootInitialPos + (Vector3.down * fallDropDepth * e);
                visualRoot.localRotation = Quaternion.Euler(0f, spin, 0f);
                visualRoot.localScale = Vector3.one * Mathf.Lerp(1f, fallShrinkTo, e);
            }
        }

        if (falltime > fallDuration)
        {
            AudioManager.instance?.PlayMort();
            reset();
        }
    }

    private void ResetVisual()
    {
        if (visualRoot == null) return;

        visualRoot.localPosition = visualRootInitialPos;
        visualRoot.localRotation = Quaternion.identity;
        visualRoot.localScale = Vector3.one;
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
                ResetVisual();

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
        SetSlimeTint(false);
        ResetVisual();
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
        if (slimeCooldown < 0) AudioManager.instance?.PlaySlimeImpact();  
        slimeCooldown = maxSlimeCooldown;
    }

    public void allowControl(bool a)
    {
        canControl = a;
    }
}
