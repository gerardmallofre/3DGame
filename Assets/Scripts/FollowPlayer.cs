using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private GameObject player;
    Transform camt;
    Vector3 camstartpos;

    // Start is called before the first frame update
    void Start()
    {
        camt = this.GetComponent<Transform>();
        camstartpos = camt.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            Transform playert = player.GetComponent<Transform>();

            camt.position = new Vector3(camstartpos.x, camstartpos.y, playert.position.z + camstartpos.z);
        }
    }
}
