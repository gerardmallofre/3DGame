using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionHandler : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float duration = 0.5f;
    float time = 0;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time > duration)
        {
            Destroy(this.gameObject);
        }
    }

    public void setRadius(float r)
    {
        transform.localScale = new Vector3(r, r, r);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerHandler>().takeDamage(1, Direction.NONE);
        }
        else
        {
            IEnemy ie = other.GetComponent<IEnemy>();
            if (ie != null)
            {
                ie.die(Direction.NONE);
            }
        }
    }
}
