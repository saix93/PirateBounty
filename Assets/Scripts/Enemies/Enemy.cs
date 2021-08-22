using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Cannonball CannonballPrefab;
    public Transform CannonballSpawner;
    public Transform Cannon;
    public float CannonballSpeed = 3f;
    public float maxTargetingDistance = 10f;
    public float shootingSpeed = 4f;
    public int currentHP = 2;
    public int score = 10;

    private Player _player;
    private float _timeToShoot;
    protected Animator _animator;
    private Collider2D _collider;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();
    }

    protected virtual void Start()
    {
        _player = FindObjectOfType<Player>();
    }

    private void Update()
    {
        if (_player is null) return;
        if (currentHP <= 0) return;

        _timeToShoot += Time.deltaTime;
        
        if (Vector3.Distance(_player.transform.position, transform.position) <= maxTargetingDistance)
        {
            Cannon.rotation = Utils.GetRotation2D(Cannon, _player.transform.position);

            if (_timeToShoot > shootingSpeed)
            {
                ShootCannon();

                _timeToShoot = 0;
            }
        }
    }

    private void ShootCannon()
    {
        var cannonball = Instantiate(CannonballPrefab, CannonballSpawner);
        cannonball.Init(CannonballSpeed, gameObject.layer);
        cannonball.transform.SetParent(null);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Hit();
    }

    protected virtual void Hit()
    {
        currentHP--;
        _animator.SetTrigger("Hit");

        if (currentHP <= 0)
        {
            DestroyEntity();
        }
    }

    protected virtual void DestroyEntity()
    {
        _collider.enabled = false;
        Destroy(Cannon.gameObject);
        
        GameManager._.AddScore(score);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxTargetingDistance);
    }
}
