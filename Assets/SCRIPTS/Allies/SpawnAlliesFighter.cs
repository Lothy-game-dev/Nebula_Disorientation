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
    public GameObject SpawnHole;
    public AudioClip SpawnSoundEffect;
    #endregion
    #region NormalVariables
    // will do delay later
    public List<GameObject> Allies;
    public float[] AllySpawnDelay;
    public Vector2[] AllySpawnPosition;
    public int[] AllySpawnID;
    public float AllyMaxHPScale;
    public float AllyBountyScale;
    private GameObject ChosenModel;
    public int EscortSpawnNumber;
    private int TotalNumberOfSpawnSound;
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
    public void SpawnAlly()
    {
        Allies = new List<GameObject>();
        for (int i = 0; i < AllySpawnID.Length; i++)
        {
            StartCoroutine(CreateAlly(AllySpawnID[i], AllySpawnPosition[i], i, Random.Range(0,2f)));
        }
        if (EscortSpawnNumber>0)
        {
            for (int i=0;i<EscortSpawnNumber;i++)
            {
                Vector2 SpawnPos = new Vector2(Random.Range(-4900, -3500), Random.Range(3500, 4900));
                GameObject SpawnEffect = Instantiate(SpawnHole, SpawnPos, Quaternion.identity);
                SpawnEffect.SetActive(true);
                Destroy(SpawnEffect, 1.5f);
                StartCoroutine(CreateAlly(1, SpawnPos, i,0));
            }
        }
    }
    private IEnumerator CreateAlly(int id, Vector2 spawnPos, int count, float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject SpawnEffect = Instantiate(SpawnHole, spawnPos, Quaternion.identity);
        SpawnEffect.SetActive(true);
        Destroy(SpawnEffect, 1.5f);
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
        if (id >= 13)
        {
            Ally.tag = "AlliesEliteFighter";
            Ally.transform.GetChild(6).gameObject.SetActive(false);
            Ally.transform.GetChild(7).gameObject.SetActive(true);
        }
        if (id==1)
        {
            Ally.transform.localScale *= 3;
        }
        AudioSource aus = Ally.AddComponent<AudioSource>();
        aus.clip = SpawnSoundEffect;
        aus.spatialBlend = 1;
        aus.rolloffMode = AudioRolloffMode.Linear;
        aus.maxDistance = 1000;
        aus.minDistance = 500;
        aus.priority = 256;
        aus.dopplerLevel = 0;
        aus.spread = 360;
        Destroy(aus, 4f);
        Ally.name = ChosenModel.name + " |" + spawnPos.x + " - " + spawnPos.y + " - " + count;
        Ally.GetComponent<SpriteRenderer>().sprite = ChosenModel.GetComponent<SpriteRenderer>().sprite;
        Ally.GetComponent<AlliesShared>().HPScale = AllyMaxHPScale;
        Ally.GetComponent<AlliesShared>().InitData(DataDict, ChosenModel);
    }
    #endregion
}
