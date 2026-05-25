using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTrapHandler : MonoBehaviour
{
    [SerializeField] float shotCooldown=2f;
    [SerializeField] GameObject arrow;
    Vector3 arrowVec;
    GameObject cl;
    float time;
    // Start is called before the first frame update
    void Start()
    {
        time = shotCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime;
        if (time < 0)
        {
            time = shotCooldown;
            GameObject obj = Instantiate(arrow, new Vector3(transform.position.x, 0.4f, transform.position.z), transform.rotation);
            obj.transform.parent = cl.transform;
            obj.GetComponent<ArrowHandler>().setVec(arrowVec);
        }
    }

    public void setDirection(Direction d)
    {
        if (d == Direction.RIGHT)
        {
            arrowVec = new Vector3(1, 0, 0);
        }
        else if (d == Direction.LEFT)
        {
            transform.Rotate(0, 180, 0);
            arrowVec = new Vector3(-1, 0, 0);
        }
        else if (d == Direction.UP)
        {
            transform.Rotate(0, 270, 0);
            arrowVec = new Vector3(0, 0, 1);
        }
        else if (d == Direction.DOWN)
        {
            transform.Rotate(0, 90, 0);
            arrowVec = new Vector3(0, 0, -1);
        }
    }

    public void setLevelCreator(GameObject c) { cl = c; }
}
