using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class NavBaked : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        BakedMapNavMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void BakedMapNavMesh()
    {
        NavMeshSurface navMeshSurface = transform.GetComponent<NavMeshSurface>();
        navMeshSurface.BuildNavMesh();
    }
}
