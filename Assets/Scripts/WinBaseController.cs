using UnityEngine;

public class WinBaseController : MonoBehaviour
{
    public bool victoryActive = true;
    public Renderer platformRenderer;
    public Color activeColor = Color.green;
    public Color inactiveColor = Color.red;

    private void Start()
    {
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
            Debug.Log("TOCÓ EL PLAYER Y LA VICTORIA ESTÁ ACTIVADA");
        }
    }
}
