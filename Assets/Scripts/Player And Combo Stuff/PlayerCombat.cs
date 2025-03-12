using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    public ParticleSystem chargeEffect;

    [SerializeField]GameObject gunPrefab;
    [SerializeField]GameObject chargedGunPrefab;
    [SerializeField] Transform[] gunTransformPosition;

    [SerializeField] Animator gunAnim;
    
    [Header("Combo stuff")]
    public bool _isInTransformMode;
    public bool isRunning;
    public bool isIntangible;
    private AbilityManager abilityManager;

    public int CurrentComboPriorty = 0;
    [SerializeField] ControlManager controlManager;

    public Move fastMoveRight;
    public Move fastMoveLeft;

    [SerializeField]private float InstantCoolDown = .22f;
    private bool StartInstantCoolDown;
    [SerializeField]private float ShootCoolDown = .05f;
    private bool StartShootCoolDown;
    [SerializeField] bool canShoot = true;

    private ManaManager manaManager;

    public Transform groundCheckPoint;


    void Start(){
        //animator = GetComponent<Animator>();
        controlManager = FindObjectOfType<ControlManager>();
        abilityManager = GetComponent<AbilityManager>();
        manaManager = GetComponent<ManaManager>();
    }

    void Update()
    {
        if(StartInstantCoolDown){
            InstantCoolDown -= Time.deltaTime;

            if(InstantCoolDown <= 0f){
                StartInstantCoolDown = false;
                InstantCoolDown = .22f;
            }
        }
    
        if(!GetComponent<PlayerMovement>().isGrounded){
            CauseDamageJump();
        }
    }

    void CauseDamageJump(){
        if(GetComponent<PlayerMovement>().isGrounded){
            return;
        }
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(groundCheckPoint.position, 2f, LayerMask.GetMask("Enemy"));
        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyLife enemyScript = enemy.GetComponent<EnemyLife>();
            if (enemyScript != null)
            {
                if(enemyScript.canBeSmashed == true){
                    enemyScript.TakeDamage(10);
                    GetComponent<PlayerMovement>().IndirectedJump();
                }
            
            }
        }
    }

    public void FastAbilityRight(){
        PlayMove(fastMoveRight.GetMove(), 10);
    }

    public void FastAbilityLeft(){
        PlayMove(fastMoveLeft.GetMove(), 10);
    }

    public void SimpleShoot(){
        //CHECAR DIREÇÃO DO TIRO: cima caso olhando para cima e baixo caso planando
        if(canShoot){
            //gun.Play();
            if(GetComponent<PlayerMovement>().estaPlanando == true){
                GameObject particle = Instantiate(gunPrefab, gunTransformPosition[1].position, gunTransformPosition[1].rotation); //ATIRA P BAIXO
            }else if(GetComponent<PlayerMovement>().isLookingUp == true){
                GameObject particle = Instantiate(gunPrefab, gunTransformPosition[2].position, gunTransformPosition[2].rotation); //ATIRA P CIMA
            }else{
                GameObject particle = Instantiate(gunPrefab, gunTransformPosition[0].position, gunTransformPosition[0].rotation); //ATIRA HORIZONTAL
            }
            gunAnim.SetTrigger("Shoot");
        }
    }

    public void prepareChargedAttack(){
        //chargeEffect.gameObject.SetActive(true);
        if(GetComponent<PlayerMovement>().estaPlanando || GetComponent<PlayerMovement>().isLookingUp){
            return;
        }
        chargeEffect.Play();
    }

    public void cancelChargedAttack(){
        chargeEffect.Stop();
    }

    public void ChargedAttack(){
        chargeEffect.Stop();
        if(GetComponent<PlayerMovement>().estaPlanando || GetComponent<PlayerMovement>().isLookingUp){
            return;
        }
        //chargeEffect.gameObject.SetActive(false);
        //chargedGun.Play();
        GameObject particle = Instantiate(chargedGunPrefab, gunTransformPosition[0].position, transform.rotation);
    }

    #region Special Attacks Area

    public void PlayMove(Moves move, int ComboPriorty)
    {
        if (Moves.None != move)
        {
            if (ComboPriorty >= CurrentComboPriorty)
            {
                CurrentComboPriorty = ComboPriorty;

                switch (move)
                {
                    case Moves.Transform:
                        //transformação
                        if(!_isInTransformMode){
                            _isInTransformMode = true;
                            GetComponent<PlayerAnimation>().TransformAnimation();
                            //abilityManager.SwitchAbility();
                        } // muda a habilidade permanente atual do error
                        break;

                    case Moves.teletransporte:
                        Teletransporte();
                        break;
                        // Adicione mais ataques especiais aqui
                    case Moves.NullAbility:
                        abilityManager.PlayAbility(0);
                        break;
                    case Moves.GhostCode:
                        abilityManager.PlayAbility(1);
                        isIntangible = true;
                        break;
                    case Moves.RunCode:
                        abilityManager.PlayAbility(2);
                        isRunning = true;
                        break;
                    case Moves.FlyCode:
                        abilityManager.PlayAbility(3);
                        break;
                }
            }else
                return;

            CurrentComboPriorty = 0;
            
        }
    }
    //TELETRANSPORTES
    public void Teletransporte(){
        manaManager.UseMana(4f);
        if(StartInstantCoolDown){
            return;
        }
        StartInstantCoolDown = true;
        float TPforce;
        if(GetComponent<PlayerMovement>().verticalValue == 0f){
            if(GetComponent<PlayerMovement>().isFacingRight){
                TPforce = 7f;
            }else{
                TPforce = -7f;
            }
            GetComponent<PlayerAnimation>().PlayParticleSystem(1);
            this.gameObject.GetComponent<Transform>().position = new Vector2((transform.position.x + TPforce), transform.position.y);
        }else{
            if(GetComponent<PlayerMovement>().verticalValue > 0f){
                TPforce = 5f;
            }else{
                TPforce = -5f;
            }
            this.gameObject.GetComponent<Transform>().position = new Vector2(transform.position.x, (transform.position.y + TPforce));
        }
    }

    #endregion
}