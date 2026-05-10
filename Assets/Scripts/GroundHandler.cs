using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FallState { NONE, SHAKE, FALL };

public class GroundHandler : MonoBehaviour
{
    private FallState fallstate=FallState.NONE;
    [SerializeField] float maxshake = 0.1f;
    [SerializeField] float timeshake = 0.2f;
    [SerializeField] float fallspeed = 5f;
    int shakeToggle = 1;
    float time = 0.0f;
    private Vector3 initPos;

    // Start is called before the first frame update
    void Start()
    {
        initPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (fallstate == FallState.SHAKE)
        {
            time += Time.deltaTime;
            transform.localPosition = new Vector3(initPos.x, initPos.y + shakeToggle * time/timeshake * 2 * maxshake - shakeToggle * maxshake, initPos.z);
            if (time > timeshake)
            {
                time = 0.0f;
                shakeToggle *= -1;
            }
        }
        else if (fallstate == FallState.FALL)
        {
            time += Time.deltaTime;
            transform.localPosition = new Vector3(initPos.x, initPos.y - fallspeed * time, initPos.z);
            if (time > 2)
            {
                Destroy(transform.gameObject);
            }
        }
    }

    public void setFallState(FallState state)
    {
        if (fallstate!=FallState.FALL) fallstate = state;
    }
}
