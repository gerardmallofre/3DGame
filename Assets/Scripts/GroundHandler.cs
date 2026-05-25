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
            GameObject obj=CheckAbove();
            if (obj != null)
            {
                AxeHandler ah = obj.GetComponent<AxeHandler>();   
                if (ah != null) ah.falling = true;
                obj.transform.localPosition -= new Vector3(0, fallspeed * Time.deltaTime, 0);
            }
            if (time > 1)
            {
                Destroy(transform.gameObject);
                if (obj != null) Destroy(obj);
            }
        }
    }

    public void setFallState(FallState state)
    {
        if (fallstate != FallState.FALL)
        {
            fallstate = state;

        }
    }

    GameObject CheckAbove()
    {
        float min = 0f; float max = 5f; Vector3 v = new Vector3(0, 1, 0);
        Vector3 P = transform.localPosition + new Vector3(0, -0.5f, 0);
        float closestDistance = max + 1.0f;
        GameObject obj = null;

        RaycastHit[] hits = Physics.RaycastAll(P, v, max);
        foreach (RaycastHit hit in hits)
        {
            var g = hit.collider.gameObject;
            if ((hit.distance > min) && (hit.distance < max) && g.tag != "Ground"
                && g.tag != "Player" && g.GetComponent<IEnemy>() == null && g.tag != "Hitbox")
                if (hit.distance < closestDistance)
                {
                    closestDistance = hit.distance;
                    obj = g;
                }
        }

        if (obj != null)
        {
            Transform t = obj.transform;
            while (t.parent != null && t.parent != transform.parent)
                t = t.parent;
            obj = t.gameObject;
        }
        return obj;
    }
}
