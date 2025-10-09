using UnityEngine;


public class PlayerPassive : MonoBehaviour
{
    public static PlayerPassive Instance;

    public int SpecialEnemyCount = 1;

    [SerializeField] int SpecialEnemyRequirement = 5;
    [SerializeField] MilestoneFramework GolemMilestone;
    [SerializeField] MilestoneFramework CerberusMilestone;
    [SerializeField] MilestoneFramework FlyingMilestone;



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
        if(GolemMilestone.MilestoneCount == GolemMilestone.MilestoneAmountNeeded[GolemMilestone.MilestoneIndex])
        {
            if(GolemMilestone.MilestoneIndex != GolemMilestone.MilestoneAmountNeeded.Length - 1)
            {
                GolemMilestone.MilestoneIndex += 1;
            }

            gameManager.instance.player.GetComponent<PlayerController>().AddHPMilestone(20);

        }

        if(CerberusMilestone.MilestoneCount == CerberusMilestone.MilestoneAmountNeeded[CerberusMilestone.MilestoneIndex])
        {
            if(CerberusMilestone.MilestoneIndex != CerberusMilestone.MilestoneAmountNeeded.Length - 1)
            {
                CerberusMilestone.MilestoneIndex += 1;

            }
            gameManager.instance.player.GetComponent<PlayerController>().AddBloodMeterMilestone(20);
        }

        if(FlyingMilestone.MilestoneCount == FlyingMilestone.MilestoneAmountNeeded[FlyingMilestone.MilestoneIndex])
        {
            if (FlyingMilestone.MilestoneIndex != FlyingMilestone.MilestoneAmountNeeded.Length - 1)
            {
                FlyingMilestone.MilestoneIndex += 1;

            }
            gameManager.instance.player.GetComponent<PlayerController>().AddPlayerDamageMilestone(10);
        }

        if(SpecialEnemyCount >= SpecialEnemyRequirement)
        { 
            gameManager.instance.player.GetComponent<PlayerController>().hasThirdAbility = true;
        }
    }

    public void ResetMilestone(bool Answer)
    {
        if(Answer == true)
        {
            FlyingMilestone.MilestoneCount = 0;
            GolemMilestone.MilestoneCount = 0;
            CerberusMilestone.MilestoneCount = 0;

            FlyingMilestone.MilestoneIndex = 1;
            GolemMilestone.MilestoneIndex = 1;
            CerberusMilestone.MilestoneIndex = 1;

            gameManager.instance.player.GetComponent<PlayerController>().hasThirdAbility = false;
        }
    }

}
