using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHandler : MonoBehaviour
{
    GameObject cl;
    private bool isopen = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setLevelCreator(GameObject g) { cl = g; }

    public void open()
    {
        isopen = true;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (isopen && other.transform.gameObject.tag=="Player")
        {
            cl.GetComponent<CreateLevel>().advanceLevel();
        }
    }

    public bool isOpen()
    {
        return isopen;
    }
}
