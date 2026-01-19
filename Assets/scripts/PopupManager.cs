using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    public GameObject popupOverlay;
    public TMP_Text messageText;
    public Button okButton;

    private void Awake()
    {
        // Ensure hidden at start
        popupOverlay.SetActive(false);

        okButton.onClick.RemoveAllListeners();
        okButton.onClick.AddListener(ClosePopup);
    }

    public void ShowPopup(string msg)
    {
        messageText.text = msg;
        popupOverlay.SetActive(true);

        // pause the game so Sarah can't move while reading
        Time.timeScale = 0f;
    }

    public void ClosePopup()
    {
        popupOverlay.SetActive(false);
        Time.timeScale = 1f;
    }
}
