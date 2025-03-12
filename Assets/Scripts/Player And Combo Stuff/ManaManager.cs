using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaManager : MonoBehaviour
{
    public float currentMana = 0f;
    public float maxMana = 100f;
    public float manaGainRate = 1f; // Mana ganha por segundo
    [SerializeField] private Image manaImage; // Referência à imagem de mana

    void Start()
    {
        GameObject manaBarContent = GameObject.Find("ManaBarContent");
        manaImage = manaBarContent.GetComponent<Image>();

        // Inicializa a imagem de mana
        if (manaImage != null)
        {
            manaImage.fillAmount = currentMana / maxMana;
        }
    }

    void Update()
    {
        // Atualiza a imagem de mana
        if (manaImage != null)
        {
            manaImage.fillAmount = currentMana / maxMana;
        }
    }

    public void GainMana(float amount)
    {
        currentMana += amount;
        if (currentMana > maxMana)
        {
            currentMana = maxMana; // Limita a mana ao máximo
        }
    }

    public void UseMana(float amount)
    {
        currentMana -= amount;
        if (currentMana < 0)
        {
            currentMana = 0; // Limita a mana ao mínimo
        }
    }
}