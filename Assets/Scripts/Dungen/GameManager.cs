using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using static BattleManager;
using UnityEngine.SceneManagement;
using static UIManager;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class GameManager : MonoBehaviour
{

    //Time to wait before starting level, in seconds.
    public float levelStartDelay = 2f;
    //Delay between each Player turn.
    public float turnDelay = 0.1f;
    //Starting value for Player food points.
    public int playerFoodPoints = 100;
    //Static instance of GameManager which allows it to be accessed by any other script.

    //Static instance of GameManager which allows it to be accessed by any other script.
    public static GameManager instance = null;

    //Boolean to check if it's players turn, hidden in inspector but public.
    //[HideInInspector] public bool playersTurn = true;
    public bool playersTurn = true;

    public List<HeroData> heroDatas;


    //Text to display current level number.
    private Text levelText;
    //Image to block out level as levels are being set up, background for levelText.
    private GameObject levelImage;

    //Store a reference to our BoardManager which will set up the level.
    private BoardManager boardScript;
    //Current level number, expressed in game as "Day  1".
    public int level = 0;

    //List of all Enemy units, used to issue them move commands.
    private List<Enemy> enemies;
    //Boolean to check if enemies are moving.
    public bool enemiesMoving;
    //Boolean to check if we're setting up board, prevent Player from moving during setup.
    private bool doingSetup = true;

    //Hero team list
    public List<BattleManager.heroTypes> heroList;
    //Enemy team list
    public List<BattleManager.enemyTypes>  enemyList;

    //the enemy which is fighting with the player.
    public Enemy currentEnemy;

    private void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        //Assign enemies to a new List of Enemy objects.
        enemies = new List<Enemy>();

        //Get a component reference to the attached BoardManager script
        boardScript = GetComponent<BoardManager>();

        //temp code to add units.
        InitiateHeroTeam();
        //Call the InitGame function to initialize the first level 
        InitGame();

        //Debug.logger.logEnabled = false;

        SceneManager.sceneLoaded += LevelUpScene;
    }

    private void Start()
    {
        if (BattleManager.instance != null)
        {
            BattleManager.instance.gameObject.SetActive(false);
        }
        playerFoodPoints = 100;
    }

    //This is called each time a scene is loaded.
    void LevelUpScene(Scene scene, LoadSceneMode mode)
    {
        //Add one to our level number.
        if(scene.name == "Dungen")
        {
            level++;
            Debug.Log("levelupscene");
            if (BattleManager.instance != null)
            {
                BattleManager.instance.gameObject.SetActive(false);
            }
            //Call InitGame to initialize our level.
            InitGame();
        }
        
    }


    //Initializes the game for each level.
    void InitGame()
    {
        
        //While doingSetup is true the player can't move, prevent player from moving while title card is up.
        doingSetup = true;

        //Get a reference to our image LevelImage by finding it by name.
        levelImage = GameObject.Find("LevelImage");

        //Get a reference to our text LevelText's text component by finding it by name and calling GetComponent.
        levelText = GameObject.Find("LevelText").GetComponent<Text>();

        //Set the text of levelText to the string "Day" and append the current level number.
        levelText.text = "Day " + level;

        //Set levelImage to active blocking player's view of the game board during setup.
        levelImage.SetActive(true);

        //Call the HideLevelImage function with a delay in seconds of levelStartDelay.
        Invoke("HideLevelImage", levelStartDelay);

        //Clear any Enemy objects in our List to prepare for next level.
        enemies.Clear();
        //Call the SetupScene function of the BoardManager script, pass it current level number.
        boardScript.SetupScene(level);

    }

    /// <summary>
    /// initiate the hero team.
    /// </summary>
    private void InitiateHeroTeam()
    {
        heroList.Clear();
        enemyList.Clear();

        //读取英雄配置
        ReadHeroConfig();
    }

    private void ReadHeroConfig()
    {
        //获取存在的类型和等级
        heroDatas = UIManager.LoadHeroDataToFight();

        foreach(HeroData heroData in heroDatas)
        {
            instance.heroList.Add(heroData.type);
        }
        
    }

    //Hides black image used between levels
    void HideLevelImage()
    {
        //Disable the levelImage gameObject.
        levelImage.SetActive(false);

        //Set doingSetup to false allowing player to move again.
        doingSetup = false;
    }

    //Update is called every frame.
    void Update()
    {
        //Check that playersTurn or enemiesMoving or doingSetup are not currently true.
        if (playersTurn || enemiesMoving || doingSetup)

            //If any of these are true, return and do not start MoveEnemies.
            return;

        //Start moving enemies.
        StartCoroutine(MoveEnemies());

        if(enemies.Count == 0)
        {
            StopCoroutine(MoveEnemies() );
        }
    }

    //Coroutine to move enemies in sequence.
    IEnumerator MoveEnemies()
    {
        //While enemiesMoving is true player is unable to move.
        enemiesMoving = true;

        //Wait for turnDelay seconds, defaults to .1 (100 ms).
        yield return new WaitForSeconds(turnDelay);

        //If there are no enemies spawned (IE in first level):
        if (enemies.Count == 0)
        {
            //Wait for turnDelay seconds between moves, replaces delay caused by enemies moving when there are none.
            yield return new WaitForSeconds(turnDelay);
        }

        //Loop through List of Enemy objects.
        for (int i = 0; i < enemies.Count; i++)
        {
            //Call the MoveEnemy function of Enemy at index i in the enemies List.
            enemies[i].MoveEnemy();

            //Wait for Enemy's moveTime before moving next Enemy, 
            yield return new WaitForSeconds(enemies[i].moveTime);
        }
        //Once Enemies are done moving, set playersTurn to true so player can move.
        playersTurn = true;

        //Enemies are done moving, set enemiesMoving to false.
        enemiesMoving = false;
    }


    //GameOver is called when the player reaches 0 food points
    public void GameOver()
    {

        //Set levelText to display number of levels passed and game over message
        levelText.text = "After " + level + " days, you starved.";

        //Enable black background image gameObject.
        levelImage.SetActive(true);
        
        //Disable this GameManager.
        enabled = false;

        //Call the HideLevelImage function with a delay in seconds of levelStartDelay.
        Invoke("BackToHall", 2.0f);
    }

    /// <summary>
    /// return to the start hall
    /// </summary>
    public void BackToHall()
    {
        /*将状态设置false才能退出游戏*/
        /*#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
                Application.Quit();*/

        SceneManager.LoadScene("Hall");
    }

    //Call this to add the passed in Enemy to the List of Enemy objects.
    public void AddEnemyToList(Enemy script)
    {
        //Add Enemy to List enemies.
        enemies.Add(script);
    }
    /// <summary>
    /// enter battle in dungen
    /// </summary>
    /// <param name="enemy">the enemy which is fighting with</param>
    public void EnterBattle(Enemy enemy)
    {
        currentEnemy = enemy;
        RandomEnemyTeam();
        Debug.Log("Eneter battle.");
        BattleManager.instance.gameObject.SetActive(true);
        BattleManager.instance.GenerateEnemies();
    }
    /// <summary>
    /// exti battle in dungen
    /// </summary>
    public void ExitBattle()
    {
        Destroy(GameObject.Find("EnemyTeam").gameObject);
        BattleManager.instance.gameObject.SetActive(false);
        enemies.Remove(currentEnemy);
        Destroy(currentEnemy.gameObject);
        UIManager.Instance.AddCoins(10);
    }
    private void RandomEnemyTeam()
    {
        //clear enemy team
        BattleManager.instance.ClearEnemyQueue();
        enemyList.Clear();
        

        //random add enemy to enemy list.
        // there are up to 4 enemies and at least 1 enemy.
        float count = UnityEngine.Random.value;
        while(enemyList.Count == 0 || (count > 0.7f && enemyList.Count <= 4))
        {
            enemyList.Add(BattleManager.instance.RandomEnemyType());
            count = UnityEngine.Random.value;
        }
    }
}
