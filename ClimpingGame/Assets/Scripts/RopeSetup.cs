using UnityEngine;

public class RopeSetup : MonoBehaviour
{
    public float ropeLength = 10f;

    public Transform topPoint;
    public Transform bottomPoint;
    public Transform ropeMesh; // the Cylinder child
    public Transform colliderTarget; // empty child GameObject with the CapsuleCollider on it

    void OnValidate()
    {
        if (topPoint != null)
            topPoint.localPosition = new Vector3(0, -0.4f, 0);

        if (bottomPoint != null)
            bottomPoint.localPosition = new Vector3(0, -ropeLength + 0.4f, 0);

        if (ropeMesh != null)
        {
            ropeMesh.localPosition = new Vector3(0, -ropeLength / 2f, 0);
            ropeMesh.localScale = new Vector3(ropeMesh.localScale.x, ropeLength / 2f, ropeMesh.localScale.z);
        }

        if (colliderTarget != null)
        {
            // Move the child to the center of the rope
            colliderTarget.localPosition = new Vector3(0, -ropeLength / 2f, 0);

            // Collider center stays at 0, height matches rope
            CapsuleCollider col = colliderTarget.GetComponent<CapsuleCollider>();
            if (col != null)
            {
                col.center = Vector3.zero;
                col.height = ropeLength;
            }
        }
    }
}