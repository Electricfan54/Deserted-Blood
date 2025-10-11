using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour
{
    [SerializeField]
    UnityEvent eventToTrigger;
    public bool triggered = false;
    public bool canRetrigger = false;
    private void Awake()
    {
        triggered = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered && !canRetrigger)
            return;
        triggered = true;
        eventToTrigger?.Invoke();
    }
}
