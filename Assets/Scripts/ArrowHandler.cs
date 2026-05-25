using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowHandler : MonoBehaviour
{
    [SerializeField] float speed = 3f;
    Direction dir;
    Vector3 vec;
    Vector3 initPos;
    // Start is called before the first frame update
    void Start()
    {
        initPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + vec * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.tag=="Wall" && Mathf.Abs((transform.position-initPos).magnitude)>1) || (other.gameObject.tag=="Door" && !other.GetComponent<DoorHandler>().isOpen()))
        {
            Destroy(this.gameObject);
        }
        else if (other.GetComponent<IEnemy>() != null)
        {
            other.GetComponent<IEnemy>().takeDamage(dir);
            Destroy(this.gameObject);
        }
        else if (other.gameObject.tag == "Player")
        {
            other.GetComponent<PlayerHandler>().takeDamage(1, dir);
            Destroy(this.gameObject);
        }
    }

    public void setVec(Vector3 v) { 
        vec = v;
        if (v == new Vector3(1, 0, 0)) dir = Direction.RIGHT;
        else if (v == new Vector3(-1, 0, 0)) dir = Direction.LEFT;
        else if (v == new Vector3(0, 0, 1)) dir = Direction.UP;
        else if (v == new Vector3(0, 0, -1)) dir = Direction.DOWN;
    }
}
