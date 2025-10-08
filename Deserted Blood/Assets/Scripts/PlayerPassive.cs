using UnityEngine;


public class PlayerPassive : MonoBehaviour
{
    public static PlayerPassive Instance;

    public int SpecialEnemyCount = 1;

    [SerializeField] int SpecialEnemyRequirement = 5;
    [SerializeField] MileStoneClass GolemMilestone;
    [SerializeField] MileStoneClass CerberusMilestone;
    [SerializeField] MileStoneClass FlyingMilestone;



    private void Awake()
    {
        Instance = this;

    }

    private void Start()
    {


    }
    void Update()
    {
        
    }

    public void AddToMilestone(EnemyAI.EnemyType type, bool isSpecial)
    {
        switch(type)
        {
            case EnemyAI.EnemyType.none:
                
                break;
            case EnemyAI.EnemyType.golem:
                GolemMilestone.MilestoneCount += 1;
                break;
            case EnemyAI.EnemyType.cerberus:
                CerberusMilestone.MilestoneCount += 1;
                break;
            case EnemyAI.EnemyType.flying:
                FlyingMilestone.MilestoneCount += 1;
                break;
        }

        if(isSpecial)
        {
            SpecialEnemyCount += 1;
        }

        CheckMileStone();
    }

    void CheckMileStone()
    { 
        if(GolemMilestone.MilestoneCount >= GolemMilestone.MilestoneAmountNeeded[GolemMilestone.MilestoneIndex])
        {
            GolemMilestone.MilestoneIndex += 1;
            // add buffs
        }

        if(CerberusMilestone.MilestoneCount >= CerberusMilestone.MilestoneAmountNeeded[CerberusMilestone.MilestoneIndex])
        {
            CerberusMilestone.MilestoneIndex += 1;
        }

        if(FlyingMilestone.MilestoneCount >= FlyingMilestone.MilestoneAmountNeeded[FlyingMilestone.MilestoneIndex])
        {
            FlyingMilestone.MilestoneIndex += 1;
        }

        if(SpecialEnemyCount >= SpecialEnemyRequirement)
        { 
            gameManager.instance.player.GetComponent<PlayerController>().hasThirdAbility = true;
        }
    }

}

[CreateAssetMenu]
public class MileStoneClass : ScriptableObject
{
    public int[] MilestoneAmountNeeded;
    public int MilestoneIndex;
    public int MilestoneCount;
}
