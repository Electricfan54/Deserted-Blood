using TMPro;
using UnityEngine;

public class UnityChanEasterEgg : MonoBehaviour
{
    [SerializeField] GameObject Text;
    [SerializeField] Animator UnityChanAnimator;
    bool CheckForInput = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(CheckForInput)
        {
            Text.SetActive(true);
            if(Input.GetKeyDown(KeyCode.E))
            {
                UnityChanAnimator.SetBool("interacted", true);
                Text.GetComponentInChildren<TextMeshProUGUI>().text = "Unity-Chan: I'm your biggest fan, go kick that orc king in the face!";
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            CheckForInput = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            CheckForInput = false;
            Text.SetActive(false);
            Text.GetComponentInChildren<TextMeshProUGUI>().text = "Press E to interact";
            UnityChanAnimator.SetBool("interacted", false);

        }
    }
}
