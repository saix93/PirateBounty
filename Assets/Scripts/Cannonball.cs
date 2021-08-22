using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannonball : MonoBehaviour
{
    public float forwardSpeed = 4f;
    public float maxTravelingTime = 4f;
    public AudioClip CannonFire;
    public AudioClip Explosion;

    private float _lifeTime;
    private bool _alive;
    private bool _moving;
    private Collider2D _collider;
    private Animator _animator;
    private AudioSource _audioSource;

    public void Init(float newSpeed, int layerToIgnore)
    {
        forwardSpeed = newSpeed;
    }

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _collider.enabled = false;
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }

    private IEnumerator Start()
    {
        _moving = true;
        _alive = true;
        _audioSource.PlayOneShot(CannonFire);
        
        yield return new WaitForSeconds(.2f);

        _collider.enabled = true;
    }

    private void Update()
    {
        if (!_alive) return;
        
        _lifeTime += Time.deltaTime;

        if (_lifeTime > maxTravelingTime)
        {
            StartCoroutine(DestroyCannonball());
        }
        
        if (_moving) transform.Translate(forwardSpeed * Time.deltaTime, 0, 0);
    }

    private IEnumerator DestroyCannonball()
    {
        _animator.SetTrigger("Impact");
        _audioSource.PlayOneShot(Explosion);
        _collider.enabled = false;
        _moving = false;
        _alive = false;
        
        yield return new WaitForSeconds(5f);
        
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        StartCoroutine(DestroyCannonball());
    }
}
