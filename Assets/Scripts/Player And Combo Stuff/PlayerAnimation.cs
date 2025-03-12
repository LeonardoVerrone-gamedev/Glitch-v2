using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    [SerializeField] Animator asasAnimator;
    private PlayerMovement playerMovement;

    [Header("ParticleSystems")]
    [SerializeField] private GameObject[] particles;

    public bool Planando;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        UpdateAnimations();
    }

    void UpdateAnimations()
    {
        animator.SetFloat("Speed", Mathf.Abs(playerMovement.GetHorizontalValue()));
        animator.SetBool("Jumping", !playerMovement.IsGrounded());
        animator.SetBool("Planando", playerMovement.estaPlanando);
        animator.SetBool("LookingUp", playerMovement.isLookingUp);
        //animator.SetBool("PushBack", playerMovement.isBeingPushed);
    }

    public void TransformAnimation(){
        animator.SetTrigger("Transform");
        //GetComponent<Player>().StopMove();
        //GetComponent<Player>().Invoke("StartMove", 0.8f);
    }

    public void PlayParticleSystem(int index){
        GameObject particle = Instantiate(particles[index], transform.position, Quaternion.identity);
        //particle.transform.parent = transform;
    }

    public void PlayAnimationByTrigger(string trigger){
        animator.SetTrigger(trigger);
    }

    public void PlayAsasAnimation(){
        asasAnimator.SetTrigger("Bate");
    }
}