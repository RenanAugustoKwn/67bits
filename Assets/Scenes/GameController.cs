using UnityEngine;

public class GameController : MonoBehaviour
{
    public int coins = 0;
    public int level = 0;
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
}
