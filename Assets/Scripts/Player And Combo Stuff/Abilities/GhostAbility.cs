using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostAbility : Ability
{
    [SerializeField] private BoxCollider2D boxCollider; // O BoxCollider2D que será desativado/ativado
    private SpriteRenderer spriteRenderer; // O SpriteRenderer do jogador
    private bool _isActive;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    public override void Activate()
    {
        // Reduz a opacidade para 50%
        Color color = spriteRenderer.color;
        color.a = 0.5f; // 50% de opacidade
        spriteRenderer.color = color;

        // Desativa o BoxCollider2D
        if (boxCollider != null)
        {
            boxCollider.enabled = false;
        }
        _isActive = true;
    }

    public override void Deactivate()
    {
        // Restaura a opacidade para 100%
        Color color = spriteRenderer.color;
        color.a = 1f; // 100% de opacidade
        spriteRenderer.color = color;
        GetComponent<PlayerCombat>().isIntangible = false;

        // Ativa o BoxCollider2D
        if (boxCollider != null)
        {
            boxCollider.enabled = true;
        }
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
