using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunAbility : Ability
{
    [SerializeField] private float runSpeed;
    private float regularSpeed;
    private PlayerMovement playerMovement;
    private bool _isActive;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        regularSpeed = playerMovement.speed;
        runSpeed = playerMovement.speed * 2f;
    }

    public override void Activate()
    {
        // aumenta a velocidade
        playerMovement.speed = runSpeed;
        _isActive = true;
    }

    public override void Deactivate()
    {
        // Restaura a velocidade original
        playerMovement.speed = regularSpeed;

        //GetComponent<AbilityManager>().StopRun();
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
