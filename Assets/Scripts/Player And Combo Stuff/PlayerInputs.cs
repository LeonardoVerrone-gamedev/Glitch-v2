using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    public PlayerCombat playerCombat;
    public PlayerMovement playerMovement;
    public ControlManager controlManager;
    public FastSelectionAbilityMenu fastSelectionMenu;

    public InputAction playerControls;

    public bool canMove;

    private float horizontal;
    public float lastHorizontal = 0f;

    public float vertical;

    [SerializeField]private Vector2 cameraVector;
    [SerializeField]private CameraFollowObject cameraFollowObject;

    private float switchAbilityValor;

    [SerializeField]private float buttonPressTime;
    private const float longPressThreshold = 0.5f; // Tempo para considerar um pressionamento longo
    private const float doublePressThreshold = 0.3f;// Tempo para considerar um pressionamento curto
    [SerializeField]private bool isLongPress = false;


    void Start()
    {
        fastSelectionMenu = FindObjectOfType<FastSelectionAbilityMenu>();
        controlManager = FindObjectOfType<ControlManager>();
        cameraFollowObject = FindObjectOfType<CameraFollowObject>();
    }

    public void CanMove_SetTrue(){
        Debug.Log("can move setado para true");
        canMove = true;
    }
    public void CanMove_SetFalse(){
         Debug.Log("can move setado para falso");
        canMove = false;
    }

    void Update()
    {
        if(canMove){
            playerMovement.horizontalValue = horizontal;
            playerMovement.verticalValue = vertical;
            if(horizontal == 0f && playerCombat.isRunning){
                GetComponent<RunAbility>().Deactivate();
                playerCombat.isRunning = false;
            }
        }

        cameraFollowObject.cameraManualMoveVector = new Vector3(cameraVector.x, cameraVector.y, 0f);

        if(fastSelectionMenu.inMenu){
            canMove = false;
            fastSelectionMenu.horizontal = horizontal;
        }
    }

    public void Move(InputAction.CallbackContext context){
        if(horizontal > 0){
            lastHorizontal = 1f;
        }else{
            lastHorizontal = -1f;
        }
        playerMovement.lastHorizontal = lastHorizontal;
        horizontal = context.ReadValue<Vector2>().x;
        vertical = context.ReadValue<Vector2>().y;
    }

    public void CameraMove(InputAction.CallbackContext context){
        cameraVector = new Vector2(context.ReadValue<Vector2>().x, context.ReadValue<Vector2>().y);
    }

    public void OnUseFastAbilityLB(InputAction.CallbackContext context) {
    if (context.started) {
        Debug.Log("OnUseFastAbilityLB");
        if (!canMove) {
            return;
        }
        playerCombat.FastAbilityLeft(); // Executa a habilidade rápida ao pressionar
    }
}

public void OnUseFastAbilityRB(InputAction.CallbackContext context) {
    if (context.started) {
        Debug.Log("OnUseFastAbilityRB");
        if (!canMove) {
            return;
        }
        playerCombat.FastAbilityRight(); // Executa a habilidade rápida ao pressionar
    }
}

public void OnOpenFastAbilityMenuRB(InputAction.CallbackContext context) {
    if (context.performed) {
        Debug.Log("OnOpenFastAbilityMenuRB");
        if (!fastSelectionMenu.inMenu) {
            fastSelectionMenu.OpenMenu(); // Abre o menu ao pressionar
            fastSelectionMenu.RB = true; // Define que é o botão direito
        }
    }

    if (context.canceled) {
        Debug.Log("Closing Fast Ability Menu RB");
        if (fastSelectionMenu.inMenu) {
            fastSelectionMenu.SetFastAbility(); // Faz a seleção ao soltar
            canMove = true; // Permite que o jogador se mova novamente
        }
    }
}

public void OnOpenFastAbilityMenuLB(InputAction.CallbackContext context) {
    if (context.performed) {
        Debug.Log("OnOpenFastAbilityMenuLB");
        if (!fastSelectionMenu.inMenu) {
            fastSelectionMenu.OpenMenu(); // Abre o menu ao pressionar
            fastSelectionMenu.RB = false; // Define que é o botão esquerdo
        }
    }

    if (context.canceled) {
        Debug.Log("Closing Fast Ability Menu LB");
        if (fastSelectionMenu.inMenu) {
            fastSelectionMenu.SetFastAbility(); // Faz a seleção ao soltar
            canMove = true; // Permite que o jogador se mova novamente
        }
    }
}
    //public void Fire(InputAction.CallbackContext context){
        //if(context.performed && canMove){
            //playerCombat.HandleShooting(true);
            //controlManager.KeysPressed.Add("RightTrigger");
            //controlManager.ResetCheck();
        //}
        //if(context.canceled){
            //playerCombat.HandleShooting(false);
        //}
    //}

    public void A_Button(InputAction.CallbackContext context){
        if(!canMove){
            return;
        }
        if(context.performed){
            playerMovement.Jump(playerMovement.jumpForce);
            controlManager.KeysPressed.Add("A");
            controlManager.ResetCheck();
        }
    }

    public void B_Button(InputAction.CallbackContext context){
        if(fastSelectionMenu.inMenu){
            fastSelectionMenu.Cancel();
            canMove = true;
        }
        if(!canMove){
            return;
        }
        if(context.performed){
            //GetComponent<AbilityManager>().SwitchToNullAbility();
            controlManager.KeysPressed.Add("B");
            controlManager.ResetCheck();
        }
    }

    public void Tap_X_Button(InputAction.CallbackContext context){
        if(!canMove){
            return;
        }
        if(context.performed){
            controlManager.KeysPressed.Add("X");
            playerCombat.SimpleShoot();
            controlManager.ResetCheck();
        }

    }

    public void Hold_X_Button(InputAction.CallbackContext context){
        if(!canMove){
            return;
        }
        if(context.started){
            playerCombat.prepareChargedAttack();
        }
        if(context.canceled){
            if(context.duration > .3f){
                playerCombat.ChargedAttack();
            }else{
                playerCombat.cancelChargedAttack();
            }
        }
    }

    public void Y_Button(InputAction.CallbackContext context){
        if(!canMove){
            return;
        }
        if(context.performed){
            controlManager.KeysPressed.Add("Y");
            controlManager.ResetCheck();
        }
    }

    public void Up_Button(InputAction.CallbackContext context){
        if(!canMove){
            return;
        }
        if(context.performed){
            playerMovement.isLookingUp = true;
            controlManager.KeysPressed.Add("Up");
            controlManager.ResetCheck();
        }
        if(context.canceled){
            playerMovement.isLookingUp = false;
        }
    }

    public void Down_Button(InputAction.CallbackContext context){
        if(!canMove){
            return;
        }
        if(context.performed){
            playerMovement.Planar();
            controlManager.KeysPressed.Add("Down");
            controlManager.ResetCheck();
        }
    }

    public void Left_Button(InputAction.CallbackContext context){
        if(!canMove){
            return;
        }
        if(context.performed){
            controlManager.KeysPressed.Add("Left");
            controlManager.ResetCheck();
        }
    }

    public void Right_Button(InputAction.CallbackContext context){
        if(!canMove){
            return;
        }
        if(context.performed){
            controlManager.KeysPressed.Add("Right");
            controlManager.ResetCheck();
        }
    }

    public void ResetControlManagerCheck(InputAction.CallbackContext context){
        if(context.performed){
            controlManager.ResetCheck();
        }
    }
}
