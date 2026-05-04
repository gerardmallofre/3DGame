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
        Debug.Log("Door opened");
        isopen = true;
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enter door");
        if (isopen)
        {
            cl.GetComponent<CreateLevel>().advanceLevel();
        }
    }
}
