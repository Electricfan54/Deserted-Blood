using UnityEngine;

public class tutoral : MonoBehaviour
{

    [SerializeField] GameObject TutorialText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (TutorialText != null)
        {
            TutorialText.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (TutorialText != null)
            TutorialText.SetActive(false);
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
