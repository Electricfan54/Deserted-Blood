using UnityEngine;

public class gameManager : MonoBehaviour
{

    public static gameManager instance;

    public GameObject player;

    private void Awake()
    {

        instance = this;

        player = GameObject.FindWithTag("Player");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
