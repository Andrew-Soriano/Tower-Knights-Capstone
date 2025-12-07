using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    //Main Menu
    public VisualElement mainMenu;
    private Button _startGameButton;
    private Button _tutorialButton;
    private Button _creditsButton;

    private VisualElement _tutorialMenu;
    private Button _tutorialBackButton;
    private Label _tutorialLabel;

    private VisualElement _creditsMenu;
    private Button _creditsBackButton;
    private Label _creditsLabel;

    //Game Over Menu
    public VisualElement gameOverMenu;
    private Button _grestartButton;
    private Button _gquitButton;

    //Victory Menu
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
        _creditsButton= UIManager.Root.Q<Button>("MainMenuCreditsButton");

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
        _creditsMenu = UIManager.Root.Q<VisualElement>("CreditsMenu");
        _creditsBackButton = _creditsMenu.Q<Button>("CreditsBackButton");
        _creditsLabel = _creditsMenu.Q<Label>("CreditsLabel");
        _creditsLabel.text = @"Lead Programmer (With a “Surprise” Lack of Junior Programmers)
	Andrew Soriano
Models From:
	Kay Lousberg, under the Creative Commons Zero license.
	Specific sets of KayKit Products Used Include:
KayKit - Character Pack : Adventurers - https://kaylousberg.itch.io/kaykit-adventurers
KayKit - Mystery Monthly : Series 4 - https://kaylousberg.itch.io/kaykit-series-4
KayKit - Mystery Monthly : Series 5 - https://kaylousberg.itch.io/kaykit-series-5
KayKit - Character Animations - https://kaylousberg.itch.io/kaykit-character-animations
KayKit - Medieval Hexagon Pack - https://kaylousberg.itch.io/kaykit-medieval-hexagon
KayKit : Resource Bits (Mostly Used as Screenshots, but the Screenshots came From 3D Models in Unity under this License) - https://kaylousberg.itch.io/resource-bits
KayKit - Character Pack : Skeletons - https://kaylousberg.itch.io/kaykit-skeletons
	Etienne Pouvreau- Sylized bomb and explosion VFX unity.  I’m honestly not sure what license, but his itch.io page tags it as royalty free - https://smolware.itch.io/sylized-bomb-and-explosion-vfx-unity
	Renderer Knight (Real Name Not Given) - Fantasy Skybox FREE   - A Skybox licensed under the Standard Unity Asset Store EULA - https://assetstore.unity.com/packages/2d/textures-materials/sky/fantasy-skybox-free-18353
CraftPix (Real Name not Provided) - Fantasy RPG Icons Mega Collection – Icons used in game, mostly for upgrades. Made available under Unity the Standard Unity Asset Store EULA here - https://assetstore.unity.com/packages/2d/gui/icons/5800-fantasy-rpg-icons-pack-229460
And sounds from the incredible people on freesound.org below:
Footstep_Grass.wav by FallujahQc -- https://freesound.org/s/403163/ -- License: Attribution 3.0
Rattling Bones.wav by spookymodem -- https://freesound.org/s/202102/ -- License: Creative Commons 0
lightning strike.wav by parnellij -- https://freesound.org/s/74892/ -- License: Creative Commons 0
Knight Right Footstep on Gravel 1 (With Chainmail) by Ali_6868 -- https://freesound.org/s/384887/ -- License: Creative Commons 0
ICEMisc-Hitting A Bag Of Ice_CRB_OwSfx_Loop by Christiaan_Bot00+ -- https://freesound.org/s/766116/ -- License: Creative Commons 0
Piano loops 190 octave up long loop 120 bpm by josefpres -- https://freesound.org/s/831895/ -- License: Creative Commons 0
Piano loops 193 octave down short loop 120 bpm by josefpres -- https://freesound.org/s/832667/ -- License: Creative Commons 0
Arrow.mp3 by thecrow_br -- https://freesound.org/s/574044/ -- License: Creative Commons 0
Arrow Release and Hit.wav by calvarychurchatlanta -- https://freesound.org/s/179996/ -- License: Creative Commons 0
Retro, Bomb Explosion.wav by LilMati -- https://freesound.org/s/522572/ -- License: Creative Commons 0
Bomb, grenade, shot at enemy.wav by Puerta118m -- https://freesound.org/s/471642/ -- License: Creative Commons 0
Small bomb with hit by rodrigocswm -- https://freesound.org/s/449434/ -- License: Creative Commons 0
Short twangs cleaned.wav by cubic.archon -- https://freesound.org/s/44192/ -- License: Creative Commons 0

And help from viewers like you (I think?)
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
        _creditsButton.clicked += ShowCredits;

        //Tutorial Menu
        _tutorialBackButton.clicked += BackToMainMenu;

        //Tutorial Menu
        _creditsBackButton.clicked += BackToMainMenu;

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
        _creditsButton.clicked -= ShowCredits;
        _tutorialBackButton.clicked -= BackToMainMenu;
        _creditsBackButton.clicked -= BackToMainMenu;
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

    private void ShowCredits()
    {
        mainMenu.style.display = DisplayStyle.None;
        _creditsMenu.style.display = DisplayStyle.Flex;
    }


    private void BackToMainMenu()
    {
        _tutorialMenu.style.display = DisplayStyle.None;
        _creditsMenu.style.display = DisplayStyle.None;
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
