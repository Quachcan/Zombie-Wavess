using EnemyScripts;
using PlayerScripts;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        private GameState currentState;
        
        public PlayerMovement playerMovement;
        
        [SerializeField]
        private EnemyMovement enemyMovement;
        [SerializeField]
        private EnemySpawner enemySpawner;
        [SerializeField]
        private bool isGameOver;
        

        private void OnEnable()
        {
            Instance = this;
        }

        private void OnDisable()
        {
            Instance = null;
        }

        void Start()
        {
            isGameOver = false;
            SetGameState(GameState.Playing);
            Initialize();
            TimeManager.Instance.StartTimer();
        }

        private void Initialize()
        {
            Player.Instance.Initialize();
            PlayerCombat.Instance.Initialize();
            EnemySpawner.Instance.Initialize();
        }
        
        public void GameOver()
        {
            if(isGameOver)
                return;
            isGameOver = true;
            HandleGameOverState();
            UIManager.Instance.ShowGameOverPanel();
        }

        public void RestartGame()
        {
            UIManager.Instance.HideGameOverPanel();
            SceneManagers.Instance.ReloadScene();
        }

        public void OnTimeUp()
        {
            if (isGameOver) return;
            isGameOver = true;
            HandleGameOverState();
            //OnPlayerWin();
            UIManager.Instance.ShowVictoryPanel();
        }
        
        public void OnPlayerTakeDamage(int currentHealth)
        {
            UIManager.Instance.health = currentHealth;
            UIManager.Instance.UpdateHealth();
        }

        private void SetGameState(GameState newState)
        {
            currentState = newState;
            Debug.Log($"Game state changed to {currentState}");

            switch (newState)
            {
                case GameState.Playing:
                    HandlePlayingState();
                    break;
                case GameState.Paused:
                    HandlePausedState();
                    break;
                case GameState.GameOver:
                    HandleGameOverState();
                    break;
            }
        }
        

        private void HandlePlayingState()
        {
            Time.timeScale = 1;
        }

        private void HandlePausedState()
        {
            Time.timeScale = 0;
        }

        private void HandleGameOverState()
        {
            Time.timeScale = 0;
        }

        public void PauseGame()
        {
            if (currentState == GameState.Playing)
            {
                SetGameState(GameState.Paused);
            }
        }

        public void ResumeGame()
        {
            if (currentState == GameState.Paused)
            {
                SetGameState(GameState.Playing);
            }
        }

        public void SaveGame()
        {
            Player.Instance.SavePlayer();
        }

        public void LoadGame()
        {
            Player.Instance.LoadPlayer();
        }

        public void OnPlayerWin(int rewardAmount)
        {
            SaveGame();
            CurrencyManager.Instance.AddCoin(rewardAmount);
        }

        public void UpdatePlayerExp(int currentExp, int expToNextLevel, int currentLevel)
        {
            UIManager.Instance.UpdateExpUI(currentExp, expToNextLevel, currentLevel);
        }
    }

    public enum GameState
    {
        Playing,
        Paused,
        GameOver
    }
}
