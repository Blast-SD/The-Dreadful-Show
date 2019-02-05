﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/*
[Script Header] CustomNavMeshAgent Version 0.0.1
Created by: Thiebaut Alexis 
Date: 14/01/2019
Description: - Agent of the customNavMesh, they can Follow a path stocked in their CustomNavPath
             - They can check if their path can be compute before following a path
             - They have an offset and a size that allow them to be on the navmesh

///
[UPDATES]
Update n°: 001
Updated by: Thiebaut Alexis 
Date: 14/01/2019
Description: Rebasing the agent on a previously created agent

Update n°: 002
Updated By Thiebaut Alexis
Date: 21/01/2019
Description: Try to add steering to the agent

Update n°: 003
Updated by: Thiebaut Alexis
Date: 25/01/2019 - 31/01/2019
Description: Adding Steering Behaviour to the agent
             - The path is now smoothed
             - The agent can't avoid each other. Still have to implement the agent avoidance

Update n°: 004
Updated by: Thiebaut Alexis
Date: 04/02/2019 
Description: Fixing Offseted Positions errors
*/
public class CustomNavMeshAgent : MonoBehaviour
{
    #region Events
    public event Action OnMovementStarted; 
    public event Action OnDestinationReached;
    #endregion

    #region FieldsAndProperty
    #region Inspector
    #region Vector3
    [Header("Agent Settings")]
    [SerializeField] private Vector3 positionOffset;
    #endregion

    #region float
    [SerializeField, Range(.1f, 5)] private float height = 1;
    public float Height { get { return height / 2; } }

    [SerializeField, Range(.5f, 2)] private float radius = 1;
    public float Radius { get { return radius * .75f; } }

    [SerializeField, Range(-5, 5)] private float baseOffset = 0;

    [SerializeField, Range(.1f, 10)] private float speed = 1;

    [SerializeField, Range(1, 10)] private float detectionRange = 2;

    [SerializeField, Range(.1f, 10)] private float steerForce = 2;

    [SerializeField, Range(.1f, 10)] private float avoidanceForce = 2;
    #endregion

    #endregion

    #region Other Fields and properties
    #region bool
    bool isMoving = false;
    public bool IsMoving { get { return isMoving; } }
    #endregion

    #region int
    int pathIndex = 0;
    #endregion

    #region Path
    private CustomNavPath currentPath = new CustomNavPath();
    public CustomNavPath CurrentPath { get { return currentPath; } }
    #endregion

    #region CalculatingState
    private CalculatingState pathState = CalculatingState.Waiting;
    public CalculatingState PathState { get { return pathState; } }
    #endregion

    #region Vector3
    public Vector3 CenterPosition { get { return transform.position + positionOffset; } }
    public Vector3 OffsetSize { get { return new Vector3(radius, height, radius); } }
    public Vector3 OffsetPosition
    {
        get
        {
            Vector3 _heightOffset = new Vector3(0, (height / 2) + baseOffset, 0);
            return  CenterPosition - _heightOffset ;
        }
    }

    public Vector3 LastPosition
    {
        get
        {
            if (currentPath.PathPoints.Count == 0) return CenterPosition; 
            return currentPath.PathPoints.Last();
        }
    }

    [SerializeField] private Vector3 velocity; 
    public Vector3 Velocity { get { return velocity;} }
    #endregion
    #endregion
    #endregion

    #region Methods

    /// <summary>
    /// Check if the destination can be reached
    /// </summary>
    /// <param name="_position">destination to reach</param>
    /// <returns>if the destination can be reached</returns>
    public bool CheckDestination(Vector3 _position)
    {
        pathState = CalculatingState.Calculating;
        bool _canBeReached = PathCalculator.CalculatePath(CenterPosition, _position, currentPath, CustomNavMeshManager.Instance.Triangles);
        if (_canBeReached)
        {
            pathState = CalculatingState.Ready;
            StartCoroutine(FollowPath());
        }
        else pathState = CalculatingState.Waiting;
        return _canBeReached;
    }

    /// <summary>
    /// Make the agent follows the path
    /// </summary>
    /// <returns></returns>
    IEnumerator FollowPath()
    {
        isMoving = true;
        pathIndex = 1;
        velocity = Vector3.zero; 
        List<Vector3> _followingPath = currentPath.PathPoints;  //List of points in the path

        /*STEERING*/

        //Predicted Velocity
        Vector3 _predictedVelocity;
        //Predicted position if the agent use the predicted velocity
        Vector3 _predictedPosition;

        // Previous Position
        Vector3 _previousPosition = CenterPosition;
        //Next Position
        Vector3 _nextPosition = _followingPath[pathIndex];

        Vector3 _dir;
        Vector3 _targetPosition;
        Vector3 _normalPoint;

        // Magnitude of the normal from the dir b reaching the predicted location
        float _distance = 0;

        /* First the velocity is equal to the normalized direction from the agent position to the next position */
        if (velocity == Vector3.zero)
            velocity = (_nextPosition - OffsetPosition).normalized * speed;

        while (Vector3.Distance(OffsetPosition, LastPosition) > radius)
        {
            /* Apply the velocity to the transform position multiply by the speed and by Time.deltaTime to move*/
            transform.position += velocity * Time.deltaTime;

            /* If the agent is close to the next position
             * Update the previous and the next point
             * Also update the pathIndex
             * if the pathindex is greater than the pathcount break the loop
             * else continue in the loop
             */
            if (Vector3.Distance(OffsetPosition, _nextPosition) <= .1f)
            {
                //set the new previous position
                _previousPosition = _followingPath[pathIndex];
                //Increasing path index
                pathIndex++;
                if (pathIndex > _followingPath.Count - 1) break;
                //Set the new next Position
                _nextPosition = _followingPath[pathIndex];
                continue;
            }

            /* Get the predicted Velocity and the Predicted position*/
            _predictedVelocity = velocity;
            _predictedPosition = OffsetPosition + _predictedVelocity;

            /*Get the transposed Position of the predicted position on the segment between the previous and the next point
            * The agent has to get closer while it's to far away from the path 
            */
            _normalPoint = GetNormalPoint(_predictedPosition, _previousPosition, _nextPosition);


            /* Direction of the segment between the previous and the next position normalized in order to go further on the path
             * Targeted position is the normal point + a offset defined by the direction of the segment to go a little further on the path
             * If the target is out of the segment between the previous and the next position, the target position is the next position
             */
            _dir = (_nextPosition - _previousPosition).normalized;
            _targetPosition = _normalPoint + _dir;
            if (Vector3.Distance(_previousPosition, _targetPosition) > Vector3.Distance(_previousPosition, _nextPosition))
            {
                Seek(_nextPosition);
            }

            /* Distance between the predicted position and the normal point on the segment 
             * If the distance is greater than the radius, it has to steer to get closer
             */
            _distance = Vector3.Distance(_predictedPosition, _normalPoint);
            if (_distance > (radius))
            {
                Seek(_targetPosition);
            }
            /* Check if there is any obstacle in front of the agent*/
            yield return new WaitForEndOfFrame();
        }
        pathState = CalculatingState.Waiting;
        pathIndex = 1;
        isMoving = false;
        OnDestinationReached?.Invoke();
    }

    /// <summary>
    /// Get the transposed point of the predicted position on a segement between the previous and the next position
    /// </summary>
    /// <param name="_predictedPosition">Predicted Position</param>
    /// <param name="_previousPosition">Previous Position</param>
    /// <param name="_nextPosition">Next Position</param>
    /// <returns></returns>
    Vector3 GetNormalPoint(Vector3 _predictedPosition, Vector3 _previousPosition, Vector3 _nextPosition)
    {
        Vector3 _ap = _predictedPosition - _previousPosition;
        Vector3 _ab = (_nextPosition - _previousPosition).normalized;
        Vector3 _ah = _ab * (Vector3.Dot(_ap, _ab));
        return (_previousPosition + _ah);
    }

    /// <summary>
    /// Calculate the needed velocity 
    /// Desired velocity - currentVelocity
    /// </summary>
    /// <param name="_target"></param>
    void Seek(Vector3 _target)
    {
        Vector3 _desiredVelocity = (_target - OffsetPosition).normalized * speed;
        Vector3 _steer = (_desiredVelocity - velocity) * steerForce;
        velocity += _steer;
    }

    /// <summary>
    /// Calculate a path until reaching a destination
    /// </summary>
    /// <param name="_position">destination to reach</param>
    public void SetDestination(Vector3 _position)
    {
        pathState = CalculatingState.Calculating;
        if (PathCalculator.CalculatePath(CenterPosition, _position, currentPath, CustomNavMeshManager.Instance.Triangles))
        {
            pathState = CalculatingState.Ready;
            StartCoroutine(FollowPath());
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void StopAgent()
    {
        StopCoroutine(FollowPath());
        pathState = CalculatingState.Waiting;
        pathIndex = 1;
        isMoving = false;
        velocity = Vector3.zero; 
    }
    #endregion

    #region UnityMethods
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(CenterPosition, .1f); 
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(CenterPosition + velocity, .1f);
        Gizmos.DrawLine(CenterPosition, CenterPosition + velocity );
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(OffsetPosition, .1f);
        if (currentPath == null || currentPath.PathPoints == null || currentPath.PathPoints.Count == 0) return;
        for (int i = 0; i < currentPath.PathPoints.Count; i++)
        {
            Gizmos.DrawSphere(currentPath.PathPoints[i], .2f);
        }
        for (int i = 0; i < currentPath.PathPoints.Count - 1; i++)
        {
            Gizmos.DrawLine(currentPath.PathPoints[i], currentPath.PathPoints[i + 1]);
        }
    }
    #endregion
}
public enum CalculatingState
{
    Waiting,
    Calculating,
    Ready
}