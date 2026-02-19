using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int score = 0;
    public float playerHP = 100f;
    public float maxHP = 100f;

    void Awake()
    {
        Instance = this;
    }

    public void AddScore(int amount)
    {
        score += amount;
        Debug.Log("Score: " + score);
    }

    public void TakeDamage(float amount)
    {
        playerHP -= amount;
        Debug.Log("Player HP: " + playerHP);
        if (playerHP <= 0f)
            Debug.Log("GAME OVER");
    }
}