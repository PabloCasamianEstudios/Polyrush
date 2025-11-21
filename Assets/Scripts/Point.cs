using UnityEngine;

public class Point : MonoBehaviour
{
    public int pointValue = 1;
    public float floatAmplitude = 0.5f;
    public float floatFrequency = 2f;
    private Vector3 startPos;


    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        transform.position = startPos + Vector3.up * Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.Rotate(Vector3.up, 100f * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.AddPoints(pointValue);
            }

            Destroy(gameObject);
        }
    }
}
