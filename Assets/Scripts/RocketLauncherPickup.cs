using UnityEngine;

public class RocketLauncherPickup : MonoBehaviour
{
   private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();

        if (player != null)
        {
            player.hasRocketLauncher = true;

            player.rocketLauncher.gameObject.SetActive(true);

            Destroy(gameObject);
        }
    }
}
