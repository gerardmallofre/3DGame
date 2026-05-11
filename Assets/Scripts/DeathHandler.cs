using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DeathState { ALIVE, DYING, DEAD};

public class DeathHandler : MonoBehaviour
{
    [SerializeField] float duration = 1f;
    [SerializeField] float knockback = 0.5f;
    private Vector3 knockvector;
    private float time = 0f;
    private DeathState state;
    private Vector3 initpos;
    private Vector3 rotaxis;
    private int rotdirection;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (state == DeathState.DYING)
        {
            time += Time.deltaTime;
            transform.localPosition = initpos + knockvector * knockback * (time / duration) + new Vector3(0f, -0.5f, 0f) * time/duration;
            transform.Rotate(rotaxis * rotdirection * Time.deltaTime / duration * 90);
            if (time > duration) state = DeathState.DEAD;
        }
    }
        
    public void startDeath(Direction d)
    {
        Vector3 v = new Vector3(0, 0, 0);
        if (d == Direction.UP) v = new Vector3(0, 0, 1);
        else if (d == Direction.DOWN) v = new Vector3(0, 0, -1);
        else if (d == Direction.RIGHT) v = new Vector3(1, 0, 0);
        else if (d == Direction.LEFT) v = new Vector3(-1, 0, 0);

        initpos = transform.localPosition;
        initpos = transform.localPosition;
        knockvector = v * -1;
        state = DeathState.DYING;

        if (v==new Vector3(1, 0, 0)) { rotaxis = new Vector3(0, 0, 1); rotdirection = 1; }
        else if (v == new Vector3(-1, 0, 0)) { rotaxis = new Vector3(0, 0, 1); rotdirection = -1; }
        else if (v == new Vector3(0, 0, 1)) { rotaxis = new Vector3(1, 0, 0); rotdirection = -1; }
        else if (v == new Vector3(0, 0, -1)) { rotaxis = new Vector3(1, 0, 0); rotdirection = 1; }
    }

    public DeathState getState() { return state; }
}
