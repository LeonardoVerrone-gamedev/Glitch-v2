using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BullEnemy : MonoBehaviour
{
    public Vector3 patrolPointA; // Ponto A da patrulha
    public Vector3 patrolPointB; // Ponto B da patrulha
    public float patrolSpeed = 2f; // Velocidade de patrulha
    public float detectionRange = 5f; // Distância de detecção do jogador
    public float attackSpeed = 5f; // Velocidade de ataque
    public float attackDistanceOffset = 6f; // Distância de ataque
    public float prepareTime = 3f; // Tempo de preparação para ataque
    public float coolDownTime = 0.5f; // Tempo de preparação para ataque
    public float recuoTime;
    [SerializeField]BoxCollider2D shield;

    public Transform attackPoint;

    private Transform player; // Referência ao jogador
    public int damage = 10;
    private bool isFacingRight = true; // Direção que o inimigo está olhando
    private float prepareTimer = 0f; // Temporizador para preparação do ataque
    private Vector3 target; // Ponto atual de patrulha

    bool atingiu;
    bool recuo;

    private Vector3 originPoint;
    private Rigidbody2D rb; // Componente Rigidbody2D

    private enum State { Idle, PreparingAttack, Attacking, CoolDown, Recuo }
    [SerializeField]private State currentState = State.Idle;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Encontra o jogador
        rb = GetComponent<Rigidbody2D>(); // Obtém o Rigidbody2D
        target = patrolPointA; // Começa patrulhando em direção ao ponto A
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                Patrol();
                break;

            case State.PreparingAttack:
                PrepareAttack();
                break;

            case State.Attacking:
                Attack();
                break;
            case State.CoolDown:
                CoolDown();
                break;
            case State.Recuo:
                Recuar();
                break;
        }

        // Obtém a velocidade do Rigidbody2D
        Vector2 velocity = rb.velocity;

        // Verifica se a velocidade é diferente de zero
        if (velocity != Vector2.zero)
        {
            // Normaliza o vetor de velocidade para obter a direção
            Vector2 direction = velocity.normalized;

            // Chama a função Flip se a direção mudou
            if ((direction.x > 0 && !isFacingRight) || (direction.x < 0 && isFacingRight))
            {
                Flip();
            }

            //Debug.Log("Direção do objeto: " + direction);
        }//else
        //{
            //Debug.Log("O objeto está parado.");
        //}
    }

    void Patrol()
    {
        if(target != patrolPointA && target != patrolPointB){
            target = patrolPointA;
        }
        // Move o inimigo em direção ao alvo
        Vector3 direction = (target - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * patrolSpeed, rb.velocity.y); // Aplica a velocidade de patrulha

        // Verifica se o inimigo chegou ao ponto alvo apenas no eixo X
        if (Mathf.Abs(transform.position.x - target.x) <= 0.1f)
        {
            // Troca o alvo entre pointA e pointB
            target = (target == patrolPointA) ? patrolPointB : patrolPointA;
        }

        if (Vector2.Distance(transform.position, player.position) < detectionRange)
        {
            currentState = State.PreparingAttack;
        }
    }

    private bool IsPlayerInFront()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        return (isFacingRight && directionToPlayer.x > 0) || (!isFacingRight && directionToPlayer.x < 0);
    }

    private void PrepareAttack()
    {
        rb.velocity = Vector2.zero;
        originPoint = transform.position;
        Vector3 direction = (player.position - transform.position).normalized; // Direção para o jogador
        if(direction.x > 0){
            target = new Vector3(player.position.x + 15f, transform.position.y, transform.position.z);
        }else{
            target = new Vector3(player.position.x - 15f, transform.position.y, transform.position.x);
        }
        prepareTimer += Time.deltaTime;
        if (prepareTimer >= prepareTime)
        {
            currentState = State.Attacking;
            prepareTimer = 0f;
        }
    }

    private void Attack()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Vector3 direction = (target - transform.position).normalized;
        shield.enabled = true;
        rb.velocity = new Vector2(direction.x * attackSpeed, rb.velocity.y); // Aplica a velocidade de ataque

        // Verifica se o jogador saiu do alcance de detecção
        if (Vector3.Distance(transform.position, originPoint) >= 6f)
        {
            currentState = State.CoolDown; // Retorna ao estado Idle se o jogador sair do alcance
            rb.velocity = Vector2.zero; // Para o movimento
        }

        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, .5f, LayerMask.GetMask("Player"));
        foreach (Collider2D _player in hitPlayer)
        {
            PlayerHealth playerHealth = _player.GetComponent<PlayerHealth>();
            if (playerHealth != null && !atingiu)
            {
                atingiu = true;
                float hitDirection = (player.position.x - transform.position.x);
                if(hitDirection > 0){
                    playerHealth.TakeDamage(damage, true, true);
                }else{
                    playerHealth.TakeDamage(damage, true, false);
                }
                originPoint = transform.position;
            }
        }

        if(atingiu){
            shield.enabled = false;
            prepareTimer += Time.deltaTime;
            if (prepareTimer >= coolDownTime)
            {
                rb.velocity = Vector2.zero;
                originPoint = transform.position;
                currentState = State.Recuo; 
                prepareTimer = 0f;
                atingiu = false;
            }
        }
    }

    private void Recuar(){
        Vector2 direction = new Vector2(player.position.x, transform.position.y ); // Direção para o jogador
        rb.velocity = new Vector2(-direction.x * 0.3f, rb.velocity.y); 
        recuo = true;
        if(recuo){
            prepareTimer += Time.deltaTime;
            if (prepareTimer >= recuoTime)
            {
                rb.velocity = Vector2.zero;
                currentState = State.Idle; 
                prepareTimer = 0f;
                recuo = false;
            }
        }
    }

    private void CoolDown()
    {
        shield.enabled = false;
        prepareTimer += Time.deltaTime;
        if (prepareTimer >= coolDownTime)
        {
            currentState = State.Idle;
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

    private void OnDrawGizmos()
    {
        // Desenha os pontos de patrulha no editor para visualização
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(patrolPointA, 0.2f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(patrolPointB, 0.2f);
    }
}