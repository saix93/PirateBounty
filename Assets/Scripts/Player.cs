using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Visual parts")]
    public SpriteRenderer ShipRenderer;
    public Sprite ShipStatus0;
    public Sprite ShipStatus1;
    public Sprite ShipStatus2;
    public Sprite ShipStatus3;
    
    [Header("MainCanvas")]
    public Image Crosshair;
    
    [Header("Progress Bars")]
    public Transform LeftProgressBar;
    public Image LeftProgressBarFiller;
    public Transform RightProgressBar;
    public Image RightProgressBarFiller;

    [Header("Cannons")]
    public Transform LeftCannonContainer;
    public Transform RightCannonContainer;
    public Animator LeftCannonAnimator;
    public Animator RightCannonAnimator;
    public Cannonball CannonballPrefab;

    [Header("Data")]
    public float minSpeed = 1f;
    public float maxSpeed = 5f;
    public float accelerationMultiplier = 1f;
    public float minRotationSpeed = 90f;
    public float maxRotationSpeed = 180f;
    public float rotationNeededToReload = 180f;
    public float cannonShotDelay = .2f;
    public ShipStatus PlayerStatus;

    private Animator _playerAnimator;
    private float _lastH;
    private float _reloadAmount;
    private int _rightCannonBallsLoaded;
    private int _leftCannonBallsLoaded;

    private float _currentSpeed;
    private float _currentRotationSpeed;

    private bool _alive;

    enum ShipSides
    {
        No,
        Right,
        Left
    }

    private void Awake()
    {
        _playerAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        _alive = true;
        _currentSpeed = minSpeed;
        UpdateShipStatusVisual();
    }

    private void Update()
    {
        if (!_alive) return;

        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");
        var chargingSide = GetShipSide(h);

        if (v > 0)
        {
            _currentSpeed = Mathf.Clamp(_currentSpeed + Time.deltaTime * accelerationMultiplier, minSpeed, maxSpeed);
        }
        else if (v < 0)
        {
            _currentSpeed = Mathf.Clamp(_currentSpeed - Time.deltaTime * accelerationMultiplier, minSpeed, maxSpeed);
        }
        
        if (Math.Abs(_lastH - h) < .1f && chargingSide != ShipSides.No)
        {
            if (chargingSide == ShipSides.Left && _leftCannonBallsLoaded > 0
                || chargingSide == ShipSides.Right && _rightCannonBallsLoaded > 0)
            {
                _reloadAmount = 0f;
            }
            else
            {
                _reloadAmount += Time.deltaTime;
            }
        }
        else
        {
            _reloadAmount = 0f;
        }
        
        _lastH = h;

        _currentRotationSpeed = CalculateRotation(_currentSpeed);

        if (_reloadAmount >= rotationNeededToReload / _currentRotationSpeed)
        {
            switch (chargingSide)
            {
                case ShipSides.Left:
                    if (_leftCannonBallsLoaded > 0) break;
                    LeftCannonAnimator.SetBool("CannonReady", true);
                    _leftCannonBallsLoaded++;
                    break;
                case ShipSides.Right:
                    if (_rightCannonBallsLoaded > 0) break;
                    RightCannonAnimator.SetBool("CannonReady", true);
                    _rightCannonBallsLoaded++;
                    break;
            }

            _reloadAmount = 0;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (_leftCannonBallsLoaded > 0)
            {
                StartCoroutine(ShootCannon_CR(ShipSides.Left));
            }
            if (_rightCannonBallsLoaded > 0)
            {
                StartCoroutine(ShootCannon_CR(ShipSides.Right));
            }
        }
        
        // Si se rota más de 1 vez se acumulan los disparos / se hace un disparo más gordo
        UpdateReadyToFireUI(chargingSide, _reloadAmount / (rotationNeededToReload / _currentRotationSpeed));
        UpdateCrosshair();
        transform.Rotate(0, 0, _currentRotationSpeed * h * Time.deltaTime * -1);
        transform.Translate(0, _currentSpeed * Time.deltaTime, 0);

        ShipSides GetShipSide(float _h)
        {
            var side = ShipSides.No;
            
            if (_h < -.2f)
            {
                side = ShipSides.Left;
            }
            else if (_h > .2f)
            {
                side = ShipSides.Right;
            }

            return side;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!_alive) return;
        
        GetHit();
        UpdateShipStatusVisual();
    }

    private float CalculateRotation(float speed)
    {
        float normal = Mathf.InverseLerp(minSpeed, maxSpeed, speed);
        float rotationSpeed = Mathf.Lerp(maxRotationSpeed, minRotationSpeed, normal);

        return rotationSpeed;
    }

    private void UpdateShipStatusVisual()
    {
        Sprite newSprite;
        
        switch (PlayerStatus)
        {
            case ShipStatus.Fine:
                newSprite = ShipStatus0;
                break;
            case ShipStatus.Damaged:
                newSprite = ShipStatus1;
                break;
            case ShipStatus.Critical:
                newSprite = ShipStatus2;
                break;
            case ShipStatus.Destroyed:
                newSprite = ShipStatus3;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        ShipRenderer.sprite = newSprite;
    }

    private void UpdateReadyToFireUI(ShipSides chargingSide, float progress)
    {
        if (progress < .01f
            || chargingSide == ShipSides.Left && _leftCannonBallsLoaded > 0
            || chargingSide == ShipSides.Right && _rightCannonBallsLoaded > 0)
        {
            LeftProgressBar.gameObject.SetActive(false);
            RightProgressBar.gameObject.SetActive(false);
            return;
        }

        switch (chargingSide)
        {
            case ShipSides.No:
                break;
            case ShipSides.Right:
                RightProgressBar.gameObject.SetActive(true);
                RightProgressBarFiller.fillAmount = progress / 2;
                break;
            case ShipSides.Left:
                LeftProgressBar.gameObject.SetActive(true);
                LeftProgressBarFiller.fillAmount = progress / 2;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(chargingSide), chargingSide, null);
        }
    }

    private IEnumerator ShootCannon_CR(ShipSides cannon)
    {
        var wait = new WaitForSeconds(cannonShotDelay);
        
        switch (cannon)
        {
            case ShipSides.Left:
                while (_leftCannonBallsLoaded > 0)
                {
                    LeftCannonAnimator.SetTrigger("CannonShot");
                    var cannonball = InstantiateCannonball(LeftCannonAnimator.transform);
                    _leftCannonBallsLoaded--;
                    yield return wait;
                }
                
                LeftCannonAnimator.SetBool("CannonReady", false);
                
                break;
            case ShipSides.Right:
                while (_rightCannonBallsLoaded > 0)
                {
                    RightCannonAnimator.SetTrigger("CannonShot");
                    var cannonball = InstantiateCannonball(RightCannonAnimator.transform);
                    _rightCannonBallsLoaded--;
                    yield return wait;
                }
                
                RightCannonAnimator.SetBool("CannonReady", false);
                
                break;
        }
        
        Cannonball InstantiateCannonball(Transform spawner)
        {
            var cannonball = Instantiate(CannonballPrefab, spawner);
            cannonball.transform.SetParent(null);

            return cannonball;
        }
    }

    private void UpdateCrosshair()
    {
        var mouse = Input.mousePosition;
        mouse.z = 0;
        Crosshair.transform.position = mouse;

        LeftCannonContainer.rotation = Utils.GetRotation2D(LeftCannonContainer, Camera.main.ScreenToWorldPoint(mouse));
        RightCannonContainer.rotation = Utils.GetRotation2D(RightCannonContainer, Camera.main.ScreenToWorldPoint(mouse));
    }

    private void GetHit()
    {
        PlayerStatus++;
        _playerAnimator.SetTrigger("ShipHit");

        if (PlayerStatus == ShipStatus.Destroyed)
        {
            ShipDestroyed();
        }
    }

    private void ShipDestroyed()
    {
        // Fin del juego
        _alive = false;
        GameManager._.ShowGameOverScreen();
    }
}

public enum ShipStatus
{
    Fine,
    Damaged,
    Critical,
    Destroyed
}
