using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using System;

public class GameController : MonoBehaviour
{
    public int coins = 0;
    public int level = 1;
    public GameObject character_man;  // Referência ao personagem
    public Material[] levelMaterials;
    public TextMeshProUGUI coinstextMeshPro;
    public TextMeshProUGUI leveltextMeshPro;
    public GameObject panel;
    private bool isPaused = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.JoystickButton3)) OpenPainel();
        if (isPaused && Input.GetKeyDown(KeyCode.JoystickButton0)) BuyLevelUp(3);
    }
    public void OpenPainel()
    {
        TogglePause();
    }
    public void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            panel.SetActive(true);
            DisableStackInertiaScripts();
        }
        else {
            panel.SetActive(false);
            EnableStackInertiaScripts();
        }
            Time.timeScale = isPaused ? 0f : 1f;

    }
    public void AddCoins(int value)
    {
        coins += value;
        AttText();
        Debug.Log("Total de moedas: " + coins);

    }
    public void BuyLevelUp(int price)
    {
        if (coins < price)
        {
            Debug.Log("Sem moedas suficientes");
            panel.SetActive(false);
            TogglePause();
            return;
        }

        coins -= price;
        level++;

        AttText();
        ApplyMaterialForLevel();
        panel.SetActive(false);
        TogglePause();
        Debug.Log($"Level up! Novo nível = {level}");
    }
    private void AttText()
    {
        coinstextMeshPro.text = "Moedas = " + coins.ToString();
        leveltextMeshPro.text = "Level = " + level.ToString();
    }
    private void ApplyMaterialForLevel()
    {
        if (!character_man)
        {
            Debug.LogWarning("character_man não atribuído!");
            return;
        }

        SkinnedMeshRenderer smr = character_man.GetComponentInChildren<SkinnedMeshRenderer>();
        if (!smr)
        {
            Debug.LogWarning("SkinnedMeshRenderer não encontrado!");
            return;
        }

        int idx = level - 1;

        if (idx >= levelMaterials.Length)
        {
            Debug.LogWarning("levelMaterials não possui material para este nível!");
            return;
        }

        // Cria um novo array de materiais com o mesmo comprimento do atual
        Material[] newMats = new Material[smr.materials.Length];

        // Atribui o novo material para todos os slots
        for (int i = 0; i < newMats.Length; i++)
            newMats[i] = levelMaterials[idx];

        smr.materials = newMats;
    }
    void DisableStackInertiaScripts()
    {
        StackInertia[] inertiaScripts = FindObjectsByType<StackInertia>(FindObjectsSortMode.None);
        foreach (StackInertia script in inertiaScripts)
        {
            script.enabled = false;
        }
    }

    void EnableStackInertiaScripts()
    {
        StackInertia[] inertiaScripts = FindObjectsByType<StackInertia>(FindObjectsSortMode.None);
        foreach (StackInertia script in inertiaScripts)
        {
            script.enabled = true;
            script.ResetPreviousPosition();
        }
    }
}
