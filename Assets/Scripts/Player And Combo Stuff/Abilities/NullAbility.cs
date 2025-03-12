using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NullAbility : Ability
{
    private bool _isActive;
    public override void Activate()
    {
        // Não faz nada
        _isActive = true;
    }

    public override void Deactivate()
    {
        // Não faz nada
        _isActive = false;
    }

    public override float GetManaCost()
    {
        return -2f; // Custo de mana por segundo
    }

    public override bool isActive()
    {
        return _isActive;
    }
}
