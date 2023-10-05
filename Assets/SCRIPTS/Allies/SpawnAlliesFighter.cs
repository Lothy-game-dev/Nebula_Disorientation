using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAlliesFighter : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject AllyModel;
    public GameObject AllyTemplate;
    #endregion
    #region NormalVariables
    // will do delay later
    public List<GameObject> Allies;
    private Dictionary<int, float> AllySpawnDelay;
    private Vector2[] AllySpawnPosition;
    private int[] AllySpawnID;
    private GameObject ChosenModel;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        AllySpawnID = new int[20];
        for (int i=0;i<20;i++)
        {
            AllySpawnID[i] = Random.Range(2, 4);
        }
        AllySpawnPosition = new Vector2[20];
        for (int i = 0; i < 20; i++)
        {
            AllySpawnPosition[i].x = Random.Range(-2100f, -700f);
            AllySpawnPosition[i].y = Random.Range(-3500f, 3500f);
        }
        AllySpawnDelay = new Dictionary<int, float>();
        AllySpawnDelay.Add(1, 5f);
        AllySpawnDelay.Add(2, 10f);
        AllySpawnDelay.Add(3, 5f);
        AllySpawnDelay.Add(4, 10f);
        Allies = new List<GameObject>();
        for (int i = 0; i < AllySpawnID.Length; i++)
        {
            CreateEnemy(AllySpawnID[i], AllySpawnPosition[i]);
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
        Dictionary<string, object> DataDict = FindObjectOfType<AccessDatabase>().GetDataAlliesById(id);
        // Get Model
        for (int i = 0; i < AllyModel.transform.childCount; i++)
        {
            if (AllyModel.transform.GetChild(i).name.Replace(" ", "").ToLower().Equals(((string)DataDict["Name"]).Replace(" ", "").ToLower()))
            {
                ChosenModel = AllyModel.transform.GetChild(i).gameObject;
            }
        }
        GameObject Ally = Instantiate(AllyTemplate, new Vector3(spawnPos.x, spawnPos.y, AllyTemplate.transform.position.z), Quaternion.identity);
        Allies.Add(Ally);
        Ally.name = ChosenModel.name + " |" + spawnPos.x + " - " + spawnPos.y;
        Ally.GetComponent<SpriteRenderer>().sprite = ChosenModel.GetComponent<SpriteRenderer>().sprite;
        Ally.GetComponent<AlliesShared>().InitData(DataDict, ChosenModel);
    }

    public IEnumerator SpawnAlly()
    {
        yield return null;
    }
    #endregion
}
