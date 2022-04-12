using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupTile : InteractiveTile
{
    [SerializeField] private PickupEffect _effect;

    public override void Activate(Larry character)
    {
        character.AddEffect(_effect);
        gameObject.SetActive(false);
    }
}
