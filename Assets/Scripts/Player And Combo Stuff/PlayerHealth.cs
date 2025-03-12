using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float currentHealth = 0f;
    public float maxHealth = 100f;
    [SerializeField] private Image healthImage; // Referência à imagem de vida

    public int vidas = 3;

    private Rigidbody2D rb;
    private PlayerMovement playerMovement;
    private float timer = 0f;
    [SerializeField]private float pushDuration = 2f; // Duração do empurrão em segundos
    [SerializeField] private float DirVert;
    [SerializeField] private float DirHor;
    [SerializeField] private float pushBackForce;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Obtém o Rigidbody2D do jogador
        playerMovement = GetComponent<PlayerMovement>();
        GameObject healthBarContent = GameObject.Find("HealthBarContent");
        healthImage = healthBarContent.GetComponent<Image>();

        // Inicializa a imagem de mana
        if (healthImage != null)
        {
            healthImage.fillAmount = currentHealth / maxHealth;
        }
    }

    void Update()
    {
        // Atualiza a imagem de mana
        if (healthImage != null)
        {
            healthImage.fillAmount = currentHealth / maxHealth;
        }

        if (playerMovement.isBeingPushed)
        {
            timer += Time.deltaTime; // Incrementa o timer com o tempo desde o último frame

            // Verifica se o tempo do empurrão expirou
            if (timer >= pushDuration)
            {
                playerMovement.isBeingPushed = false; // Reseta o estado de empurrão
                GetComponent<PlayerInputs>().canMove = true;
                timer = 0f; // Reseta o timer
            }
        }
    }

    public void TakeDamage(int damage, bool pushBack, bool _right){
        currentHealth-=damage;

        if(pushBack == true){
            // Aplica a força de empurrão
            PushBack(_right);
        }
    }

    public void PushBack(bool _right)
    {
        timer = 0f;
        GetComponent<RunAbility>().Deactivate();
        GetComponent<PlayerCombat>().isRunning = false;
        rb.velocity = Vector2.zero;
        playerMovement.isBeingPushed = true;
        GetComponent<PlayerInputs>().canMove = false;
        
        Vector2 hitDirection;
        if(_right == true){
            hitDirection = new Vector2(DirHor, DirVert);
        }else{
            hitDirection = new Vector2(-DirHor, DirVert);
        }
        // Normaliza a direção do golpe e aplica a força
        Vector2 pushDirection = hitDirection.normalized;
        //Animação do pushback
        // Verifica a direção do golpe para virar o jogador
        if (pushDirection.x > 0 && playerMovement.isFacingRight)
        {
            playerMovement.Flip(); // Vira o jogador para a esquerda
        }
        else if(pushDirection.x < 0 && !playerMovement.isFacingRight)
        {
            playerMovement.Flip(); // Vira o jogador para a direita
        }
        GetComponent<PlayerAnimation>().PlayAnimationByTrigger("PushBack");
        rb.AddForce(pushDirection * pushBackForce, ForceMode2D.Impulse);
    }
}
