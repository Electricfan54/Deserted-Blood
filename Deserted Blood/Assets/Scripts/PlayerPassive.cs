using UnityEngine;

[System.Serializable]
public struct GolemMilestones
{
    [SerializeField] public int[] MilestoneAmounts;
    public int MilestoneIndex;
}

public class PlayerPassive : MonoBehaviour
{
    public static PlayerPassive Instance;

    public int SpecialEnemyCount;
    public int GolemMilestoneCount;
    public int CerberusMilestoneCount;
    public int FlyingMilestoneCount;

    [SerializeField] int SpecialEnemyRequirement;

    [SerializeField] GolemMilestones GolemStruct;
    
    
    
    private void Awake()
    {
        Instance = this;

    }

    public void AddToMilestone(EnemyAI.EnemyType type, bool isSpecial)
    {
        switch(type)
        {
            case EnemyAI.EnemyType.none:
                
                break;
            case EnemyAI.EnemyType.golem:
                GolemMilestoneCount += 1;
                break;
            case EnemyAI.EnemyType.cerberus:
                CerberusMilestoneCount += 1;
                break;
            case EnemyAI.EnemyType.flying:
                FlyingMilestoneCount += 1;
                break;
        }

        if(isSpecial)
        {
            SpecialEnemyCount += 1;
        }
            
    }

    void CheckMileStone()
    {
        GolemStruct.MilestoneIndex = 1;
    }
   
}
