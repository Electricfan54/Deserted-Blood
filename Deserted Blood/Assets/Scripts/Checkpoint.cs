using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    public Transform checkpointPos;

    public void SetCheckpoint()
    {

        if (gameManager.instance.playerCheckpoint != checkpointPos)
        {
            gameManager.instance.playerCheckpoint = checkpointPos;
        }

    }
    
}
