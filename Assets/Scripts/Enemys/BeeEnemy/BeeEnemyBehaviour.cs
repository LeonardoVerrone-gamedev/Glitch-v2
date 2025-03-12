using UnityEngine;

public class BeeEnemy : MonoBehaviour
{
    public Transform player; // Referência ao jogador
    public float detectionRange = 5f; // Distância para detectar o jogador
    public float attackRange = 1f; // Distância para iniciar o ataque
    public float moveSpeed = 2f; // Velocidade de movimento normal
    public float attackSpeed = 5f; // Velocidade de ataque
    public int damage = 10; // Dano causado ao jogador
    public bool atacou;
    public float flyAroundRadius = 2.5f; // Raio do movimento em volta do jogador
    public float flyAroundSpeed = 50f; // Velocidade de rotação ao redor do jogador
    public float flyAroundDuration = 3f; // Duração do voo em círculos antes de atacar
    public float retreatDistance = 1.5f; // Distância para se afastar do jogador
    public float minCooldown = 1f; // Cooldown mínimo
    public float maxCooldown = 3f; // Cooldown máximo

    private enum State { Idle, Approaching, FlyAround, Attacking, Retreating }
    private State currentState = State.Idle;
    private float flyAroundTimer; // Temporizador para o voo em círculos
    private Vector2 attackTargetPosition; // Posição alvo para o ataque
    private Animator animator; // Referência ao Animator

    private bool attacking;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if(player == null){
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        switch (currentState)
        {
            case State.Idle:
                IdleBehavior();
                break;
            case State.Approaching:
                ApproachingBehavior();
                break;
            case State.FlyAround:
                FlyAroundBehavior();
                break;
            case State.Attacking:
                AttackingBehavior();
                break;
            case State.Retreating:
                RetreatingBehavior();
                break;
        }
    }

    private void IdleBehavior()
    {
        float oscillation = Mathf.Sin(Time.time * 2) * 0.1f; // Ajuste a amplitude conforme necessário
        transform.position += new Vector3(oscillation, 0, 0);

        if (Vector2.Distance(transform.position, player.position) < detectionRange)
        {
            currentState = State.Approaching;
        }
    }

    private void ApproachingBehavior()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;

        if (Vector2.Distance(transform.position, player.position) < attackRange)
        {
            currentState = State.FlyAround;
            flyAroundTimer = flyAroundDuration; // Iniciar temporizador para o voo em círculos
            animator.SetBool("isAttacking", true); // Iniciar animação de ataque
        }
    }

    private void FlyAroundBehavior()
    {
        // Calcular a nova posição em um círculo ao redor do jogador
        float angle = Time.time * flyAroundSpeed; // Usar o tempo para calcular o ângulo
        float x = player.position.x + Mathf.Cos(angle) * flyAroundRadius; // Posição X
        float y = player.position.y + Mathf.Sin(angle) * flyAroundRadius; // Posição Y
        Vector2 targetPosition = new Vector2(x, y); // Nova posição da abelha

        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Reduzir o temporizador
        flyAroundTimer -= Time.deltaTime;

        atacou = false;

        // Verificar se o tempo de voo em círculos terminou
        if (flyAroundTimer <= 0)
        {
            // Armazenar a posição do jogador no momento do ataque
            attackTargetPosition = player.position; // Armazenar a posição do jogador
            attacking = true;
            currentState = State.Attacking; // Mudar para o estado de ataque
        }
    }

    private void AttackingBehavior()
    {
        if(attacking){
        // Mover-se em direção à posição alvo do ataque
            Vector2 direction = (attackTargetPosition - (Vector2)transform.position).normalized;
            transform.position += (Vector3)direction * attackSpeed * Time.deltaTime;
        }

        if((Vector2.Distance(transform.position, player.position) <= 0.5f) && !atacou){
            player.GetComponent<PlayerHealth>().TakeDamage(damage, false, false); // Aplicar dano ao jogador
            atacou = true;
        }

        // Verificar se a abelha se afastou muito da posição alvo
        if (Vector2.Distance(transform.position, attackTargetPosition) > 20f) // Limite de 20 passos
        {
            // Se a abelha se afastou demais, mudar para o estado de recuo
            animator.SetBool("isAttacking", false); // Parar animação de ataque
            currentState = State.Retreating; // Mudar para o estado de recuo
        }
        if (Vector2.Distance(transform.position, attackTargetPosition) <= 0.5f)
        {
            attacking = false;
            currentState = State.Retreating;
            EndAttack();
        }
    }

    private void EndAttack(){
        animator.SetBool("isAttacking", false); // Parar animação de ataque
        currentState = State.Retreating; // Mudar para o estado de recuo
    }

    private void RetreatingBehavior()
    {
        Vector2 direction = ((Vector2)transform.position - attackTargetPosition).normalized; // Converter transform.position para Vector2
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;

        if (Vector2.Distance(transform.position, attackTargetPosition) > retreatDistance)
        {
            currentState = State.Idle; // Voltar ao estado Idle
        }
    }
}