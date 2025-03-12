using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class ControlManager : MonoBehaviour
{
    [SerializeField] float ComboResetTime = 0.5f; //The Time to reset the Combo Time
    [SerializeField] public List<string> KeysPressed; //List of all the Keys Pressed so far
    public float Hvalue;
    public float Vvalue;
    [SerializeField] TextMeshProUGUI controlsTestText; //Just for testing for printing the keys

    public bool AxisHinUse;
    public bool AxisVinUse;

    public GameObject player;

    public string allKeysPressed;

    MovesManager movesManager;

    public GameObject holder;

    public GameObject Up;
    public GameObject Down;
    public GameObject Left;
    public GameObject Right;
    public GameObject B;
    public GameObject X;
    public GameObject A;

    public bool hasBeenAdded;

    void Awake()
    {
        if (movesManager == null)
            movesManager = FindObjectOfType<MovesManager>();
    }

    void Start(){
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        //if(player.GetComponent<PlayerMovement>().canMove == true){
        //ResetCheck(); //Get the Pressed Key
        //}

        //controlsTestText.text = allKeysPressed; 

        PrintControls(); //Just for testing
    }


    public void ResetCheck(){
        if (!movesManager.CanMove(KeysPressed)) //if there is no avilable Moves reset the list
            StopAllCoroutines();

        StartCoroutine(ResetComboTimer()); //Start the Reseting process
    }

    public void ResetCombo() //Called to Reset the Combo after a move
    {
        KeysPressed.Clear();
        Debug.Log("Cleared");
        allKeysPressed = null;
    }

    IEnumerator ResetComboTimer()
    {
        yield return new WaitForSeconds(ComboResetTime);

        movesManager.PlayMove(KeysPressed); //Run the move from the list
        KeysPressed.Clear(); //Empty the list
        allKeysPressed = null;
    }

    public void PrintControls() //spawn used key in UI
    {
        //controlsTestText.text = "combo: ";
        foreach(Transform child in holder.transform){
            Destroy(child.gameObject);
        }
        foreach (string kcode in KeysPressed){
            if(kcode == "Up"){
                //Instantiate(Up, holder.transform);
            }
            if(kcode == "Down"){
                //Instantiate(Down, holder.transform);
            }
            if(kcode == "Left"){
                //Instantiate(Left, holder.transform);
            }
            if(kcode == "Right"){
                //Instantiate(Right, holder.transform);
            }
            if(kcode == "B"){
                //Instantiate(B, holder.transform);
            }
            if(kcode == "A"){
                //Instantiate(A, holder.transform);
            }
            if(kcode == "X"){
                //Instantiate(X, holder.transform);
            }
            Debug.Log(kcode);
        }
    }

}