using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    //Main Menu
    public VisualElement mainMenu;
    private Button _startGameButton;
    private Button _tutorialButton;

    private VisualElement _tutorialMenu;
    private Button _tutorialBackButton;
    private Label _tutorialLabel;

    //Game Over Menu
    public VisualElement gameOverMenu;
    private Button _grestartButton;
    private Button _gquitButton;

    //Game Over Menu
    public VisualElement victoryMenu;
    private Button _vrestartButton;
    private Button _vquitButton;
    public UIManager UIManager { get; set; }
    public CastleController CastleController { get; set; }

    private void Awake()
    {
        UIManager = UIManager ?? UIManager.instance;
        CastleController = CastleController ?? CastleController.instance;

        //Main Menu Initialize
        mainMenu = UIManager.Root.Q<VisualElement>("MainMenu");
        _startGameButton = mainMenu.Q<Button>("MainMenuStartButton");
        _tutorialButton = UIManager.Root.Q<Button>("MainMenuTutorialButton");

        //Tutorial Menu
        _tutorialMenu = UIManager.Root.Q<VisualElement>("TutorialMenu");
        _tutorialBackButton = _tutorialMenu.Q<Button>("TutorialBackButton");
        _tutorialLabel = _tutorialMenu.Q<Label>("TutorialLabel");
        _tutorialLabel.text = @"Welcome to Tower Knights, tower defense. In this game you will start with some resources, shown in the status bar at the top of the screen. These are what you spend to build towers, which are what you build to kill enemies for you, which is what you need to survive until the end of the onslaught and obtain victory! So, pay attention, because you don’t start with much and can easily be overwhelmed.
To build a tower, click on any green hex (one without a road, a resource patch, or colored deathly pale) to open a menu. You can hover over the buttons to get more information about towers, including what they cost to build. Click on one of the buttons in the menu to build a tower on the hex you selected to open that menu. If you decide not to build something now, go ahead and right click to exit out of the menu!
You may have noticed a few tabs on the top of the menu while you were there. They let you see more things to build. The default tab contains four combat towers, used to battle nearby enemies. The second tab (from the left) contains Crafting towers. These occupy the same spaces as combat towers, but do not attack the enemy. Instead, they are used to turn raw resources into other, more refined resources. There’s no other ways to get these, so you need to plan around placing some of these!
The third tab (rightmost) allows you to build resource gathering towers. If you click on that tab you may notice that the buttons are greyed out. This is because these buildings cannot be placed on the regular tiles, each one has a dedicated tile on the map in the form of forests, mountains, and iron… blob thing? (Sorry, I only had so many models in my kit). These buildings give you the raw resources (wood, stone, and ore) which are used in purchasing the lower level towers, and without which you cannot gain refined resources for higher level upgrades. These are critical to your survival, if you can spare the extra resources to build these it is usually a good idea!
Notice I said upgrades. To upgrade a tower, click on an existing tower on the map (anything you placed, just not an empty space). This will open a menu similar to the build menu, where you can upgrade that tower. This can make crafting and resource buildings more efficient, and have different effects on every combat tower, all of which are usually quite good! However, higher level upgrades (some upgrades have one level, others three) tend to cost refined resources, so don’t rely on this immediately in the early game.
Your resource gathering and crafting towers payout at the end of every round, and the enemy will be kind enough to wait for you to click the next round button before continuing. Now is the best time to make a plan, and place your towers!
Survive until the enemy runs out of forces to win, but be warned. They get more guys and bigger baddies over time, so don’t wait until its too late. Build yourself up, until you bring them down! (Or until they get passed you too many times and deplete your Castle’s HP. Those Lightning strikes aren’t free!)
";

        //Game Over Initialize
        gameOverMenu = UIManager.Root.Q<VisualElement>("GameOverMenu");
        gameOverMenu.style.display = DisplayStyle.None;

        _grestartButton = gameOverMenu.Q<Button>("GameOverRestartGameButton");
        _gquitButton = gameOverMenu.Q<Button>("GameOverQuitButton");

        //Victory Initialize
        gameOverMenu = UIManager.Root.Q<VisualElement>("VictoryMenu");
        gameOverMenu.style.display = DisplayStyle.None;

        _vrestartButton = gameOverMenu.Q<Button>("VictoryRestartGameButton");
        _vquitButton = gameOverMenu.Q<Button>("VictoryQuitButton");
    }

    private void OnEnable()
    {
        //Main Menu
        _startGameButton.clicked += StartGame;
        _tutorialButton.clicked += ShowTutorial;

        //Tutorial Menu
        _tutorialBackButton.clicked += BackToMainMenu;

        //Game Over Menu 
        UIManager.RegisterSafeClick(_grestartButton, RestartGame);
        UIManager.RegisterSafeClick(_gquitButton, QuitGame);

        //Victory Menu 
        UIManager.RegisterSafeClick(_grestartButton, RestartGame);
        UIManager.RegisterSafeClick(_gquitButton, QuitGame);

        CastleController.gameOver += OpenGameOverMenu;
        EnemyBaseController.victory += OpenVictoryMenu;
    }

    private void OnDisable()
    {
        _startGameButton.clicked -= StartGame;
        _tutorialButton.clicked -= ShowTutorial;
        _tutorialBackButton.clicked -= BackToMainMenu;
        CastleController.gameOver -= OpenGameOverMenu;
        EnemyBaseController.victory -= OpenVictoryMenu;
    }

    private void OpenGameOverMenu()
    {
        UIManager.OpenMenu(gameOverMenu);
        UIManager.StatusBarContoller.NextRoundButton.style.display = DisplayStyle.None;
        CameraController.instance.enabled = false;
    }

    private void OpenVictoryMenu()
    {
        UIManager.OpenMenu(victoryMenu);
        UIManager.StatusBarContoller.NextRoundButton.style.display = DisplayStyle.None;
        CameraController.instance.enabled = false;
    }

    private void StartGame()
    {
        CameraController.instance.enabled = true;
        SoundManager.instance.PlayBGM(Sounds.background);
        UIManager.StatusBarContoller.NextRoundButton.style.display = DisplayStyle.Flex;
        mainMenu.style.display = DisplayStyle.None;
    }
    private void ShowTutorial()
    {
        mainMenu.style.display = DisplayStyle.None;
        _tutorialMenu.style.display = DisplayStyle.Flex;
    }

    private void BackToMainMenu()
    {
        _tutorialMenu.style.display = DisplayStyle.None;
        mainMenu.style.display = DisplayStyle.Flex;
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
