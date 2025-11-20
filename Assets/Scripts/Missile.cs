using UnityEngine;

public class Missile : MonoBehaviour
{
    public float speed = 20f;
    private Vector3 direction;

    public void Init(Vector3 dir)
    {
        direction = dir.normalized;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
}
