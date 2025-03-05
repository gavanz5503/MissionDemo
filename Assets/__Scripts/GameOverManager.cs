using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public Button playAgainButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameOverPanel.SetActive(false); // Hide panel at game start
        playAgainButton.onClick.AddListener(PlayAgain); // Add button listener
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true); // Show Game Over screen
    }

    void PlayAgain()
    {
        MissionDemolition.S.ResetGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload scene
    }
}
