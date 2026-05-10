using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerHandler p = other.GetComponent<PlayerHandler>();
        if (p != null)
        {
            p.addCoin();
            HUDManager.Instance?.AddCoin();
            Destroy(this.gameObject);
        }
    }
}
