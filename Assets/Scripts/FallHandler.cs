using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallHandler : MonoBehaviour
{
    private bool falling = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool isFalling()
    {
        return falling;
    }

    public void setFalling(bool f) { falling = f; }

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

    public void fallCheck()
    {
        GameObject obj = CheckForGround();
        if (obj == null)
        {
            falling = true;
        }
    }
}
