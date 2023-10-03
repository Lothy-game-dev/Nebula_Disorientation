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
    #endregion
    #region NormalVariables
    // will do delay later
    public List<GameObject> Enemies;
    private Dictionary<int, float> EnemySpawnDelay;
    private Vector2[] EnemySpawnPosition;
    private int[] EnemySpawnID;
    private GameObject ChosenModel;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        EnemySpawnID = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15};
        EnemySpawnPosition = new Vector2[] { 
            new Vector2(100+500, 100), new Vector2(-100+500, 100), new Vector2(100+500, -100),
        new Vector2(-100+500, -100), new Vector2(200+500, 200), new Vector2(200+500, -200),
        new Vector2(-200+500, 200), new Vector2(-200+500, -200), new Vector2(300+500, 300),
        new Vector2(300+500, -300), new Vector2(-300+500, 300), new Vector2(-300+500, -300),
        new Vector2(400+500, 400), new Vector2(-400+500, 400), new Vector2(400+500, -400) };
        EnemySpawnDelay = new Dictionary<int, float>();
        EnemySpawnDelay.Add(1, 5f);
        EnemySpawnDelay.Add(2, 10f);
        EnemySpawnDelay.Add(3, 5f);
        EnemySpawnDelay.Add(4, 10f);
        Enemies = new List<GameObject>();
        for (int i=0;i<EnemySpawnID.Length;i++)
        {
            CreateEnemy(EnemySpawnID[i], EnemySpawnPosition[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Spawn Enemy
    private void CreateEnemy(int id, Vector2 spawnPos)
    {
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
        Enemy.name = ChosenModel.name + " |" + spawnPos.x + " - " + spawnPos.y;
        Enemy.GetComponent<SpriteRenderer>().sprite = ChosenModel.GetComponent<SpriteRenderer>().sprite;
        Enemy.GetComponent<EnemyShared>().InitData(DataDict, ChosenModel);
    }

    public IEnumerator SpawnEnemy()
    {
        yield return null;
    }
    #endregion
}
