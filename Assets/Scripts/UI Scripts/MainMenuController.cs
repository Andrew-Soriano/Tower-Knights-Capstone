using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    //Main Menu
    public VisualElement mainMenu;
    private Button _startGameButton;

    //Game Over Menu
    public VisualElement gameOverMenu;
    private Button _restartButton;
    private Button _quitButton;
    public UIManager UIManager { get; set; }
    public CastleController CastleController { get; set; }

    private void Awake()
    {
        UIManager = UIManager ?? UIManager.instance;
        CastleController = CastleController ?? CastleController.instance;

        //Main Menu Initialize
        mainMenu = UIManager.Root.Q<VisualElement>("MainMenu");
        _startGameButton = mainMenu.Q<Button>("MainMenuStartButton");

        //Game Over Initialize
        gameOverMenu = UIManager.Root.Q<VisualElement>("GameOverMenu");
        gameOverMenu.style.display = DisplayStyle.None;

        _restartButton = gameOverMenu.Q<Button>("GameOverRestartGameButton");
        _quitButton = gameOverMenu.Q<Button>("GameOverQuitButton");
    }

    private void OnEnable()
    {
        //Main Menu
        _startGameButton.clicked += StartGame;

        //Game Over Menu 
        UIManager.RegisterSafeClick(_restartButton, RestartGame);
        UIManager.RegisterSafeClick(_quitButton, QuitGame);

        CastleController.gameOver += OpenGameOverMenu;
    }

    private void OpenGameOverMenu()
    {
        UIManager.OpenMenu(gameOverMenu);
        UIManager.StatusBarContoller.NextRoundButton.style.display = DisplayStyle.None;
        CameraController.instance.enabled = false;
    }

    private void OnDisable()
    {

        //Main Menu Buttons
        _startGameButton.clicked -= StartGame;

        CastleController.gameOver -= OpenGameOverMenu;
    }

    private void StartGame()
    {
        CameraController.instance.enabled = true;
        UIManager.StatusBarContoller.NextRoundButton.style.display = DisplayStyle.Flex;
        mainMenu.style.display = DisplayStyle.None;
    }

    private void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    private void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
