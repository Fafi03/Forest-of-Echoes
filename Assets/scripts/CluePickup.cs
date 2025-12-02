using UnityEngine;

public class CluePickup : MonoBehaviour
{
    // Drag the LevelCompleteText object here in the Inspector
    public GameObject levelCompleteUI;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only react if the player touches the clue
        if (other.CompareTag("Player"))
        {
            // Show the "Level Complete" text
            if (levelCompleteUI != null)
            {
                levelCompleteUI.SetActive(true);
            }

            // Optionally stop player movement after finishing
            PlayerController2D controller = other.GetComponent<PlayerController2D>();
            if (controller != null)
            {
                controller.enabled = false;
            }

            // Hide the clue so it looks collected
            gameObject.SetActive(false);
        }
    }
}
