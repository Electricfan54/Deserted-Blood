using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform Target;
    [SerializeField] float camSmoothSpeed;

    Vector3 offset = new Vector3(0, 1.2f, -6.7f);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void LateUpdate()
    {
        if (!Target) return;

        Vector3 MovePosition = Target.position + offset;
        transform.position = Vector3.Lerp(transform.position, MovePosition, camSmoothSpeed * Time.deltaTime);

        transform.LookAt(Target);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
