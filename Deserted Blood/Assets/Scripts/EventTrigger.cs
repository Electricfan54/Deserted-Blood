using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour
{
    [SerializeField]
    UnityEvent eventToTrigger;
    public bool triggered = false;
    private void Awake()
    {
        triggered = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
            return;
        eventToTrigger?.Invoke();
    }
}
