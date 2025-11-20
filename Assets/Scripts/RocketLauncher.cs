using UnityEngine;

public class RocketLauncher : MonoBehaviour
{
    public GameObject missilePrefab;
    public Transform firePoint; //spawnpoint
    public Camera playerCamera;

    public void Shoot()
    {
        Ray ray = playerCamera.ScreenPointToRay(
            new Vector3(Screen.width / 2f, Screen.height / 2f, 0f)
        );

        Vector3 targetPoint;

        // Raycast ignorando al propio jugador
        if (Physics.Raycast(ray, out RaycastHit hit, 2000f, ~LayerMask.GetMask("Player")))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(2000f);
        }

        // DIRECCIÓN SOLO BASADA EN LA CÁMARA
        Vector3 direction = (targetPoint - playerCamera.transform.position).normalized;

        GameObject missile = Instantiate(missilePrefab, firePoint.position, Quaternion.LookRotation(direction));

        missile.GetComponent<Missile>().Init(direction);
    }

}
