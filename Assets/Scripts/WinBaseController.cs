using UnityEngine;

public class WinBaseController : MonoBehaviour
{
    public bool victoryActive = true;
    public Renderer platformRenderer;
    public Color activeColor = Color.green;
    public Color inactiveColor = Color.red;

    private GameManager gameManager;

    [Header("Particles")]
    public GameObject winParticles;

    private void Start()
    {
        GameObject managerObject = GameObject.FindGameObjectWithTag("GameManager");
        if (managerObject != null)
        {
            gameManager = managerObject.GetComponent<GameManager>();
        }

        UpdateColor();
    }

    public void SetVictoryActive(bool active)
    {
        victoryActive = active;
        UpdateColor();
    }

    private void UpdateColor()
    {
        if (platformRenderer != null)
            platformRenderer.material.color = victoryActive ? activeColor : inactiveColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!victoryActive) return;

        if (other.CompareTag("Player"))
        {
            if (gameManager != null)
            {
                gameManager.LevelCompleted();
                SetVictoryActive(false);

                if (winParticles != null)
                {
                    Vector3 spawnPosition = other.transform.position + Vector3.up * 1f;
                    GameObject particlesInstance = Instantiate(winParticles, spawnPosition, Quaternion.identity);

                    ParticleSystem ps = particlesInstance.GetComponent<ParticleSystem>();
                    if (ps != null)
                    {
                        ps.Play();
                        Destroy(particlesInstance, ps.main.duration + ps.main.startLifetime.constantMax);
                    }
                    else
                    {
                        ps = particlesInstance.GetComponentInChildren<ParticleSystem>();
                        if (ps != null)
                        {
                            ps.Play();
                            Destroy(particlesInstance, ps.main.duration + ps.main.startLifetime.constantMax);
                        }
                    }
                }
                GetComponent<Collider>().enabled = false;
            }
        }
    }
}
