using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovesManager : MonoBehaviour
{
    [SerializeField] List<Move> avaliableMoves; //todos os movimentos liberados
    PlayerCombat playerController;
    ControlManager controlManager;

    void Awake()
    {
        playerController = FindObjectOfType<PlayerCombat>();
        controlManager = FindObjectOfType<ControlManager>();

        avaliableMoves.Sort(Compare); //escolhe baseado em prioridade
    }

    public bool CanMove(List<string> keycodes) //returna verdadeiro se não for nulo
    {
        foreach (Move move in avaliableMoves)
        {
            if (move.isMoveAvilable(keycodes))
                return true;
        }
        return false;
    }

    public void PlayMove(List<string> keycodes) 
    {
        foreach (Move move in avaliableMoves)
        {
            if (move.isMoveAvilable(keycodes))
            {
                playerController.PlayMove(move.GetMove(), move.GetMoveComboPriorty());
                break;
            }
        }
    }

    public int Compare(Move move1, Move move2)
    {
        return Comparer<int>.Default.Compare(move2.GetMoveComboPriorty(), move1.GetMoveComboPriorty());
    }
}

