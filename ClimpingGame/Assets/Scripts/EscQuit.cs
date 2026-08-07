using UnityEngine;
using TMPro;


public class EscQuitUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The TextMeshPro UI text that shows the hold-to-quit message.")]
    public TMP_Text messageText;

    [Header("Timing")]
    [Tooltip("How long the message stays visible after pressing Esc (if not held to quit).")]
    public float displayDuration = 5f;

    [Tooltip("How long Esc must be held down to quit the game.")]
    public float holdToQuitDuration = 2f;

    [Header("Message")]
    public string promptMessage = "Hold ESC to quit...";

    private float displayTimer = 0f;   // counts down while message is showing
    private float holdTimer = 0f;      // counts up while Esc is held
    private bool isShowingMessage = false;

    void Start()
    {
        if (messageText != null)
            messageText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowMessage();
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            holdTimer += Time.deltaTime;

            if (holdTimer >= holdToQuitDuration)
            {
                QuitGame();
                return; 
            }
        }

        // reset hold timer after release
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            holdTimer = 0f;
        }

        // Count down the display timer independently of whether the key is held
        if (isShowingMessage)
        {
            displayTimer -= Time.deltaTime;
            if (displayTimer <= 0f)
            {
                HideMessage();
            }
        }
    }

    private void ShowMessage()
    {
        isShowingMessage = true;
        displayTimer = displayDuration;
        holdTimer = 0f;

        if (messageText != null)
        {
            messageText.text = promptMessage;
            messageText.gameObject.SetActive(true);
        }
    }

    private void HideMessage()
    {
        isShowingMessage = false;

        if (messageText != null)
            messageText.gameObject.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game (Esc held).");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}