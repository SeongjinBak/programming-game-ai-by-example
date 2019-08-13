using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField]
    private Vector2 leftPost;
    [SerializeField]
    private Vector2 rightPost;

    // the direction of the goal

    public Vector2 facing;
    // the center of the goal line.
    [SerializeField]
    private Vector2 center;
    // It is increased when function Scored() is called.
    [SerializeField]
    private int numGoalsScored;


    private void Awake()
    {
        leftPost = new Vector2(transform.position.x , transform.position.y + 5f);
        rightPost = new Vector2(transform.position.x, transform.position.y - 5f);
        center = (leftPost + rightPost) / 2;
        numGoalsScored = 0;
        //facing = Vector2.Perpendicular((rightPost - leftPost).normalized);
    }

    public Vector2 Center()
    {
        return center;
    }
    public Vector2 LeftPost()
    {
        return leftPost;
    }
    public Vector2 RightPost()
    {
        return rightPost;
    }
    public Vector2 Facing()
    {
        return facing;
    }
}
