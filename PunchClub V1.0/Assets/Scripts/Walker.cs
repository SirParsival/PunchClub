﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
//1
[RequireComponent(typeof(Actor))]
public class Walker : MonoBehaviour
{
    //2
    public NavMeshAgent navMeshAgent;
    private NavMeshPath navPath;
    private List<Vector3> corners;
    //3
    float currentSpeed;
    float speed;
    //4
    private Actor actor;
    private System.Action didFinishWalk;

    //5
    void Start()
    {
        navMeshAgent.updatePosition = false;
        navMeshAgent.updateRotation = false;
        actor = GetComponent<Actor>();
    }

    public bool MoveTo(Vector3 targetPosition, System.Action callback = null)
    {
        navMeshAgent.Warp(transform.position);
        didFinishWalk = callback;
        speed = actor.speed;
        navPath = new NavMeshPath();
        bool pathFound = navMeshAgent.CalculatePath(targetPosition, navPath);
        if (pathFound)
        {
            corners = navPath.corners.ToList();
            return true;
        }
        return false;
    }

    public void StopMovement()
    {
        navPath = null;
        corners = null;
        currentSpeed = 0;
    }

    protected void FixedUpdate()
    {
        //2
        bool canWalk = actor.CanWalk();
        if (canWalk && corners != null && corners.Count > 0)
        {
            currentSpeed = speed;
            actor.body.MovePosition(Vector3.MoveTowards(transform.position, corners[0],
            Time.fixedDeltaTime * speed));
            //3
            if (Vector3.SqrMagnitude(
            transform.position - corners[0]) < 0.6f)
            {
                corners.RemoveAt(0);
            }
            if (corners.Count > 0)
            {
                currentSpeed = speed;
                //4
                Vector3 direction = transform.position - corners[0];
                actor.FlipSprite(direction.x >= 0);
            }
            else
            {
                //5
                currentSpeed = 0.0f;
                if (didFinishWalk != null)
                {
                    didFinishWalk.Invoke();
                    didFinishWalk = null;
                }
            }
        }
        actor.baseAnim.SetFloat("Speed", currentSpeed);
    }
}
