using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyShip : Enemy
{
    [Header("Enemy Ship")]
    public Sprite ShipStatus0;
    public Sprite ShipStatus1;
    public Sprite ShipStatus2;
    public Sprite ShipStatus3;
    public ShipStatus Status;
    public SpriteRenderer ShipRenderer;

    protected override void Hit()
    {
        base.Hit();

        if (currentHP < 0) return;
        
        Status = (ShipStatus)(3 - currentHP);
        UpdateShipStatusVisual();
    }

    protected override void DestroyEntity()
    {
        base.DestroyEntity();

        StartCoroutine(ShipDeath());
    }

    private IEnumerator ShipDeath()
    {
        yield return new WaitForSeconds(1f);
        
        _animator.SetTrigger("Death");
        
        yield return new WaitForSeconds(3f);
        
        Destroy(gameObject);
    }

    private void UpdateShipStatusVisual()
    {
        Sprite newSprite;
        
        switch (Status)
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
}
