using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyAbility : Ability
{
    [SerializeField] private GameObject asas;
    private bool _isActive;

    public override void Activate()
    {
        asas.SetActive(true);
        GetComponent<PlayerMovement>().infinityJump = true;
        _isActive = true;
    }

    public override void Deactivate()
    {
        asas.SetActive(false);
        GetComponent<PlayerMovement>().infinityJump = false;
        GetComponent<PlayerMovement>().StopPlanar();
        _isActive = false;
    }

    public override float GetManaCost()
    {
        return 2f; // Custo de mana por segundo
    }

    public override bool isActive()
    {
        return _isActive;
    }
}
