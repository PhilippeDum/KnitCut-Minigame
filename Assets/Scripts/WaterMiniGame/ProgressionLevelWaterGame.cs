using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ProgressionLevelWaterGame : MonoBehaviour
{
    public NavMeshAgent NMA;
    public GameObject EndOfGameW;
    private float tempsBeforeMoving = 0f;
    private bool startMoving = false;
    private float tempsSecurité = 0f;
    private float tempsBeforeFailed = 2f;
    private VictoryWaterGame victoryWaterGame;
    private bool Failed = false;
    // Start is called before the first frame update
    void Start()
    {
        victoryWaterGame = FindObjectOfType<VictoryWaterGame>();
    }

    // Update is called once per frame
    void Update()
    {

        StopMiniGameWater();

    }
    void MovingWaterStart()
    {
        NMA.isStopped = false;
        tempsBeforeMoving = tempsBeforeMoving + Time.deltaTime;
        if (tempsBeforeMoving >= 5f)
        {
            NMA.SetDestination(EndOfGameW.transform.position);
            tempsBeforeMoving = 5f;
            startMoving = true; 
            
        }

    }
    void StopMovingVerification()
    {
        
        if (startMoving == true)
        {
            tempsSecurité = tempsSecurité + Time.deltaTime;
            if(tempsSecurité >= 3f)
            {
                tempsSecurité = 3f;
                if (NMA.velocity.x == 0f && NMA.velocity.z == 0f)
                {
                    Debug.Log("Failed");
                    Failed = true;
                }
                
            }
        }
        
    }
    void StopMiniGameWater()
    {
        if (victoryWaterGame.win == true || Failed == true)
        {
            NMA.isStopped = true;
            return;
        }
        else
        {
            MovingWaterStart();
            StopMovingVerification();
        }
    }

    
       
}
