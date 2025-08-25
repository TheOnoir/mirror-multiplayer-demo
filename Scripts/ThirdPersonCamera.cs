using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 3, -6);
    [SerializeField] private float smoothSpeed = 10f;

    void LateUpdate()
    {
        if (target == null) return;

        // calculating the position based on the player's rotation 
        Vector3 desiredPosition = target.position + target.TransformDirection(offset);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
