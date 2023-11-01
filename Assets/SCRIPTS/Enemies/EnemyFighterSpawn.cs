using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFighterSpawn : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject EnemyModel;
    public GameObject EnemyTemplate;
    public GameObject SpawnHole;
    public AudioClip SpawnSoundEffect;
    #endregion
    #region NormalVariables
    // will do delay later
    public List<GameObject> Enemies;
    public float SpawnDelay;
    public Vector2[] EnemySpawnPosition;
    public int[] EnemySpawnID;
    public int[] EnemyTier;
    public float EnemyMaxHPScale;
    public float EnemyBountyScale;
    private GameObject ChosenModel;
    public float DelayBetweenSpawn;
    public float DelaySpawnSBB;
    private float DelaySpawnSBBTimer;
    private bool IsSpawningSBB;
    public List<Vector2> SpawnSBBPos;
    private int SBBCount;
    public string Priority;
    public bool Escort;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (IsSpawningSBB)
        {
            DelaySpawnSBBTimer -= Time.deltaTime;
            if (DelaySpawnSBBTimer <=0f)
            {
                DelaySpawnSBBTimer = DelaySpawnSBB;
                Dictionary<string, object> SpawnPosData = FindObjectOfType<AccessDatabase>().GetSpawnPositionDataByType("ES");
                string[] VectorRangeTopLeft = ((string)SpawnPosData["PositionLimitTopLeft"]).Split("|");
                string[] VectorRangeBottomRight = ((string)SpawnPosData["PositionLimitBottomRight"]).Split("|");
                int k = Random.Range(0, VectorRangeTopLeft.Length);
                int LeftLimit = int.Parse(VectorRangeTopLeft[k].Replace("(", "").Replace(")", "").Split(",")[0]);
                int TopLimit = int.Parse(VectorRangeTopLeft[k].Replace("(", "").Replace(")", "").Split(",")[1]);
                int RightLimit = int.Parse(VectorRangeBottomRight[k].Replace("(", "").Replace(")", "").Split(",")[0]);
                int BottomLimit = int.Parse(VectorRangeBottomRight[k].Replace("(", "").Replace(")", "").Split(",")[1]);
                Vector2 SpawnPos = new Vector2(Random.Range(LeftLimit, RightLimit), Random.Range(BottomLimit, TopLimit));
                StartCoroutine(CreateEnemy(1, SpawnPos, SBBCount, 1, 0, true));
                SBBCount++;
            }
        }
    }
    #endregion
    #region Spawn Enemy

    public void SpawnEnemy()
    {
        Enemies = new List<GameObject>();
        if (DelayBetweenSpawn > 0f)
        {
            StartCoroutine(SpawnEnemyByTime());
        } else
        {
            for (int i = 0; i< EnemySpawnID.Length; i++)
            {
                StartCoroutine(CreateEnemy(EnemySpawnID[i], EnemySpawnPosition[i], i, EnemyTier[i], Random.Range(0,2f), false));
            }
        }
        if (DelaySpawnSBB > 0f)
        {
            IsSpawningSBB = true;
            SBBCount = 0;
        }
    }

    private IEnumerator SpawnEnemyByTime()
    {
        for (int i = 0; i < EnemySpawnID.Length; i++)
        {
            StartCoroutine(CreateEnemy(EnemySpawnID[i], EnemySpawnPosition[i], i, EnemyTier[i], 0, false));
            yield return new WaitForSeconds(DelayBetweenSpawn);
        }
    }

    private IEnumerator CreateEnemy(int id, Vector2 spawnPos, int count, int Tier, float delay, bool isBoom)
    {
        yield return new WaitForSeconds(delay);
        Dictionary<string, object> DataDict = FindObjectOfType<AccessDatabase>().GetDataEnemyById(id);
        // Get Model
        for (int i=0;i<EnemyModel.transform.childCount;i++)
        {
            if (EnemyModel.transform.GetChild(i).name.Replace(" ", "").ToLower().Equals(((string)DataDict["Name"]).Replace(" ","").ToLower()))
            {
                ChosenModel = EnemyModel.transform.GetChild(i).gameObject;
            }
        }
        GameObject Enemy = Instantiate(EnemyTemplate, new Vector3(spawnPos.x, spawnPos.y, EnemyTemplate.transform.position.z), Quaternion.identity);
        Enemies.Add(Enemy);
        if (id >= 13)
        {
            Enemy.tag = "EliteEnemy";
            Enemy.transform.GetChild(6).gameObject.SetActive(false);
            Enemy.transform.GetChild(7).gameObject.SetActive(true);
        } 
        if (id==1)
        {
            Enemy.transform.localScale *= 2;
            Enemy.transform.GetChild(6).gameObject.SetActive(false);
            Enemy.transform.GetChild(11).gameObject.SetActive(true);
        }
        Enemy.name = ChosenModel.name + " |" + spawnPos.x + " - " + spawnPos.y + " - " + count;
        GameObject SpawnEffect = Instantiate(SpawnHole, spawnPos, Quaternion.identity);
        SpawnEffect.SetActive(true);
        Destroy(SpawnEffect, 1.5f);
        AudioSource aus = Enemy.AddComponent<AudioSource>();
        aus.clip = SpawnSoundEffect;
        aus.spatialBlend = 1;
        aus.rolloffMode = AudioRolloffMode.Linear;
        aus.maxDistance = 1000;
        aus.minDistance = 500;
        aus.priority = 256;
        aus.dopplerLevel = 0;
        aus.spread = 360;
        Destroy(aus, 4f);
        if (isBoom)
        {
            Enemy.GetComponent<EnemyShared>().Priority = "WSSS";
        } else
        {
            if (Priority!=null && Priority != "")
            {
                Enemy.GetComponent<EnemyShared>().Priority = Priority;
            }
        }
        Enemy.GetComponent<SpriteRenderer>().sprite = ChosenModel.GetComponent<SpriteRenderer>().sprite;
        Enemy.GetComponent<EnemyShared>().HPScale = EnemyMaxHPScale;
        Enemy.GetComponent<EnemyShared>().CashBountyScale = EnemyBountyScale;
        Enemy.GetComponent<EnemyShared>().Tier = Tier;
        Enemy.GetComponent<EnemyShared>().Escort = Escort;
        Enemy.GetComponent<EnemyShared>().EnemyID = id;
        Enemy.GetComponent<EnemyShared>().InitData(DataDict, ChosenModel);
    }
    #endregion
}
