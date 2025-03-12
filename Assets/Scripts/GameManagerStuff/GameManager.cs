using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject learningAndPauseUIPanel; // O painel da UI que você deseja abrir/fechar
    private bool isUIPanelActive = false;

    [SerializeField]GameObject playerPrefab;
    [SerializeField]Transform playerStartPosition;

    public void ToggleUIPanel()
    {
        isUIPanelActive = !isUIPanelActive;
        learningAndPauseUIPanel.SetActive(isUIPanelActive);
        Time.timeScale = isUIPanelActive ? 0 : 1; // Congela o jogo se a UI estiver ativa
    }

    public bool IsUIPanelActive()
    {
        return isUIPanelActive;
    }

    void Start(){
        //Instantiate(playerPrefab, playerStartPosition.position, Quaternion.identity);
    }
}
