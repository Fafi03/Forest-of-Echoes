using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MemoryCluePickup : MonoBehaviour
{
    [Header("Puzzle UI")]
    public GameObject puzzleOverlay;
    public TMP_Text feedbackText;
    public Button[] runeButtons;   // size 4

    [Header("Flash Settings")]
    public float flashOnSeconds = 0.55f;
    public float flashOffSeconds = 0.18f;
    public float flashScale = 1.25f;
    public Color flashColor = Color.yellow;

    [Header("After Attempt")]
    public float afterAttemptPause = 0.9f;

    [Header("Letter UI After Success")]
    public GameObject letterOverlay;
    public TMP_Text letterText;
    public Button letterNextButton;   // <-- drag your Next button here

    [TextArea(4, 10)]
    public string successLetterMessage =
@"Sarah,

The forest remembers patterns —
and now so do you.

This fragment is yours.";

    private bool triggered = false;
    private bool solved = false;
    private bool showingSequence = false;

    private PlayerController2D cachedPlayer;
    private Collider2D clueCollider;
    private SpriteRenderer clueSprite;

    private List<int> pattern = new List<int>();
    private List<int> attempt = new List<int>();

    private Vector3[] originalScales;
    private Color[] originalColors;

    void Awake()
    {
        clueCollider = GetComponent<Collider2D>();
        clueSprite = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (puzzleOverlay != null) puzzleOverlay.SetActive(false);
        if (letterOverlay != null) letterOverlay.SetActive(false);
        if (feedbackText != null) feedbackText.text = "";

        CacheRuneOriginals();
        SetupButtons();

        // Hook the Next button from code (so inspector OnClick can be empty)
        if (letterNextButton != null)
        {
            letterNextButton.onClick.RemoveAllListeners();
            letterNextButton.onClick.AddListener(GoNextLevel);
        }
        else
        {
            Debug.LogWarning("Letter Next Button is NOT assigned in MemoryCluePickup.");
        }
    }

    private void CacheRuneOriginals()
    {
        if (runeButtons == null) return;

        originalScales = new Vector3[runeButtons.Length];
        originalColors = new Color[runeButtons.Length];

        for (int i = 0; i < runeButtons.Length; i++)
        {
            if (runeButtons[i] == null) continue;

            originalScales[i] = runeButtons[i].transform.localScale;
            Image img = runeButtons[i].GetComponent<Image>();
            originalColors[i] = (img != null) ? img.color : Color.white;
        }
    }

    private void SetupButtons()
    {
        if (runeButtons == null) return;

        for (int i = 0; i < runeButtons.Length; i++)
        {
            int idx = i;
            if (runeButtons[i] == null) continue;

            runeButtons[i].onClick.RemoveAllListeners();
            runeButtons[i].onClick.AddListener(() => OnRuneClicked(idx));
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered || solved) return;
        if (!other.CompareTag("Player")) return;

        if (runeButtons == null || runeButtons.Length < 4)
        {
            Debug.LogError("Rune Buttons must be size 4 and assigned.");
            return;
        }

        triggered = true;

        cachedPlayer = other.GetComponent<PlayerController2D>();
        if (cachedPlayer != null) cachedPlayer.enabled = false;

        if (clueCollider != null) clueCollider.enabled = false;
        if (clueSprite != null) clueSprite.enabled = false;

        attempt.Clear();

        if (feedbackText != null) feedbackText.text = "";
        if (puzzleOverlay != null) puzzleOverlay.SetActive(true);

        Time.timeScale = 0f;

        StartNewRound();
    }

    private void StartNewRound()
    {
        attempt.Clear();
        GenerateRandomPattern4();

        StopAllCoroutines();
        StartCoroutine(FlashPattern());
    }

    private void GenerateRandomPattern4()
    {
        pattern.Clear();
        pattern.Add(0); pattern.Add(1); pattern.Add(2); pattern.Add(3);

        for (int i = pattern.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = pattern[i];
            pattern[i] = pattern[j];
            pattern[j] = temp;
        }
    }

    private IEnumerator FlashPattern()
    {
        showingSequence = true;
        if (feedbackText != null) feedbackText.text = "Watch the runes...";
        yield return new WaitForSecondsRealtime(0.6f);

        ResetAllRunes();

        foreach (int idx in pattern)
        {
            FlashRune(idx, true);
            yield return new WaitForSecondsRealtime(flashOnSeconds);

            FlashRune(idx, false);
            yield return new WaitForSecondsRealtime(flashOffSeconds);
        }

        if (feedbackText != null) feedbackText.text = "Your turn.";
        showingSequence = false;
    }

    private void OnRuneClicked(int idx)
    {
        if (solved) return;
        if (showingSequence) return;

        attempt.Add(idx);

        if (attempt.Count >= 4)
            StartCoroutine(EvaluateAttempt());
    }

    private IEnumerator EvaluateAttempt()
    {
        showingSequence = true;

        bool correct = true;
        for (int i = 0; i < 4; i++)
        {
            if (attempt[i] != pattern[i]) { correct = false; break; }
        }

        if (correct)
        {
            if (feedbackText != null) feedbackText.text = "Correct!";
            yield return new WaitForSecondsRealtime(0.25f);
            SolvePuzzle();
        }
        else
        {
            if (feedbackText != null) feedbackText.text = "Wrong answer. Try again.";
            yield return new WaitForSecondsRealtime(afterAttemptPause);

            showingSequence = false;
            StartNewRound();
        }
    }

    private void FlashRune(int idx, bool on)
    {
        if (runeButtons == null || idx < 0 || idx >= runeButtons.Length) return;
        if (runeButtons[idx] == null) return;

        runeButtons[idx].transform.localScale = on ? (originalScales[idx] * flashScale) : originalScales[idx];

        Image img = runeButtons[idx].GetComponent<Image>();
        if (img != null) img.color = on ? flashColor : originalColors[idx];
    }

    private void ResetAllRunes()
    {
        if (runeButtons == null) return;

        for (int i = 0; i < runeButtons.Length; i++)
        {
            if (runeButtons[i] == null) continue;

            runeButtons[i].transform.localScale = originalScales[i];

            Image img = runeButtons[i].GetComponent<Image>();
            if (img != null) img.color = originalColors[i];
        }
    }

    private void SolvePuzzle()
    {
        solved = true;

        if (puzzleOverlay != null) puzzleOverlay.SetActive(false);

        if (letterOverlay != null) letterOverlay.SetActive(true);
        if (letterText != null) letterText.text = successLetterMessage;

        // still paused until Next
    }

    public void GoNextLevel()
    {
        Debug.Log("NEXT button pressed — trying to load next scene...");

        Time.timeScale = 1f;

        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning("No next scene in Build Settings. Add Level 5 and make sure order is correct.");
            return;
        }

        SceneManager.LoadScene(nextIndex);
    }
}
