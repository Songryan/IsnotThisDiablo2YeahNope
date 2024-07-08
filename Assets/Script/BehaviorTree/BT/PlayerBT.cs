using BehaviorTree;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerBT : BehaviorTree.Tree
{
    //public UnityEngine.Transform[] waypoints;

    public static float speed = 2f;
    public static float Runspeed = 4f;
    public static float fovRange = 5f;
    public static float attackRange = 2f;

    [SerializeField] private NavMeshAgent agent;

    protected override Node SetupTree()
    {
        agent = transform.GetComponent<NavMeshAgent>();
        
        Node root = new Selector(new List<Node>
        {
            new Sequence(new List<Node>{
                new CheckEnemyInAttackRangeOfPlayer(transform, agent),
                new TaskPlayerAttack(transform),
            }),
            new Sequence(new List<Node>{
                new CheckEnemyInFOVRange(transform, "Monster"),
                new TaskGoToTarget(transform),
            }),
            new TaskSearchEndPoint(transform, agent),
        });

        return root;
    }
}
