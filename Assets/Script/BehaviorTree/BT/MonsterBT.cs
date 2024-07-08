using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class MonsterBT : BehaviorTree.Tree
{
    public UnityEngine.Transform[] waypoints;

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
                new CheckEnemyInAttackRangeOfMonster(transform, agent),
                new TaskMonsterAttack(transform),
            }),
            new Sequence(new List<Node>{
                new CheckPlayerInFOVRange(transform, "Player"),
                new TaskGoToTarget(transform),
            }),
            new TaskPatrol(transform, waypoints, agent),
        });

        return root;
    }
}
