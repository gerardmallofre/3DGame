using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHandler : MonoBehaviour
{
    GameObject cl;
    private bool isopen = false;
    private bool goneThrough = false;

    private Animator animator;

    void Start()
    {
animator = GetComponentInChildren<Animator>();    }

    void Update()
    {

    }

    public void setLevelCreator(GameObject g) { cl = g; }

    public void open()
    {
        isopen = true;
        AudioManager.instance.PlayOpenDoor();

        if (animator != null)
        {
            animator.SetTrigger("Obrir");
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (!goneThrough && isopen && other.transform.gameObject.tag == "Player")
        {
            goneThrough = true;
            cl.GetComponent<CreateLevel>().enteredDoor();
            HUDManager.Instance.AddDoor();
        }
    }

    public bool isOpen()
    {
        return isopen;
    }
}