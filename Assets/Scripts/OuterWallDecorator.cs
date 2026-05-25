using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OuterWallDecorator : MonoBehaviour
{
    [SerializeField] GameObject decoration;
    bool isLeftWall=false;

    public void decorate()
    {
        int num = Random.Range(0, 4);
        for (int i = 0; i < num; ++i)
        {
            GameObject obj;
            if (isLeftWall)
            {
                obj = Instantiate(decoration, new Vector3(transform.position.x + 0.5f, Random.Range(0.1f, 1.9f), transform.position.z + Random.Range(-0.35f, 0.35f)), transform.rotation);
            }
            else {
                obj = Instantiate(decoration, new Vector3(transform.position.x + Random.Range(-0.35f, 0.35f), Random.Range(0.1f, 1.9f), transform.position.z - Random.Range(0.45f, 0.55f)), transform.rotation);
            }
            obj.transform.parent = transform;
        }
    }

    public void setLeftWall(bool b)
    {
        isLeftWall = b;
    }
}
