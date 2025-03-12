using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 4f;
    public float jumpForce = 10f;
    public bool infinityJump = false;
    [SerializeField] public bool estaPlanando;
    public bool isLookingUp = false;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckCollider;
    [SerializeField] private Transform wallCheckPoint;
    [SerializeField] private LayerMask groundLayer;
    public bool isGrounded;

    [SerializeField] SpriteRenderer asasSpriteRenderer;

    [Header("Flip Settings")]
    public bool isFacingRight = true;
    public bool isBeingPushed;

    [Header("Camera Settings")]
    private Coroutine resetTriggerCoroutine;
    private CameraFollowObject cameraFollowObject;

    [Header("Other")]
    bool atingiuParede;
    float paredeTimer = 3f;
    private Rigidbody2D rb;
    public float horizontalValue;
    public float lastHorizontal;
    public float verticalValue;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cameraFollowObject = GameObject.FindObjectOfType<CameraFollowObject>();
    }

    void Update()
    {
        if(!isBeingPushed){
            FlipCheck(); // Verifica se o jogador precisa flipar
        }

        if(infinityJump && asasSpriteRenderer.color.a < 0.5f && !estaPlanando){
            Color color = asasSpriteRenderer.color;
            color.a = 1f; // 100% de opacidade
            asasSpriteRenderer.color = color;
        }

        if(GetComponent<PlayerCombat>().isRunning && !GetComponent<PlayerCombat>().isIntangible){
            Collider2D[] hitWall = Physics2D.OverlapCircleAll(wallCheckPoint.position, .5f, LayerMask.GetMask("Wall"));
            if(hitWall.Length > 0)
            {
                PlayerHealth playerHealth = GetComponent<PlayerHealth>();
                if (playerHealth != null && !atingiuParede)
                {
                    atingiuParede = true;
                    Vector2 hitDirection;
                    if(isFacingRight){
                        playerHealth.PushBack(false);
                    }else{
                        playerHealth.PushBack(true);
                    }
                }
            }
        }

        if(atingiuParede){
            paredeTimer -= Time.deltaTime;
            isLookingUp = false;
            if(paredeTimer <= 0f){
                atingiuParede = false;
                paredeTimer = .75f;
            }
        }

        if(isGrounded && estaPlanando){
            StopPlanar();
        }
    }

    public void Planar(){
        if(!isGrounded && infinityJump){
            estaPlanando = true;
            GameObject.FindObjectOfType<CameraManagerScript>().SwitchCamera(1); //camera de planar
            //rb.gravityScale = 0.12f;
            Color color = asasSpriteRenderer.color;
            color.a = 0f; // 0% de opacidade
            asasSpriteRenderer.color = color;
        }
    }

    public void StopPlanar(){
        if(!estaPlanando){
            return;
        }
        GameObject.FindObjectOfType<CameraManagerScript>().SwitchCamera(0); //volta p a camera normal
        if(asasSpriteRenderer.gameObject.activeSelf){
            Color color = asasSpriteRenderer.color;
            color.a = 1f; // 100% de opacidade
            asasSpriteRenderer.color = color;
        }
        //rb.gravityScale = 1f;
        estaPlanando = false;
    }

    void FixedUpdate()
    {
        Move();
        GroundCheck();
    }

    void Move()
    {
        if(!isBeingPushed){
            rb.velocity = new Vector2(horizontalValue * speed, rb.velocity.y);
        }
        if(estaPlanando){
            rb.velocity = new Vector2(lastHorizontal * (speed * 0.75f), (rb.velocity.y * 0.75f));
        }
        if(!isGrounded && !isBeingPushed && !estaPlanando){
            rb.velocity = new Vector2(horizontalValue * (speed * 0.55f), (rb.velocity.y));
        }
    }

    public void Jump(float jumpPower)
    {
        if(isBeingPushed || (!isGrounded && !infinityJump)){
            return;
        }

        if(estaPlanando){
            StopPlanar();
        }

        float actualJumpForce;
        if(infinityJump){
            if(!isGrounded){
                actualJumpForce = jumpPower * 0.75f;
            }else{
                actualJumpForce = jumpPower * 1f;
            }
        }else{
            actualJumpForce = jumpPower * 1f;
        }

        if(isGrounded || infinityJump){
            if(isGrounded){
                GetComponent<PlayerAnimation>().PlayAnimationByTrigger("JumpTrigger");
            }
            if(infinityJump){
                GetComponent<PlayerAnimation>().PlayAsasAnimation();
            }
            rb.velocity = new Vector2(0f, 0f); // Reseta a velocidade vertical antes do pulo
            rb.AddForce(new Vector2(0f, actualJumpForce), ForceMode2D.Impulse);
        }
    }

    public void IndirectedJump(){
        rb.velocity = new Vector2(rb.velocity.x, 0f); // Reseta a velocidade vertical antes do pulo
        rb.AddForce(new Vector2(0f, jumpForce * 0.75f), ForceMode2D.Impulse);
    }

    void GroundCheck()
    {
        isGrounded = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckCollider.position, 0.2f, groundLayer);
        if (colliders.Length > 0)
        {
            isGrounded = true;

            foreach (var collider in colliders)
            {
                if (collider.CompareTag("MovingPlatform"))
                {
                    transform.parent = collider.transform;
                }
                else
                {
                    transform.parent = null;
                }
            }
        }
    }

    void FlipCheck()
    {
        if (horizontalValue > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (horizontalValue < 0 && isFacingRight)
        {
            Flip();
        }
    }

    public void Flip()
    {
        isFacingRight = !isFacingRight;
        //Vector3 scale = transform.localScale;
        //scale.x *= -1;
        //transform.localScale = scale;
        transform.rotation = Quaternion.Euler(0, isFacingRight ? 0 : 180, 0);
        cameraFollowObject.CallTurn();
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }

    public float GetHorizontalValue()
    {
        return horizontalValue;
    }
}