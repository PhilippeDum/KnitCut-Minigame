using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SynchroPath : MonoBehaviour
{
    public NavMeshSurface NMS;
    MovePiece MP;
    // Start is called before the first frame update
    void Start()
    {
        MP = FindObjectOfType<MovePiece>();
        NMS = gameObject.GetComponent<NavMeshSurface>();
        // Build navmesh path
        NMS.BuildNavMesh();
    }

    // Update is called once per frame
    void Update()
    {
        SwitchVerification();
    }
    void SwitchVerification()
    {
        if(MP.switchOn == true)
        {
            // Rebuild the navmesh path every time a switch happen
            NMS.BuildNavMesh();
            MP.switchOn = false;
        }
    }
}
