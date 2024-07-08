using System.Collections.Generic;
using BehaviorTree;

public class MonsterBT : Tree
{
    public UnityEngine.Transform[] waypoints;

    public static float speed = 2f;
    public static float Runspeed = 4f;
    public static float fovRange = 5f;
    public static float attackRange = 1f;

    protected override Node SetupTree()
    {
        Node root = new Selector(new List<Node>
        {
            new Sequence(new List<Node>{
                new CheckEnemyInAttackRangeOfMonster(transform),
                new TaskMonsterAttack(transform),
            }),
            new Sequence(new List<Node>{
                new CheckPlayerInFOVRange(transform, "Player"),
                new TaskGoToTarget(transform),
            }),
            new TaskPatrol(transform, waypoints),
        });

        return root;
    }
}
