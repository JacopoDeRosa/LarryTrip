using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class Larry : MonoBehaviour
{
    [SerializeField]
    private Health _health;

    [SerializeField]
    private LarryStanceController _slideController;

    [SerializeField]
    private bool _isEnemy;

    [SerializeField]
    private float _targetSpeed;

    [SerializeField]
    private float _speedChangeSmoothness;

    [ShowInInspector]
    private List<EffectContainer> _activeEffects;

    [SerializeField]
    private float _currentSpeed;

    public float Speed { get => _currentSpeed; }
    public Stance CurrentStance { get => _slideController.CurrentStance; }

    public event FloatChangeHandler _onSpeedChange;

    public event IntChangeHandler _onDamage;
    public event IntChangeHandler _onHeal;

    private void Awake()
    {
        _activeEffects = new List<EffectContainer>();
    }

    // The onSpeedChange event gets called at start to tell listeners about the character's starting speed
    private void Start()
    {
        _currentSpeed = _targetSpeed;
        _onSpeedChange?.Invoke(Speed);
    }

    private void Update()
    {
        UpdateSpeed();
        UpdateEffects();
    }

    private void UpdateSpeed()
    {
        if (_currentSpeed <= _targetSpeed - 0.1f || _currentSpeed >= _targetSpeed + 0.1f)
        {
            float smooth = Mathf.Clamp01(_speedChangeSmoothness * Time.deltaTime);

            _currentSpeed = Mathf.Lerp(_currentSpeed, _targetSpeed, smooth);

            _onSpeedChange?.Invoke(_currentSpeed);
        }
        else if (_currentSpeed != _targetSpeed)
        {
            _currentSpeed = _targetSpeed;
            _onSpeedChange?.Invoke(_currentSpeed);
        }
    }

    private void UpdateEffects()
    {
        List<EffectContainer> toRemove = new List<EffectContainer>();
        foreach (EffectContainer cont in _activeEffects)
        {
            if (cont.AddToCurrentTime(Time.deltaTime))
            {
                toRemove.Add(cont);
            }
        }

        foreach (EffectContainer remove in toRemove)
        {
            remove.Effect.End(this);
            _activeEffects.Remove(remove);
        }
    }

    // Sets the character speed and invokes the onSpeedChange event to comunicate with eventual listeners, this is used for example
    // to comunicate with Larry's animator without referencing it in the character script.
    public void ChangeSpeed(float speed)
    {
        _targetSpeed += speed;
    }

    public void AddEffect(PickupEffect effect)
    {
        var container = new EffectContainer(effect);

        if (_activeEffects.Contains(container))
        {
            var existingContainer = _activeEffects.Find(x => x.Equals(container));
            existingContainer.SetCurrentTime(0);
            return;
        }

        effect.Begin(this);
        _activeEffects.Add(new EffectContainer(effect));
    }

    public void DealDamage(int damage)
    {
        _onDamage?.Invoke(damage);
        _health.ChangeHp(-damage);
    }
    public void HealDamage(int damage)
    {
        _onHeal?.Invoke(damage);
        _health.ChangeHp(damage);
    }

    private void OnTriggerEnter(Collider other)
    {
        // When Larry touches a tile with an interaction compent he triggers the effect on himself
        // After that it's the tile's job to give the effect to larry
        var tile = other.gameObject.GetComponent<InteractiveTile>();
        if (tile != null)
        {
            tile.Activate(this);
        }
    }
}

