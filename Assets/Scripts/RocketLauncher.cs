using UnityEngine;

public class RocketLauncher : MonoBehaviour
{
    public GameObject missilePrefab;
    public Transform firePoint; //spawnpoint
    public Camera playerCamera;

    public void Shoot()
    {
        // Ray desde el centro de la pantalla
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(1000f);
        }

        Vector3 direction = (targetPoint - firePoint.position).normalized;

        GameObject missile = Instantiate(missilePrefab, firePoint.position, Quaternion.identity);

        // Inicializa dirección del misil
        missile.GetComponent<Missile>().Init(direction);
    }

}
