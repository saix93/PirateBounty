using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyNavMeshAgent : MonoBehaviour
{
    public float DistanceFromTargetToStopAt = 2f;
    
    private Transform _player;
    private NavMeshAgent _agent;
    private Enemy _enemy;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _enemy = GetComponent<Enemy>();
    }

    private void Start()
    {
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;

        _player = FindObjectOfType<Player>().transform;
    }

    private void Update()
    {
        if (_enemy.currentHP <= 0)
        {
            _agent.isStopped = true;
            return;
        }

        var dest = (_player.position - transform.position).normalized;
        
        _agent.SetDestination(_player.position - dest * DistanceFromTargetToStopAt);
        transform.rotation = Utils.GetRotation2D(transform, _player.position, 720);
    }

    private void OnDrawGizmos()
    {
        if (_player == null) return;
        
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, _player.position);
    }
}
