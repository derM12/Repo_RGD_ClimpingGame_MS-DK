using UnityEngine;

public class DeleteOnTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickaxe"))
        {
            AudioManager.Instance?.PlayRockSmash();
            Destroy(gameObject);
        }
    }
}