using UnityEngine;

public class TutorialSign : MonoBehaviour
{
    public GameObject panelToShow;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Show();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Hide();
        }
    }

    public void Show()
    {
        panelToShow.SetActive(true);
    }

    public void Hide()
    {
        panelToShow.SetActive(false);
    }
}
