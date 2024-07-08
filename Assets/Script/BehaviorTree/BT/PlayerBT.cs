using BehaviorTree;
using System.Collections.Generic;

public class PlayerBT : Tree
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
                new CheckEnemyInAttackRangeOfPlayer(transform),
                new TaskPlayerAttack(transform),
            }),
            new Sequence(new List<Node>{
                new CheckEnemyInFOVRange(transform, "Monster"),
                new TaskGoToTarget(transform),
            }),
            new TaskSearchEndPoint(transform, waypoints),
        });

        return root;
    }
}
