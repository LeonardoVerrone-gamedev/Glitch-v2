using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaEnemy : MonoBehaviour
{
    private Transform player; // Referência ao jogador
    public float detectionRange;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    public int damage = 10;
    private bool isFacingRight = true; // Direção que o inimigo está olhando
    private enum State { Idle, PrepareAttack, Attack, CoolDown }
    [SerializeField] private State currentState = State.Idle;
    public GameObject SmokePrefab;

    public float prepareTimer = 0f;
    public float coolDownTime = 1f; 

    private bool hasDisapeared = true;

    private Vector3 originPoint;
    public bool originPointSeted;

    int hits = 0;
    public bool lastLeft;
    bool atingiu;
    bool teletransportou;

    private bool IsPlayerInFront()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        return (isFacingRight && directionToPlayer.x > 0) || (!isFacingRight && directionToPlayer.x < 0);
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Encontra o jogador
        rb = GetComponent<Rigidbody2D>(); // Obtém o Rigidbody2D
        spriteRenderer = GetComponent<SpriteRenderer>();
        hasDisapeared = true;
        Color color = spriteRenderer.color;
        color.a = 0f; // 0% de opacidade
        spriteRenderer.color = color;
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                Disappear();
                WaitPlayer();
                break;
            case State.Attack:
                Attack();
                break;
            case State.PrepareAttack:
                PrepareAttack();
                break;
            case State.CoolDown:
                CoolDown();
                break;
        }

        // Chama o método Flip para garantir que o inimigo sempre olhe para o jogador
        if (player != null)
        {
            UpdateFacingDirection();
        }
    }

    public void Disappear()
    {
        if (!hasDisapeared)
        {
            hasDisapeared = true;
            Instantiate(SmokePrefab, transform.position, Quaternion.identity);
            Color color = spriteRenderer.color;
            color.a = 0f; // 0% de opacidade
            spriteRenderer.color = color;
        }
    }

    public void Appear()
    {
        if (hasDisapeared)
        {
            hasDisapeared = false;
            Instantiate(SmokePrefab, transform.position, Quaternion.identity);
            Color color = spriteRenderer.color;
            color.a = 1f; // 100% de opacidade
            spriteRenderer.color = color;
        }
    }

    public void WaitPlayer()
    {
        hits = 0;
        if (Vector2.Distance(transform.position, player.position) < detectionRange)
        {
            currentState = State.PrepareAttack;
        }
    }

    public void PrepareAttack(){
        Appear();
        // Define a posição de teletransporte com base no número de ataques
        Vector3 targetPosition;
        if (lastLeft == true)
        {
            targetPosition = new Vector3(player.position.x + 10f, player.position.y + 3f, 0f); // Teletransporte para a direita
            lastLeft = false;
        }
        else
        {
            targetPosition = new Vector3(player.position.x - 10f, player.position.y + 3f, 0f); // Teletransporte para a esquerda
            lastLeft = true;
        }
        transform.position = targetPosition; // Teletransporta o inimigo
        currentState = State.Attack;
    }

    public void Attack()
    {
        if(!atingiu){
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            Vector3 direction = (player.position - transform.position).normalized;
            rb.velocity = new Vector2(direction.x * 5f, rb.velocity.y); // Aplica a velocidade de ataque

            Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(transform.position, .5f, LayerMask.GetMask("Player"));
            foreach (Collider2D _player in hitPlayer)
            {
                PlayerHealth playerHealth = _player.GetComponent<PlayerHealth>();
                if (playerHealth != null && !atingiu)
                {
                    atingiu = true;
                    hits += 1;
                    float hitDirection = (player.position.x - transform.position.x);
                    if(hitDirection < 0f){
                        playerHealth.TakeDamage(damage, true, false);
                    }else{
                        playerHealth.TakeDamage(damage, true, true);
                    }
                }
            }
        }

        if(atingiu){
            rb.velocity = Vector2.zero;
            prepareTimer += Time.deltaTime;
            if (prepareTimer >= coolDownTime)
            {
                if (hits < 2)
                {
                    currentState = State.PrepareAttack;
                }
                else
                {
                    currentState = State.CoolDown;
                } 
                Disappear();
                prepareTimer = 0f;
                atingiu = false;
                teletransportou = false;
            }
        }
    }


    private void CoolDown()
    {
        rb.velocity = Vector2.zero;
        prepareTimer += Time.deltaTime;
        if (prepareTimer >= coolDownTime)
        {
            currentState = State.PrepareAttack;
            teletransportou = false;
            atingiu = false;
            originPointSeted = false;
            prepareTimer = 0f;
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1; // Inverte a escala no eixo X
        transform.localScale = theScale;
    }

    private void UpdateFacingDirection()
    {
        if (player.position.x < transform.position.x && isFacingRight)
        {
            Flip();
        }
        else if (player.position.x > transform.position.x && !isFacingRight)
        {
            Flip();
        }
    }
}