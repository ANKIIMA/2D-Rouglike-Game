using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Control the turn-based battle
public class BattleManager : MonoBehaviour
{
    //Singeleton
    public static BattleManager instance;

    //Reference to the enemies and hero scripts in the battle.
    private Queue<BattleEnemy> enemyScriptQueue;
    private Queue<BattleHero> heroScriptQueue;

    //Reference to the Camera.
    private Camera mainCamera;

    //Left and Right coordinates of the viewport in the world space.
    private float leftPoint;
    private float rightPoint;
    private float topPoint;
    private float bottomPoint;


    //Hero types.
    public enum heroTypes
    {
        knight
    }
    //Enemy types.
    public enum enemyTypes
    {
        FlyingEye,
        Goblin
    }

    public Transform BattleHandlerTrans;
    public GameObject DungenHandler;

    #region enable the serialization for dictionary.
    //Serialized method for dictionary.
    [System.Serializable]
    private struct prefabHeroConfig
    {
        public heroTypes type;
        public GameObject prefab;
    }
    [System.Serializable]
    private struct prefabEnemyConfig
    {
        public enemyTypes type;
        public GameObject prefab;
    }
    [SerializeField]
    private List<prefabEnemyConfig> enemyConfigs;
    [SerializeField]
    private List<prefabHeroConfig> heroConfigs;

    #endregion
    //Prefab of heros
    private Dictionary<heroTypes, GameObject> heros;
    //Prefab of enemies
    private Dictionary<enemyTypes, GameObject> enemies;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this) 
        {
            Destroy(this);
        }

        enemyScriptQueue = new Queue<BattleEnemy>();
        heroScriptQueue = new Queue<BattleHero>();
        enemyScriptQueue.Clear();
        heroScriptQueue.Clear();

        heros = new Dictionary<heroTypes, GameObject>();
        heros.Clear();
        enemies = new Dictionary<enemyTypes, GameObject>();
        enemies.Clear();

        //Dont destroy when change scenes.
        DontDestroyOnLoad(gameObject);

        DungenHandler = GameObject.Find("Dungen");
        BattleHandlerTrans = GameObject.Find("Battle").transform;

        mainCamera = Camera.main;
        instance.SetPoints();
        instance.AddDictionary();

        
        //set false and wait to be enable.
        BattleHandlerTrans.gameObject.SetActive(false);

        SceneManager.sceneLoaded += OnSceneWasLoaded;
    }

    private void OnEnable()
    {
        BattleHandlerTrans.gameObject.SetActive(true);

        DungenHandler.SetActive(false);

        //Instantiate units.
        instance.GenerateUnits();

        Debug.Log(BattleStateManager.instance.GetCurrentState().ToString());
    }

    private void Update()
    {
        
    }

    private void OnDisable()
    {
        DungenHandler.SetActive(true);

        BattleHandlerTrans.gameObject.SetActive(false);
    }



    /// <summary>
    /// Set the left, right, top, bottom point of the screen.
    /// </summary>
    private void SetPoints()
    {
        if(mainCamera != null) 
        {
            Vector3 leftBottom = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
            Vector3 rightTop = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));

            leftPoint = leftBottom.x;
            bottomPoint = leftBottom.y;
            rightPoint = rightTop.x;
            topPoint = rightTop.y;
        }
    }


    /// <summary>
    /// Add the List<struct> to the dictionary.
    /// </summary>
    private void AddDictionary()
    {
        foreach (var config in enemyConfigs)
        {
            enemies.Add(config.type, config.prefab);
        }

        foreach (var config in heroConfigs)
        {
            heros.Add(config.type, config.prefab);
        }
    }

    /// <summary>
    /// Generate the heros and enemies, enable the platfrom.
    /// </summary>
    private void GenerateUnits()
    {
        //Divide the whole platform into 8 units.
        float unitLength = (rightPoint - leftPoint) / 9;
        float heroPosX = unitLength + leftPoint;
        float enemyPosX = rightPoint - unitLength;
        float posY = (topPoint - bottomPoint) * 2 / 5;

        GameObject enemyHandler = new GameObject("EnemyTeam");
        GameObject heroHandler = new GameObject("HeroTeam");

        Vector3 heroPos = new Vector3(heroPosX, posY-0.27f, mainCamera.nearClipPlane);
        Vector3 enemyPos = new Vector3(enemyPosX, posY, mainCamera.nearClipPlane);

        //Enemy will be placed from back to front.
        //Generate enemies
        Debug.Log(GameManager.instance.enemyList.Count);
        foreach (var Type in GameManager.instance.enemyList)
        {

            Instantiate(enemies[Type], enemyPos, Quaternion.identity, enemyHandler.transform);
            enemyPos.x -= unitLength;

        }
        enemyHandler.transform.SetParent(BattleHandlerTrans);

        Debug.Log(GameManager.instance.heroList.Count);
        //Hero team will be placed from front to back. 
        foreach(var Type in GameManager.instance.heroList)
        {
            Instantiate(heros[Type], heroPos, Quaternion.identity, heroHandler.transform);  
            heroPos.x += unitLength;
            
        }
        heroHandler.transform.SetParent (BattleHandlerTrans);

    }

    /// <summary>
    /// Add team members
    /// </summary>
    /// <param name="member">member script</param>
    public void AddTeamMembers(BattleHero member)
    {
        //There are up to 3 members in the team.
        if(heroScriptQueue.Count > 3) 
        {
            Debug.Log("Reach the max team members.");
            return; 
        }
        else 
            heroScriptQueue.Enqueue(member);
    }

    /// <summary>
    /// Add enemies
    /// </summary>
    /// <param name="enemy">enemy script</param>
    public void AddEnemy(BattleEnemy enemy)
    {
        //There are up to 4 members in the enemy list.
        if(enemyScriptQueue.Count > 4)
        {
            Debug.Log("Reach the max enemy number.");
        }
        else
            enemyScriptQueue.Enqueue(enemy);
    }

    /// <summary>
    /// set the reference when the new scene was loaded.
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="loadSceneMode"></param>
    void OnSceneWasLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {

        instance.BattleHandlerTrans = GameObject.Find("BattleHandler").transform.Find("Battle");
        instance.DungenHandler = GameObject.Find("DungenHandler").transform.Find("Dungen").gameObject;

        BattleHandlerTrans.gameObject.SetActive(false);
        instance.gameObject.SetActive(false);
    }
}
