using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAI : MonoBehaviour
{
    public Transform EndPoint; // 플레이어의 Transform을 Inspector에서 할당
    private NavMeshAgent agent;
    public Animator animator;

    public float speed;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        EndPoint = GameObject.FindWithTag("EndPoint").transform; // 태그를 이용하여 플레이어의 Transform을 찾습니다.
    }


    void Update()
    {
        if (EndPoint != null)
        {
            agent.SetDestination(EndPoint.position);
            speed = agent.velocity.magnitude;

        }
        else
        {
            EndPoint = GameObject.FindWithTag("EndPoint").transform; // 태그를 이용하여 플레이어의 Transform을 찾습니다.
        }
    }
}
