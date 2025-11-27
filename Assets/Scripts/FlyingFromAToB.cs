using UnityEngine;

public class FlyingFromAToB : MonoBehaviour
{

    // SI NO PONGO GRAVITY DA SALTITOS (NUEVO ENEMIGO?)
    [Header("A to B")]
    public Transform pointA;
    public Transform pointB;

    [Header("Settings")]
    public float speed = 3f;
    public float tolerance = 0.1f;

    private Transform target;

    void Start()
    {
        target = pointA;
    }

    void Update()
    {
        if (pointA == null || pointB == null) return;

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < tolerance)
        {
            target = (target == pointA) ? pointB : pointA;
        }
    }
}
