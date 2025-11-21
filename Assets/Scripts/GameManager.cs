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

    [Header("End UI")]
    public TMP_Text finalTimeText;
    public GameObject finalBackground;
    public GameObject nextLevelButton;

    [Header("Pause Menu UI")]
    public GameObject pauseMenu;
    private bool isPaused = false;



    private float runTimer = 0f;

    [Header("Player")]
    public PlayerController player;

    [Header("Win Platform")]
    public WinBaseController winPlatform;

    // Enemigos
    private GameObject[] enemies;




    private void Start()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        StartCoroutine(StartCountdown());
        crosshair.SetActive(false);
        finalTimeText.gameObject.SetActive(false);
        finalBackground.SetActive(false);
        nextLevelButton.SetActive(false);
        pauseMenu.SetActive(false);


    // win condition
        if (winPlatform != null)
        {
            if (enemies.Length == 0)
            {
                winPlatform.SetVictoryActive(true);
            }
            else
            {
                winPlatform.SetVictoryActive(false);
            }
        }

    }
    private void Update()
    {
        // Reinicio con R
        if (Input.GetKeyDown(KeyCode.R))
        {
            RetryLevel();
        }

        // Pause
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }

        if (gameStarted)
        {
            runTimer += Time.deltaTime;
            runTimerText.text = FormatTime(runTimer);
        }

    }

    private void LateUpdate()
    {
        if (!gameStarted) return;

        if (enemies.Length > 0 && AllEnemiesDefeated())
        {
            ActivateWinPlatform();
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
        Time.timeScale = 1f;
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

        // Mostrar final
        finalTimeText.gameObject.SetActive(true);
        finalTimeText.text = FormatTime(runTimer);
        finalBackground.SetActive(true);
        nextLevelButton.SetActive(true);


        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    // -------------------- NEXT --------------------

    public void NextLevel()
    {
        string[] levels = new string[]
        {
        "SampleScene",
        "Level1",
        "Level2",
        "Level3",
        };

        string current = SceneManager.GetActiveScene().name;
        int index = System.Array.IndexOf(levels, current);

        // Si hay siguiente nivel
        if (index + 1 < levels.Length)
        {
            SceneManager.LoadScene(levels[index + 1]);
        }
        else
        {
            Debug.Log("No hay más niveles. Fin del juego.");
        }
    }

    // -------------------- PAUSE MENU --------------------
    public void PauseGame()
    {
        isPaused = true;

        pauseMenu.SetActive(true);

        Time.timeScale = 0f;

        if (player != null)
        {
            player.SetMovementEnabled(false);
            player.SetLookEnabled(false);
        }

        finalBackground.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;

        pauseMenu.SetActive(false);
        finalBackground.SetActive(false);

        Time.timeScale = 1f;

        if (player != null)
        {
            player.SetMovementEnabled(true);
            player.SetLookEnabled(true);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }


    // -------------------- ENEMIES --------------------
    private bool AllEnemiesDefeated()
    {
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null) return false;
        }
        return true;
    }

    private void ActivateWinPlatform()
    {
        if (winPlatform != null)
        {
            winPlatform.SetVictoryActive(true);
        }
    }
}
