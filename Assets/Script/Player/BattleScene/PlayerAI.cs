using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAI : MonoBehaviour
{
    public Transform EndPoint; // �÷��̾��� Transform�� Inspector���� �Ҵ�
    private NavMeshAgent agent;
    public Animator animator;

    public float speed;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        EndPoint = GameObject.FindWithTag("EndPoint").transform; // �±׸� �̿��Ͽ� �÷��̾��� Transform�� ã���ϴ�.
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
            EndPoint = GameObject.FindWithTag("EndPoint").transform; // �±׸� �̿��Ͽ� �÷��̾��� Transform�� ã���ϴ�.
        }
    }
}
