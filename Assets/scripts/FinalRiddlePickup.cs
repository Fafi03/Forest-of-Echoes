using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class FinalRiddlePickup : MonoBehaviour
{
    [Header("Riddle UI")]
    public GameObject riddleOverlay;
    public TMP_Text riddleText;
    public TMP_InputField answerInput;
    public Button submitButton;
    public TMP_Text feedbackText;

    [Header("Letter UI After Correct")]
    public GameObject letterOverlay;
    public TMP_Text letterText;
    public Button letterOkButton;

    [Header("Correct Answer Settings")]
    public string correctAnswer = "signal";
    public bool acceptTheSignal = true;

    [Header("Background Change (World Sprite)")]
    public SpriteRenderer backgroundRenderer;   // Drag BACKGROUND OBJECT from HierARCHY
    public Sprite solvedBackground;             // Drag SPRITE from Assets

    [Header("Next Scene (Optional)")]
    public bool loadNextSceneOnOk = false;

    [TextArea(6, 14)]
    public string finalRiddle =
@"I am born when the world stands still,
Yet I die when the wind runs free.
The tower holds me in silence,
But the forest sets me free.

What am I?";

    [TextArea(6, 14)]
    public string finalLetter =
@"Sarah,

You didn’t just chase the signal —
you understood it.

The tower will answer now.

Whatever comes next…
remember: this was only the beginning.";

    private bool triggered = false;
    private bool solved = false;

    private PlayerController2D cachedPlayer;
    private Collider2D clueCollider;
    private SpriteRenderer clueSprite;

    void Awake()
    {
        clueCollider = GetComponent<Collider2D>();
        clueSprite = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (riddleOverlay != null) riddleOverlay.SetActive(false);
        if (letterOverlay != null) letterOverlay.SetActive(false);

        if (riddleText != null) riddleText.text = finalRiddle;
        if (feedbackText != null) feedbackText.text = "";

        if (submitButton != null)
        {
            submitButton.onClick.RemoveAllListeners();
            submitButton.onClick.AddListener(CheckAnswer);
        }

        if (letterOkButton != null)
        {
            letterOkButton.onClick.RemoveAllListeners();
            letterOkButton.onClick.AddListener(OnFinalOk);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered || solved) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        cachedPlayer = other.GetComponent<PlayerController2D>();
        if (cachedPlayer != null)
            cachedPlayer.enabled = false;

        if (clueCollider != null)
            clueCollider.enabled = false;

        if (clueSprite != null)
            clueSprite.enabled = false;

        if (riddleOverlay != null)
            riddleOverlay.SetActive(true);

        if (answerInput != null)
        {
            answerInput.text = "";
            answerInput.ActivateInputField();
        }

        if (feedbackText != null)
            feedbackText.text = "";

        Time.timeScale = 0f;
    }

    private void CheckAnswer()
    {
        if (solved) return;

        string user = Normalize(answerInput != null ? answerInput.text : "");
        bool ok = user == Normalize(correctAnswer);

        if (!ok && acceptTheSignal)
            ok = user == "the signal";

        if (ok)
        {
            solved = true;

            if (riddleOverlay != null)
                riddleOverlay.SetActive(false);

            // 🌲 CHANGE BACKGROUND
            if (backgroundRenderer != null && solvedBackground != null)
                backgroundRenderer.sprite = solvedBackground;

            if (letterOverlay != null)
                letterOverlay.SetActive(true);

            if (letterText != null)
                letterText.text = finalLetter;
        }
        else
        {
            if (feedbackText != null)
                feedbackText.text = "Wrong answer. Try again.";

            if (answerInput != null)
            {
                answerInput.text = "";
                answerInput.ActivateInputField();
            }
        }
    }

    private string Normalize(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return "";
        s = s.Trim().ToLowerInvariant();
        s = s.Replace(".", "").Replace(",", "").Replace("’", "'");
        return s;
    }

    private void OnFinalOk()
    {
        if (letterOverlay != null)
            letterOverlay.SetActive(false);

        Time.timeScale = 1f;

        if (cachedPlayer != null)
            cachedPlayer.enabled = true;

        gameObject.SetActive(false);

        if (loadNextSceneOnOk)
        {
            int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
            if (nextIndex < SceneManager.sceneCountInBuildSettings)
                SceneManager.LoadScene(nextIndex);
        }
    }
}
