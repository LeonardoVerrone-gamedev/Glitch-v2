using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FastSelectionAbilityMenu : MonoBehaviour
{
    public GameObject movePrefab;
    public Transform HolderObject;
    public bool RB;
    public List<Move> moves;
    
    GameObject objetoSelecionado;
    public PlayerCombat player;

    public ScrollRect scroll;
    public float horizontal;

    public GameObject menu;
    public bool inMenu = false; 

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCombat>();
        //ResetInventory();
    }

    void Update()
    {
        if(!inMenu){
            return;
        }
        objetoSelecionado = EventSystem.current.currentSelectedGameObject;
        
        if(horizontal > 0){
            //rola pra esquerda
            scroll.horizontalNormalizedPosition += 0.01f;
        }else if(horizontal < 0){
            scroll.horizontalNormalizedPosition -= 0.01f;
        }
    }

    public void OpenMenu(){
        menu.SetActive(true);
        inMenu = true;
        ResetInventory();
        scroll.horizontalNormalizedPosition -= 100f;
    }

    public void Cancel(){
        foreach(Transform child in HolderObject){
            Destroy(child.gameObject);
        }
        inMenu = false;
        menu.SetActive(false);
    }

    public void ResetInventory(){
        foreach(Transform child in HolderObject){
            Destroy(child.gameObject);
        }
        GameObject firstItem = Instantiate(movePrefab);
        firstItem.transform.SetParent(HolderObject);
        firstItem.transform.localPosition = Vector3.zero;
        if(RB){
            firstItem.GetComponent<itemPrefabScript>().thisMove = player.fastMoveRight;
        }else{
            firstItem.GetComponent<itemPrefabScript>().thisMove = player.fastMoveLeft;
        }
        foreach(Move ability in moves){
            if(ability != firstItem.GetComponent<itemPrefabScript>().thisMove){
                GameObject item = Instantiate(movePrefab);
                item.transform.SetParent(HolderObject);
                item.transform.localPosition = Vector3.zero;
                item.GetComponent<itemPrefabScript>().thisMove = ability;
            }
        }
        //Transform firstChild = HolderObject.GetChild(0);
        EventSystem.current.SetSelectedGameObject(firstItem);
    }

    public void SetFastAbility(){
        if(RB){
            player.fastMoveRight = objetoSelecionado.GetComponent<itemPrefabScript>().thisMove;
        }else{
            player.fastMoveLeft = objetoSelecionado.GetComponent<itemPrefabScript>().thisMove;
        }
        Cancel();
        inMenu = false;
    }
}
