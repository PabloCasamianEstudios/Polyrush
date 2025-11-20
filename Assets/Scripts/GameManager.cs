using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{

    [Header("Timing Settings")]
    public float startCountdown = 5f;
    public bool gameStarted = false;


    [Header("UI")]
    public TMP_Text countdownText;
    public TMP_Text runTimerText;
    public GameObject crosshair;
    public TMP_Text finalTimeText;

    private float runTimer = 0f;

    [Header("Player")]
    public PlayerController player;




    private void Start()
    {
        StartCoroutine(StartCountdown()); // el contador 
        crosshair.SetActive(false);
        finalTimeText.gameObject.SetActive(false);
    }
    private void Update()
    {
        // Reinicio con R
        if (Input.GetKeyDown(KeyCode.R))
        {
            RetryLevel();
        }

        if (gameStarted)
        {
            runTimer += Time.deltaTime;
            runTimerText.text = FormatTime(runTimer);
        }

    }

    // -------------------- CONTADOR --------------------
    private IEnumerator StartCountdown()
    {
        float countdown = startCountdown;

        while (countdown > 0)
        {
            countdownText.text = Mathf.Ceil(countdown).ToString();
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        countdownText.text = "GO!";
        yield return new WaitForSeconds(0.5f);
        countdownText.gameObject.SetActive(false);

        gameStarted = true; // al terminar empieza el cronómetro
        crosshair.SetActive(true);

        if (player != null)
        {
            player.SetMovementEnabled(true);
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 1000f) % 1000f);
        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }

    // -------------------- REPETIR --------------------
    public void RetryLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // -------------------- FIN DE JUEGO --------------------
    public void LevelCompleted()
    {
        if (!gameStarted) return;

        //  Detener crono
        gameStarted = false; 
        runTimerText.gameObject.SetActive(false);
        crosshair.SetActive(false);
        
        // Desactivar player
        if (player != null)
        {
            player.SetMovementEnabled(false);
            player.SetLookEnabled(false);
        }

        // Mostrar el tiempo
        finalTimeText.gameObject.SetActive(true);
        finalTimeText.text =  FormatTime(runTimer); 
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

}
