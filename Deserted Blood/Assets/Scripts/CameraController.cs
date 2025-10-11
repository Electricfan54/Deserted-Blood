using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform Target;
    [SerializeField] float camSmoothSpeed;
    [SerializeField] Vector3 camDistance;

    Vector3 offset = new Vector3(0, 1.2f, -6.7f);
    Vector3 CurrentView = new Vector3(0, 1.2f, -6.7f);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    void LateUpdate()
    {
        if (!Target) return;

        Vector3 MovePosition = Target.position + CurrentView;
        transform.position = Vector3.Lerp(transform.position, MovePosition, camSmoothSpeed * Time.deltaTime);

        transform.LookAt(Target);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetBossDistance(Vector3 distance)
    {


        CurrentView = distance;
    }

    public void resetCam()
    {
        CurrentView = offset;
    }


}
