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
    public Vector2[] EnemySpawnPosition;
    public int[] EnemySpawnID;
    private GameObject ChosenModel;
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
    }
    #endregion
    #region Spawn Enemy

    public void SpawnEnemy()
    {
        Enemies = new List<GameObject>();
        for (int i = 0; i < EnemySpawnID.Length; i++)
        {
            CreateEnemy(EnemySpawnID[i], EnemySpawnPosition[i], i);
        }
    }
    private void CreateEnemy(int id, Vector2 spawnPos, int count)
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
        if (id >= 13)
        {
            Enemy.tag = "EliteEnemy";
        }
        Enemy.name = ChosenModel.name + " |" + spawnPos.x + " - " + spawnPos.y + " - " + count;
        Enemy.GetComponent<SpriteRenderer>().sprite = ChosenModel.GetComponent<SpriteRenderer>().sprite;
        Enemy.GetComponent<EnemyShared>().InitData(DataDict, ChosenModel);
    }
    #endregion
}
