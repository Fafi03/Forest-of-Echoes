using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FallGameOver : MonoBehaviour
{
    public GameObject gameOverOverlay; // Drag GameOverOverlay here
    public Button retryButton;         // Drag RetryButton here

    private bool triggered = false;

    private void Start()
    {
        if (gameOverOverlay != null)
            gameOverOverlay.SetActive(false);

        if (retryButton != null)
        {
            retryButton.onClick.RemoveAllListeners();
            retryButton.onClick.AddListener(Retry);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        if (gameOverOverlay != null)
            gameOverOverlay.SetActive(true);
        // Don't pause time, so Sarah keeps falling
    }

    private void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
