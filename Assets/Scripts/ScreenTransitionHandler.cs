using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTransitionHandler : MonoBehaviour
{
    [SerializeField] float duration = 1f;
    [SerializeField] float speed = 3f;
    bool active = false;
    float time = 0f;
    float scale = 0f;
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (time > duration)
        {
            active = false;
            transform.localScale = new Vector3(0, 0, 0);
            time = 0;
        }
        else if (time > duration / 2)
        {
            time += Time.deltaTime;
            scale -= speed * Time.deltaTime;
            if (scale < 0) transform.localScale = new Vector3(0, 0, 0);
            else transform.localScale = new Vector3(scale, scale, scale);
        }
        else if (active)
        {
            time += Time.deltaTime;
            scale += speed * Time.deltaTime;
            transform.localScale = new Vector3(scale, scale, scale);
        }
    }

    public void transition()
    {
        active = true;
    }

    public float getDuration() { return duration; }
}
