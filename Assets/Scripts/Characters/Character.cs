using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Character : MonoBehaviour
{
    [SerializeField]
    private Health _health;

    [SerializeField]
    private bool _isEnemy;

    [SerializeField]
    private float _speed;

    public float Speed { get => _speed; }
    public Health Health { get => _health; }

    public event FloatChangeHandler _onSpeedChange;

    public void SetSpeed(float speed)
    {
        _speed = speed;
        _onSpeedChange?.Invoke(_speed);
    }
}