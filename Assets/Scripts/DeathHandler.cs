using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DeathState { ALIVE, DYING, DEAD};

public class DeathHandler : MonoBehaviour
{
    [SerializeField] float duration = 1f;
    [SerializeField] float knockback = 0.5f;
    [SerializeField] Material mat;

    private Material[] ogmat;
    private Renderer[] renderers;

    private Vector3 knockvector;
    private float time = 0f;
    private DeathState state;
    private Vector3 initpos;
    // Start is called before the first frame update
    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        ogmat = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            ogmat[i] = renderers[i].material;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == DeathState.DYING)
        {
            time += Time.deltaTime;
            transform.localPosition = initpos + knockvector * knockback * (time / duration) + new Vector3(0f, -0.5f, 0f) * time/duration;
            transform.Rotate(new Vector3(90, 0, 0) * Time.deltaTime / duration);
            if (time > duration) state = DeathState.DEAD;
            else if (time > duration / 2)
            {
                float time2 = time - duration / 2;
                float dur2 = duration / 2;
                transform.localScale = new Vector3(1, 1, 1) * (1 - time2 / dur2);
                SetColor(new Color(1, 1, 1, 1));
            }
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
        initpos.y += 0.5f;
        knockvector = v * -1;
        state = DeathState.DYING;
    }

    public DeathState getState() { return state; }

    void SetColor(Color c)
    {
        foreach (var r in renderers)
        {
            r.material = mat;
            r.material.color = c;
        }
    }

    public void Restore()
    {
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].material = ogmat[i];
        transform.localScale = new Vector3(1, 1, 1);
        transform.rotation = new Quaternion(0, 0, 0, 0);

        state = DeathState.ALIVE;
        time = 0f;
    }
}
