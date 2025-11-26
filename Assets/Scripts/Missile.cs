using UnityEngine;

public class Missile : MonoBehaviour
{
    public float speed = 20f;
    public float maxDistance = 80f;

    // Rocket jump
    public float explosionForce = 25f;
    public float explosionRadius = 6f;

    private Vector3 direction;
    private Vector3 startPosition;


    [Header("Explosion Particles")]
    public GameObject missileExplosion;

    public void Init(Vector3 dir)
    {
        direction = dir.normalized;
        transform.rotation = Quaternion.LookRotation(direction);

        startPosition = transform.position;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        // Autodestruir por distancia
        if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
        {
            Explode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
        }
        Explode();
    }

    void Explode()
    {
        if (missileExplosion != null)
        {
            GameObject explosion = Instantiate(missileExplosion, transform.position, Quaternion.identity);
            Destroy(explosion, 1f);
        }

        // Rocket Jump
        PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        if (player != null)
        {
            player.ApplyExplosionForce(transform.position, explosionForce, explosionRadius);
        }

        Destroy(gameObject);
    }
}
