using UnityEngine;

public class CoinHandler : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        PlayerHandler p = other.GetComponent<PlayerHandler>()
                       ?? other.GetComponentInParent<PlayerHandler>();
        if (p != null)
        {
            HUDManager.Instance?.AddCoin();
            Destroy(gameObject);
        }
    }
}