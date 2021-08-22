using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSpawner : MonoBehaviour
{
    public EnemyShip ShipPrefab;

    public void SpawnShip()
    {
        var ship = Instantiate(ShipPrefab, transform);
        ship.transform.SetParent(null);
    }
}
