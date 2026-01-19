using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RiddleCluePickup : MonoBehaviour
{
    [Header("Riddle UI References")]
    public GameObject riddleOverlay;      // Drag RiddleOverlay here
    public TMP_Text riddleText;           // Drag RiddleQuestionText here
    public TMP_Text feedbackText;         // Drag FeedbackText here (above question)
    public TMP_InputField answerInput;    // Drag TMP InputField here
    public Button submitButton;           // Drag Submit button here

    [Header("Riddle Content")]
    [TextArea(2, 6)]
    public string riddleQuestion =
        "I speak without a mouth and hear without ears. I have no body, but I come alive with wind. What am I?";
    public string correctAnswer = "echo";

    [Header("After Correct Answer - Letter Card")]
    public GameObject letterOverlay;      // Drag your Letter Popup Overlay here
    public TMP_Text letterMessageText;    // Drag the TMP text inside the letter card here

    [TextArea(6, 15)]
    public string letterMessage =
@"Sarah,

You crossed the cliff — and the forest noticed.

A true signal doesn’t shout.
It waits to be understood.

Take the clue.
Keep going.";

    private PlayerController2D cachedController;
    private bool triggered = false;
    private bool clueSolved = false;

    private void Start()
    {
        // Hide UIs at start
        if (riddleOverlay != null) riddleOverlay.SetActive(false);
        if (letterOverlay != null) letterOverlay.SetActive(false);

        // Hook Submit button
        if (submitButton != null)
        {
            submitButton.onClick.RemoveAllListeners();
            submitButton.onClick.AddListener(CheckAnswer);
        }

        // Clear feedback when the player types (but NOT when we clear input in code)
        if (answerInput != null)
        {
            answerInput.onValueChanged.RemoveAllListeners();
            answerInput.onValueChanged.AddListener(_ =>
            {
                if (feedbackText != null) feedbackText.text = "";
            });
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered || clueSolved) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;
        cachedController = other.GetComponent<PlayerController2D>();

        // Show riddle UI
        if (riddleOverlay != null) riddleOverlay.SetActive(true);
        if (riddleText != null) riddleText.text = riddleQuestion;

        if (feedbackText != null)
        {
            feedbackText.gameObject.SetActive(true);
            feedbackText.text = "";
        }

        if (answerInput != null)
        {
            answerInput.SetTextWithoutNotify("");
            answerInput.ActivateInputField();
        }

        // Pause gameplay while answering
        Time.timeScale = 0f;
        if (cachedController != null) cachedController.enabled = false;
    }

    private void CheckAnswer()
    {
        if (answerInput == null || feedbackText == null) return;

        string user = answerInput.text.Trim().ToLowerInvariant();
        string correct = (correctAnswer ?? "").Trim().ToLowerInvariant();

        // If correct answer not set
        if (string.IsNullOrWhiteSpace(correct))
        {
            feedbackText.text = "ERROR: Correct answer is not set!";
            Debug.LogError("CorrectAnswer is EMPTY. Set it in Inspector!");
            return;
        }

        // Empty input
        if (string.IsNullOrWhiteSpace(user))
        {
            feedbackText.text = "Please enter an answer.";
            answerInput.ActivateInputField();
            return;
        }

        // Correct answer
        if (user == correct)
        {
            clueSolved = true;

            // Hide riddle UI
            if (riddleOverlay != null) riddleOverlay.SetActive(false);

            // Show letter UI
            if (letterOverlay != null) letterOverlay.SetActive(true);
            if (letterMessageText != null) letterMessageText.text = letterMessage;

            // Collect clue object (so it can't trigger again)
            gameObject.SetActive(false);

            // Stay paused until user closes letter (button calls CloseLetter)
            return;
        }

        // Wrong answer
        feedbackText.text = "Wrong answer. Try again.";

        // Clear input WITHOUT firing onValueChanged (so feedback doesn't disappear)
        answerInput.SetTextWithoutNotify("");
        answerInput.ActivateInputField();
    }

    // Hook your Letter OK/Next button to this method
    public void CloseLetter()
    {
        if (letterOverlay != null) letterOverlay.SetActive(false);

        // Resume game
        Time.timeScale = 1f;
        if (cachedController != null) cachedController.enabled = true;
    }
}
