using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ProgressionLevelWaterGame : MonoBehaviour
{
    public NavMeshAgent NMA;
    public GameObject EndOfGameW;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        NMA.SetDestination(EndOfGameW.transform.position);
    }
}
