using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CluePickup : MonoBehaviour
{
    [Header("Popup UI References")]
    public GameObject popupOverlay;   // Drag PopupOverlay here
    public TMP_Text messageText;      // Drag MessageText here
    public Button okButton;           // Drag OkButton here (NEXT button)

    private PlayerController2D cachedController;

    [TextArea(6, 15)]
    public string clueMessage =
@"Sarah,

The signal isn’t lost — it’s broken.

The transmitter pieces are scattered across the Whispering Forest.
Find them. Carry them to the tower.

The glowing runes will guide you.

This is only the beginning.";

    private void Start()
    {
        // Make sure popup is hidden at start
        if (popupOverlay != null)
            popupOverlay.SetActive(false);

        // Hook button click
        if (okButton != null)
        {
            okButton.onClick.RemoveAllListeners();
            okButton.onClick.AddListener(OnNextPressed);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Cache Sarah's movement script
        cachedController = other.GetComponent<PlayerController2D>();

        // Show popup + message
        if (popupOverlay != null)
            popupOverlay.SetActive(true);

        if (messageText != null)
            messageText.text = clueMessage;

        // Pause game
        Time.timeScale = 0f;

        // Disable movement
        if (cachedController != null)
            cachedController.enabled = false;

        // Hide the clue object
        gameObject.SetActive(false);
    }

    private void OnNextPressed()
    {
        // Resume time before loading next scene
        Time.timeScale = 1f;

        // Load next level (must be in Build Settings)
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex + 1);
    }
}
