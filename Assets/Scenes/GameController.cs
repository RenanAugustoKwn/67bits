using UnityEngine;

public class GameController : MonoBehaviour
{
    public int coins = 0;
    public int level = 0;
    public GameObject character_man;  // Referência ao personagem
    public Material[] levelMaterials;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void AddCoins(int value)
    {
        coins += value;
        Debug.Log("Total de moedas: " + coins);
    }
    public void BuyLevelUp(int price)
    {
        if (coins < price)
        {
            Debug.Log("Sem moedas suficientes");
            return;
        }

        coins -= price;
        level++;

        Debug.Log($"Level up! Novo nível = {level}");
        ApplyMaterialForLevel();
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
}
