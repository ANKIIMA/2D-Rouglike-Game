using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private int coins;

    public static UIManager Instance;

    public GameObject UpgradeWindow;

    public GameObject HeroSetWindow;
    public string directory = "Assets/Resources/HeroConfig/";
    private int heroCount = 0;

    private int currentHeroIndex = 0;

    [Serializable]
    public class HeroData
    {
        public int grade;
        public BattleManager.heroTypes type;
    }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(Instance);
        }
    }
    private void Start()
    {
        //heroSet
        RandomHeroSet();
        //��ʼ100��Ӳ��
        coins = 100;

        HeroSetWindow.transform.parent.Find("coins").gameObject.GetComponent<Text>().text = coins.ToString();
    }

    private void OnEnable()
    {
        ReadHeroConfig("MyHero");
        ReadUpgradeHeroConfig();
    }

    private void Update()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null)
        {
            return;
        }
        else if (mouse.leftButton.isPressed)
        {

            //ScreenPoint
            var onScreenPosition = mouse.position.ReadValue();
            //Ray(from camera to point in world space, vertical in 2D)
            var ray = Camera.main.ScreenPointToRay(onScreenPosition);
            //ray cast
            var hit = Physics2D.Raycast(new Vector2(ray.origin.x, ray.origin.y), Vector2.zero, Mathf.Infinity);

            if (hit.collider != null)
            {
                if (hit.collider.gameObject.name == "Upgrade")
                {
                    OpenUpgrade();
                }
                else if (hit.collider.gameObject.name == "HeroSet")
                {
                    OpenHeroSet();
                }
            }
        }
    }

    #region Upgrade

    private void OpenUpgrade()
    {
        UpgradeWindow.SetActive(true);
        UpgradeWindow.transform.Find("HeroInfo").gameObject.SetActive(false);
        ReadUpgradeHeroConfig();
    }

    public void CloseUpgrade()
    {
        UpgradeWindow.SetActive(false);
    }

    /// <summary>
    /// ����Ӣ��
    /// </summary>
    public void UpgradeHero()
    {
        DecreaseCoins(10);

        Transform info = UpgradeWindow.transform.Find("HeroInfo").transform;
        //��ȡ��Ϣ
        string[] Lines = File.ReadAllLines(directory + "heroConfig.txt");
        //ɾ��ԭ����Ϣ
        File.WriteAllText(directory + "heroConfig.txt", String.Empty);
        //��ʼд��
        StreamWriter sw = new StreamWriter(directory + "heroConfig.txt", true);
        for (int i = 0; i < Lines.Length; i++)
        {
            //�����Ŀ�����
            if (i == currentHeroIndex - 1)
            {
                //���·����л���д��
                HeroData obj = JsonUtility.FromJson<HeroData>(Lines[i]);
                obj.grade++;
                string json = JsonUtility.ToJson(obj);
                sw.WriteLine(json);
            }
            //����ֱ��д��
            else
            {
                sw.WriteLine(Lines[i]);
            }
        }
        sw.Close();
        //���´��������
        OpenHeroUpgradeInfo(currentHeroIndex);

    }

    /// <summary>
    /// �����������Ϣ
    /// </summary>
    public void OpenHeroUpgradeInfo(int num)
    {
        UpgradeWindow.transform.Find("HeroInfo").gameObject.SetActive(true);
        currentHeroIndex = num;

        Transform info = UpgradeWindow.transform.Find("HeroInfo").transform;
        string[] Lines = File.ReadAllLines(directory + "heroConfig.txt");

        for(int i = 0; i < Lines.Length; i++)
        {
            if(i == num - 1)
            {
                HeroData obj = JsonUtility.FromJson<HeroData>(Lines[i]);
                string typeName = obj.type.ToString();
                HeroBaseSO data = (HeroBaseSO)Resources.Load("SOAssets/" + typeName + "Data");
                if (data != null)
                {
                    info.Find("UnitName").GetComponent<Text>().text = data.m_UnitName;
                    info.Find("UnitType").GetComponent<Text>().text = data.m_type;
                    info.Find("Grade").GetComponent<Text>().text = "Grade: " + obj.grade.ToString();
                    info.Find("hp").GetComponent<Text>().text = "HP: " + (data.m_maxhp + data.m_levelhp * obj.grade).ToString();
                    info.Find("sp").GetComponent<Text>().text = "SP: " + (data.m_maxsp + data.m_levelsp * obj.grade).ToString();
                    info.Find("Attack").GetComponent<Text>().text = "Attack: " + (data.m_attackValue).ToString();
                    info.Find("Avatar").GetComponent<Image>().sprite = data.avatar;
                }
            }
        }
    }

    /// <summary>
    /// ��ȡӢ������
    /// </summary>
    public void ReadUpgradeHeroConfig( )
    {
        heroCount = 0;
        Transform myHero = UpgradeWindow.transform.Find("Hero").transform;
        string[] Lines = File.ReadAllLines(directory + "heroConfig.txt");

        for (int i = 0; i < myHero.childCount; i++)
        {
            //�ر�ÿ���Ӷ���
            Transform child = myHero.GetChild(i);
            child.gameObject.SetActive(false);
        }

        for (int i = 0, j = 0; i < myHero.childCount && j < Lines.Length; i++, j++)
        {
            //����ÿ���Ӷ���
            Transform child = myHero.GetChild(i);
            child.gameObject.SetActive(true);
            HeroData obj = JsonUtility.FromJson<HeroData>(Lines[j]);
            BattleManager.heroTypes type = obj.type;
            ReadHeroInfo(child, type);
            child.gameObject.SetActive(true);

            heroCount++;
        }
    }

    #endregion


    #region HeroSet
    private void OpenHeroSet()
    {
        HeroSetWindow.SetActive(true);
    }

    public void CloseHeroSet()
    {
        HeroSetWindow.SetActive(false);
    }

    /// <summary>
    /// ������ɿ���ļ��Ӣ��
    /// </summary>
    private void RandomHeroSet()
    {
        Transform heroSet = HeroSetWindow.transform.Find("Hero").transform;


        for (int i = 0; i < heroSet.childCount; i++)
        {

            Transform child = heroSet.GetChild(i);
            //�������һ��Ӣ������
            BattleManager.heroTypes type = BattleManager.RandomHeroType();
            ReadHeroInfo(child, type);
            child.gameObject.SetActive(true );
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="child">Ӣ������transfom���</param>
    /// <param name="type">Ӣ������</param>
    private void ReadHeroInfo(Transform child, BattleManager.heroTypes type)
    {
        string typeName = type.ToString();
        //��ȡ���Ͷ�Ӧ������
        UnitBaseSO data = (UnitBaseSO)Resources.Load("SOAssets/" + typeName + "Data");
        child.Find("Avatar").gameObject.GetComponent<Image>().sprite = data.avatar;
        child.Find("UnitType").gameObject.GetComponent<Text>().text = typeName;
        child.Find("UnitName").gameObject.GetComponent<Text>().text = data.m_UnitName;
    }

    /// <summary>
    /// ȡ���Ѿ���ļ��Ӣ��
    /// </summary>
    public void DeleteHero(GameObject button)
    {
        string[] Lines = File.ReadAllLines(directory + "heroConfig.txt");
        File.WriteAllText(directory + "heroConfig.txt", String.Empty);
        StreamWriter sw = new StreamWriter(directory + "heroConfig.txt", true);
        //����myhero���Ӷ�������д��
        Transform myHero = HeroSetWindow.transform.Find("MyHero").transform;
        for (int i = 0, j = 0; i < myHero.childCount && j < Lines.Length; i++, j++)
        {
            //����ÿ���Ӷ���
            Transform child = myHero.GetChild(i);
            //����Ҫɾ���Ķ���
            if(child.Find("Button").gameObject == button)
            {
                continue;
            }
            //��������д��
            sw.WriteLine(Lines[j]);
        }

        sw.Close();

        ReadHeroConfig("MyHero");
    }

    /// <summary>
    /// ��ļӢ��
    /// </summary>
    public void AddHero(GameObject button)
    {
        DecreaseCoins(30);

        if(heroCount >= 3)
        {
            Debug.Log("too much heros");
            return;
        }

        //�������л�����
        HeroData data = new HeroData();
        data.grade = 1;
        string arg = button.transform.parent.Find("UnitType").GetComponent<Text>().text;
        data.type = (BattleManager.heroTypes)Enum.Parse(typeof(BattleManager.heroTypes), arg);
        //���л�
        string json = JsonUtility.ToJson(data);
        StreamWriter sw = new StreamWriter(directory + "heroConfig.txt", true);
        sw.WriteLine(json);
        
        sw.Close();
        //�رո�Ӣ����
        button.transform.parent.gameObject.SetActive(false );
        ReadHeroConfig("MyHero");
    }

    /// <summary>
    /// ��ȡӢ������
    /// </summary>
    public void ReadHeroConfig(string name)
    {

        heroCount = 0;
        Transform myHero = HeroSetWindow.transform.Find(name).transform;
        string[] Lines = File.ReadAllLines(directory + "heroConfig.txt");

        for (int i = 0; i < myHero.childCount; i++)
        {
            //�ر�ÿ���Ӷ���
            Transform child = myHero.GetChild(i);
            child.gameObject.SetActive(false);
        }

        for (int i = 0, j = 0; i < myHero.childCount && j < Lines.Length; i++, j++)
        {
            //����ÿ���Ӷ���
            Transform child = myHero.GetChild(i);
            child.gameObject.SetActive(true);
            HeroData obj = JsonUtility.FromJson<HeroData>(Lines[j]);
            BattleManager.heroTypes type = obj.type;
            ReadHeroInfo(child, type);
            child.gameObject.SetActive(true);

            heroCount++;
        }
    }

    #endregion

    public void PlayDungen()
    {
        SceneManager.LoadScene("Dungen");
        if(GameManager.instance != null)
        {
            GameManager.instance.level = 0;
        }
    }

    public static List<HeroData> LoadHeroDataToFight()
    {
        string[] Lines = File.ReadAllLines("Assets/Resources/HeroConfig/" + "heroConfig.txt");
        List<HeroData> Datas = new List<HeroData>();
        for (int i = 0; i < Lines.Length; i++)
        {
            HeroData obj = JsonUtility.FromJson<HeroData>(Lines[i]);
            Datas.Add(obj);
        }

        return Datas;
    }

    public void AddCoins(int num)
    {
        coins += num;
    }

    private void DecreaseCoins(int num)
    {
        coins -= num;
        HeroSetWindow.transform.parent.Find("coins").gameObject.GetComponent<Text>().text = coins.ToString();
    }
}
