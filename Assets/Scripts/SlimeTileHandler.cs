using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeTileHandler : MonoBehaviour
{
    [SerializeField] float duration = 3f;
    private float timepassed = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timepassed += Time.deltaTime;
        if (timepassed > duration)
        {
            Destroy(transform.gameObject);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<PlayerHandler>().slime();
            Destroy(transform.gameObject);
        }
    }
}
