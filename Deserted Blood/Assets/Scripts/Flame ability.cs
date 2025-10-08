using UnityEngine;

public class Flameability : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] int abilityuses;
    public Damage dmg;
    private void Awake()
    {
        dmg = GetComponent<Damage>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
