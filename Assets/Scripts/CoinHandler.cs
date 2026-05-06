using UnityEngine;

public class CoinHandler : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        PlayerHandler p = other.GetComponent<PlayerHandler>()
                       ?? other.GetComponentInParent<PlayerHandler>();
        if (p != null)
        {
            p.addCoin();
            HUDManager.Instance?.AddCoin();
            Destroy(gameObject);
        }
    }
}