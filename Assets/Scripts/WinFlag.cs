using UnityEngine;

public class WinFlag : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<PlayerHandler>(out PlayerHandler player))
        {
            GameManager.Won();
        }
    }
}
