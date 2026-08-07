using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;

public class TriggerMovePlane : MonoBehaviour
{
    public GameObject plane;
    public float moveY = 2f;
    public float speed = 15f;

    public Canvas gameOverCanvas;       // drag your canvas here

    bool triggered = false;
    bool reachedPeak = false;
    Vector3 targetPos;

    FirstPersonController player;

    void Start()
    {
        targetPos = plane.transform.position;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.GetComponent<FirstPersonController>();

        if (gameOverCanvas != null)
            gameOverCanvas.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            targetPos = plane.transform.position + Vector3.up * moveY;
            triggered = true;
        }
    }

    void Update()
    {
        if (!triggered || reachedPeak) return;

        plane.transform.position = Vector3.MoveTowards(plane.transform.position, targetPos, speed * Time.deltaTime);

        if (Vector3.Distance(plane.transform.position, targetPos) < 0.01f)
        {
            reachedPeak = true;
            GameOver();
        }
    }

    void GameOver()
    {
        // Show canvas
        if (gameOverCanvas != null)
            gameOverCanvas.gameObject.SetActive(true);

        // Free mouse
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Disable player
        if (player != null)
            player.enabled = false;
    }
}