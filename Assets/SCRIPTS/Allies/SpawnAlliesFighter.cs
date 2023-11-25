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
    public GameplayInteriorController controller;
    #endregion
    #region NormalVariables
    // will do delay later
    public List<GameObject> Allies;
    public float[] AllySpawnDelay;
    public Vector2[] AllySpawnPosition;
    public int[] AllySpawnID;
    public string[] AllyClass;
    public float AllyMaxHPScale;
    public float AllyBountyScale;
    private GameObject ChosenModel;
    public int EscortSpawnNumber;
    private int TotalNumberOfSpawnSound;
    public bool Escort;
    public string Priority;
    public bool Defend;
    public float DelayBetweenSpawn;
    public float RandomSpawnXUpper;
    public float RandomSpawnXLower;
    public float RandomSpawnYUpper;
    public float RandomSpawnYLower;
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
    #region Spawn Ally
    public void SpawnAlly()
    {
        Allies = new List<GameObject>();
        if (DelayBetweenSpawn>0)
        {
            StartCoroutine(SpawnAllyByTime());
        } else
        {
            for (int i = 0; i < AllySpawnID.Length; i++)
            {
                StartCoroutine(CreateAlly(AllySpawnID[i], AllySpawnPosition[i], i, Random.Range(0, 2f), AllyClass[i]));
            }
        }
        if (EscortSpawnNumber>0)
        {
            for (int i=0;i<EscortSpawnNumber;i++)
            {
                Vector2 SpawnPos = new Vector2(Random.Range(-4900, -3500), Random.Range(3500, 4900));
                GameObject SpawnEffect = Instantiate(SpawnHole, SpawnPos, Quaternion.identity);
                SpawnEffect.SetActive(true);
                Destroy(SpawnEffect, 1.5f);
                StartCoroutine(CreateAlly(1, SpawnPos, i,0, "A"));
            }
        }
    }

    private IEnumerator SpawnAllyByTime()
    {
        for (int i = 0; i < AllySpawnID.Length; i++)
        {
            StartCoroutine(CreateAlly(AllySpawnID[i], new Vector2(Random.Range(RandomSpawnXLower, RandomSpawnXUpper), Random.Range(RandomSpawnYLower, RandomSpawnYUpper)), i, Random.Range(0, 2f), AllyClass[i]));
            yield return new WaitForSeconds(DelayBetweenSpawn);
        }
        SpawnAlly();
    }

    private IEnumerator CreateAlly(int id, Vector2 spawnPos, int count, float delay, string Class)
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
        }
        if (id==1)
        {
            Ally.transform.localScale *= 3;
            Ally.transform.GetChild(9).gameObject.SetActive(true);
        } else
        {
            Ally.GetComponent<AlliesShared>().Priority = Priority;
            if (Class=="A")
            {
                Ally.transform.GetChild(10).gameObject.SetActive(true);
            } else if (Class == "B")
            {
                Ally.transform.GetChild(11).gameObject.SetActive(true);
            }
            else if (Class == "C")
            {
                Ally.transform.GetChild(12).gameObject.SetActive(true);
            }
        }
        AudioSource aus = Ally.AddComponent<AudioSource>();
        aus.playOnAwake = false;
        Ally.AddComponent<SoundController>();
        aus.clip = SpawnSoundEffect;
        aus.volume = controller.MasterVolumeScale / 100f * controller.SFXVolumeScale;
        aus.spatialBlend = 1;
        aus.rolloffMode = AudioRolloffMode.Linear;
        aus.maxDistance = 1000;
        aus.minDistance = 500;
        aus.priority = 256;
        aus.dopplerLevel = 0;
        aus.spread = 360;
        aus.Play();
        Destroy(aus, 4f);
        Ally.name = ChosenModel.name + " |" + spawnPos.x + " - " + spawnPos.y + " - " + count;
        Ally.GetComponent<SpriteRenderer>().sprite = ChosenModel.GetComponent<SpriteRenderer>().sprite;
        Ally.GetComponent<AlliesShared>().HPScale = AllyMaxHPScale;
        if (Escort)
        Ally.GetComponent<AlliesShared>().Escort = Escort;
        Ally.GetComponent<AlliesShared>().Class = Class;
        Ally.GetComponent<AlliesShared>().Defend = false;
        Ally.GetComponent<AlliesShared>().InitData(DataDict, ChosenModel);
    }
    #endregion
}
