using UnityEngine;

public class SpikeTrapHandler : MonoBehaviour
{
    private float timeSpikesOut = 0.25f;
    private float timeSpikesIn = 1.5f;
    private float timeRising = 0.542f;
    private float timeFalling = 0.542f;
    private float damageCooldown = 2.0f;

    private enum SpikeState { IN, RISING, OUT, FALLING }
    private SpikeState state = SpikeState.IN;

    private Animator anim;
    private float timer = 0f;
    private float dmgCooldownCounter = 0f;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        if (anim) anim.Play("SpikeTrap_Deactivate", 0, 1f);
    }

    void Update()
    {
        timer += Time.deltaTime;
        dmgCooldownCounter -= Time.deltaTime;

        switch (state)
        {
            case SpikeState.IN:
                if (timer >= timeSpikesIn)
                { timer = 0f; state = SpikeState.RISING; anim?.Play("SpikeTrap_Activate", 0, 0f); }
                break;

            case SpikeState.RISING:
                if (timer >= timeRising)
                {
                    timer = 0f;
                    state = SpikeState.OUT;
                    anim?.Play("SpikeTrap_Activate", 0, 1f); 
                }
                break;

            case SpikeState.OUT:
                if (timer >= timeSpikesOut)
                { timer = 0f; state = SpikeState.FALLING; anim?.Play("SpikeTrap_Deactivate", 0, 0f); }
                break;

            case SpikeState.FALLING:
                if (timer >= timeFalling)
                { timer = 0f; state = SpikeState.IN; }
                break;
        }

        //Debug.Log("Spike state: " + state);
    }




    void OnTriggerStay(Collider other)
    {
        if (state == SpikeState.IN || dmgCooldownCounter > 0) return;
        PlayerHandler p = other.GetComponent<PlayerHandler>()
                       ?? other.GetComponentInParent<PlayerHandler>();
        if (p != null) { p.takeDamage(1, Direction.NONE); dmgCooldownCounter = damageCooldown; }
    }

    void OnTriggerEnter(Collider other)
    {
        if (state == SpikeState.IN || dmgCooldownCounter > 0) return;
        PlayerHandler p = other.GetComponent<PlayerHandler>()
                       ?? other.GetComponentInParent<PlayerHandler>();
        if (p != null) { p.takeDamage(1, Direction.NONE); dmgCooldownCounter = damageCooldown; }
    }

}