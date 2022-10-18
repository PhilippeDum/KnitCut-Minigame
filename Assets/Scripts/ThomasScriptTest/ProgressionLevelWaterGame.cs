using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ProgressionLevelWaterGame : MonoBehaviour
{
    public NavMeshAgent NMA;
    public GameObject EndOfGameW;
    private float tempsBeforeMoving = 0f;
    bool startMoving = false;
    float tempsSecurité = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MovingWaterStart();
        StopMovingVerification();
    }
    void MovingWaterStart()
    {
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
        if(startMoving == true)
        {
            tempsSecurité = tempsSecurité + Time.deltaTime;
            if(tempsSecurité >= 2f)
            {
                tempsSecurité = 2f;
            }
        }
        if(startMoving == true && NMA.speed < 0.1f && tempsSecurité >= 2f)
        {
            Debug.Log("Failed");
        }
    }
       
}
