using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    private List<Ability> abilities = new List<Ability>();
    private Ability currentAbility;
    private List<Ability> currentAbilities = new List<Ability>();
    private int currentAbilityIndex = 0;

    public float manaCost;

    private ManaManager manaManager; // Referência ao gerenciador de mana

    void Start()
    {
        manaManager = GetComponent<ManaManager>(); // Obtém a referência ao gerenciador de mana

        // Adicione a habilidade nula primeiro
        abilities.Add(GetComponent<NullAbility>()); // Adicione a habilidade nula (INDEX0)
        abilities.Add(GetComponent<GhostAbility>()); // Adicione a habilidade fantasma (INDEX1)
        abilities.Add(GetComponent<RunAbility>()); // Adiciona a habilidade de corrida (INDEX2)
        abilities.Add(GetComponent<FlyAbility>()); // Adiciona a habilidade de voar (INDEX3)

        // Ative a primeira habilidade (nula)
        PlayAbility(0);
    }

    public void PlayAbility(int abilityIndex){
        if(abilityIndex == 0){
            SwitchToNullAbility();
            return;
        }
        if(currentAbilities.Contains(abilities[abilityIndex])){
            abilities[abilityIndex].Deactivate();
            currentAbilities.Remove(abilities[abilityIndex]);
            return;
        }
        if(currentAbilities.Contains(abilities[0])){
            abilities[0].Deactivate();
            currentAbilities.Remove(abilities[0]);
        }
        currentAbilities.Add(abilities[abilityIndex]);
        abilities[abilityIndex].Activate();
        manaCost = 0f;
        foreach(Ability ability in currentAbilities){
            manaCost += ability.GetManaCost();
        }
    }
    
    private void Update()
    {
        // Verifica se a mana é suficiente para a habilidade atual
        
        if (manaCost > 0)
        {
            // Custo de mana positivo
            manaManager.UseMana(manaCost * Time.deltaTime);
        }
        else
        {
            // Custo de mana negativo (recarga de mana)
            manaManager.GainMana(-manaCost * Time.deltaTime);
        }

        // Verifica se a mana chegou a 0
        if (manaManager.currentMana <= 0) // 0 é o índice da habilidade nula
        {
            SwitchToNullAbility();
        }
        if(currentAbilities.Count == 0){
            SwitchToNullAbility();
        } 

        CheckActiveAbilities();

    }

    public void SwitchToNullAbility()
    {
        manaCost = 0f;
        foreach(Ability activatedAbility in currentAbilities)
        {
            activatedAbility.Deactivate();
            currentAbilities.Remove(activatedAbility);
        }
        currentAbilityIndex = 0; // Define a habilidade nula
        abilities[0].Activate();
        manaCost = abilities[0].GetManaCost();
    }

    void CheckActiveAbilities(){
        foreach(Ability ability in currentAbilities)
        {
            if(ability.isActive() == false){
                currentAbilities.Remove(ability);
                ResetMana();
            }
        }
    }

    void ResetMana(){
        manaCost = 0f;
        foreach(Ability ability in currentAbilities){
            manaCost += ability.GetManaCost();
        }
    }
}