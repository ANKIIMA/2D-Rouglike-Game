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

    //Index of the current unit
    public int unitIndex = 0;

    //Reference to the Camera.
    private Camera mainCamera;

    //point set
    private float unitLength;

    private Vector3 heroPos;
    private Vector3 enemyPos;


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
    private Dictionary<heroTypes, GameObject> herosConfig;
    //Prefab of enemies
    private Dictionary<enemyTypes, GameObject> enemiesConfig;

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
        herosConfig = new Dictionary<heroTypes, GameObject>();
        enemiesConfig = new Dictionary<enemyTypes, GameObject>();

        ReloadBattleManager();

        //Dont destroy when change scenes.
        DontDestroyOnLoad(gameObject);
        instance.SetPoints();
        instance.AddDictionary();

        SceneManager.sceneLoaded += OnSceneWasLoaded;
    }

    private void OnEnable()
    {
        BattleHandlerTrans.gameObject.SetActive(true);

        DungenHandler.SetActive(false);

        unitIndex = 0;

        
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

            float leftPoint = leftBottom.x;
            float bottomPoint = leftBottom.y;
            float rightPoint = rightTop.x;
            float topPoint = rightTop.y;

            //Divide the whole platform into 8 units.
            float unitLength = (rightPoint - leftPoint) / 9;
            float heroPosX = unitLength + leftPoint;
            float enemyPosX = rightPoint - unitLength;
            float posY = (topPoint - bottomPoint) * 2 / 5;

            heroPos = new Vector3(heroPosX, posY - 0.27f, mainCamera.nearClipPlane);
            enemyPos = new Vector3(enemyPosX, posY, mainCamera.nearClipPlane);
        }
    }


    /// <summary>
    /// Add the List<struct> to the dictionary.
    /// </summary>
    private void AddDictionary()
    {
        foreach (var config in enemyConfigs)
        {
            enemiesConfig.Add(config.type, config.prefab);
        }

        foreach (var config in heroConfigs)
        {
            herosConfig.Add(config.type, config.prefab);
        }
    }

    /// <summary>
    /// Generate the heros and enemies, enable the platfrom.
    /// </summary>
    private void GenerateUnits()
    {
        GameObject enemyHandler = new GameObject("EnemyTeam");
        GameObject heroHandler = new GameObject("HeroTeam");
        //Enemy will be placed from back to front.
        //Generate enemies
        foreach (var Type in GameManager.instance.enemyList)
        {
            Instantiate(enemiesConfig[Type], enemyPos, Quaternion.identity, enemyHandler.transform);
            enemyPos.x -= unitLength;

        }
        enemyHandler.transform.SetParent(BattleHandlerTrans);

        //Hero team will be placed from front to back. 
        foreach(var Type in GameManager.instance.heroList)
        {
            Instantiate(herosConfig[Type], heroPos, Quaternion.identity, heroHandler.transform);  
            heroPos.x += unitLength;
            
        }
        heroHandler.transform.SetParent (BattleHandlerTrans);

    }

    /// <summary>
    /// Register team members
    /// </summary>
    /// <param name="member">member script</param>
    public void RegisterHero(BattleHero member)
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
    /// Register enemies
    /// </summary>
    /// <param name="enemy">enemy script</param>
    public void RegisterEnemy(BattleEnemy enemy)
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
        ReloadBattleManager();
        instance.GenerateUnits();
    }

    /// <summary>
    /// invoke when every scene was loaded.
    /// </summary>
    private void ReloadBattleManager()
    {
        instance.BattleHandlerTrans = GameObject.Find("BattleHandler").transform.Find("Battle");
        instance.DungenHandler = GameObject.Find("DungenHandler").transform.Find("Dungen").gameObject;

        unitIndex = 0;

        mainCamera = Camera.main;

        enemyScriptQueue.Clear();
        heroScriptQueue.Clear();

        BattleHandlerTrans.gameObject.SetActive(false);
        instance.gameObject.SetActive(false);
    }

    /// <summary>
    /// return the hero who should be acting from the queue.
    /// </summary>
    /// <returns></returns>
    public BattleHero GetActionHero()
    {
        if (heroScriptQueue.Count == 0) return null;
        BattleHero currentHero = heroScriptQueue.Dequeue();
        heroScriptQueue.Enqueue(currentHero);

        return currentHero;
    }

    /// <summary>
    /// return the enemy who should be acting from the queue.
    /// </summary>
    /// <returns></returns>
    public BattleEnemy GetActionEnemy()
    {
        if (GetEnemyCount() == 0) return null;
        BattleEnemy currentEnemy = enemyScriptQueue.Dequeue();
        enemyScriptQueue.Enqueue(currentEnemy);

        return currentEnemy;
    }

    /// <summary>
    /// return the number of heros.
    /// </summary>
    /// <returns></returns>
    public int GetHeroCount()
    {
        return heroScriptQueue.Count;
    }

    /// <summary>
    /// return the number of enemies.
    /// </summary>
    /// <returns></returns>
    public int GetEnemyCount()
    {
        return enemyScriptQueue.Count;
    }

    public void AddUnitIndex()
    {
        unitIndex++;
    }

    public void ClearUnitIndex()
    {
        unitIndex = 0;
    }

    /// <summary>
    /// Delete obj in hero queue or enemy queue.
    /// </summary>
    /// <param name="obj"></param>
    public void DeleteObjectInBattleQueue(UnityEngine.Object obj)
    {
        if(obj is  BattleHero)
        {
            Queue<BattleHero> tmp = new Queue<BattleHero> ();
            foreach(var hero in heroScriptQueue)
            {
                if(obj != hero)
                {
                    tmp.Enqueue(hero);
                }
            }

            heroScriptQueue = tmp;
            tmp = null;
        }
        else if(obj is BattleEnemy)
        {
            Queue<BattleEnemy> tmp = new Queue<BattleEnemy> ();
            foreach(var enemy in enemyScriptQueue)
            {
                if(obj !=  enemy)
                {
                    tmp.Enqueue(enemy);
                }
            }

            enemyScriptQueue = tmp;
            tmp = null;
        }

    }
}
