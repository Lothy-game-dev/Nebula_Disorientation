using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LOTWScene : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject CardTemplate;
    public GameObject[] Position;
    public GameObject CardRegenerate;
    public GameObject RegenCardPos;
    public GameObject RedCardAlert;
    public GameObject PickButton;
    public GameObject RerollButton;
    public GameObject AllCardPopup;
    public GameObject AllCardButton;
    public GameObject CardOwnedPopup;
    public GameObject CardOwnedButton;
    public GameObject PopupBG;
    public GameObject LOTWNoText;
    public GameObject SessionShardText;
    #endregion
    #region NormalVariables
    private List<int> ListAllLOTW;
    private List<int> ListAllLOTWOwn;
    private List<int> ListLOTWChosen;
    private int[] ShardCostReroll;
    private List<GameObject> ListCard;
    private bool ContainRed;
    private float RegenerateMovingTimer;
    private float StopRegenCardTimer;
    private int RerollChance;
    private int chosenId;
    private int currentShard;
    private int SessionId;
    private GameObject chosenCard;
    private Dictionary<string, object> datas;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        // Get Stage Info
        SetStageInfoData();
        // Get All Cards Data
        ListAllLOTW = FindObjectOfType<AccessDatabase>().GetListIDAllLOTW(0);
        // Get Owned Cards Data
        bool check = FindObjectOfType<AccessDatabase>().CheckLOTWRepetable(PlayerPrefs.GetInt("PlayerID"));
        if (check)
        {
            if (ListAllLOTW.Contains(34))
            ListAllLOTW.Remove(34);
        }
        // Random to get 3 cards
        ListLOTWChosen = new List<int>();
        // disable button reroll
        RerollButton.GetComponent<SpriteRenderer>().color = new Color(100 / 255f, 100 / 255f, 100 / 255f);
        RerollButton.GetComponent<Collider2D>().enabled = false;
        GetRandom3Cards();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        StopRegenCardTimer -= Time.deltaTime;
        if (StopRegenCardTimer<=0f)
        {
            CardRegenerate.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        }
    }
    #endregion
    #region LOTW Spawn
    private void GetRandom3Cards()
    {
        // Disable button pick
        PickButton.GetComponent<SpriteRenderer>().color = new Color(100 / 255f, 100 / 255f, 100 / 255f);
        PickButton.GetComponent<Collider2D>().enabled = false;
        ListLOTWChosen = new List<int>();
        ContainRed = false;
        for (int i=0;i<3;i++)
        {
            int n = 0;
            float a = Random.Range(0, 100f);
            int tierChosen = a < 75 ? 3 : a < 97 ? 2 : 1;
            if (tierChosen == 1) ContainRed = true;
            do
            {
                List<int> ListChosen = FindObjectOfType<AccessDatabase>().GetListIDAllLOTW(tierChosen);
                int k = Random.Range(0, ListChosen.Count);
                n = ListChosen[k];
            }
            while (ListLOTWChosen.Contains(n));
            ListLOTWChosen.Add(n);
        }
        GenerateCards();
    }

    private void GenerateCards()
    {
        ListCard = new List<GameObject>();
        for (int i=0;i<3;i++)
        {
            GameObject Card = Instantiate(CardTemplate, Position[0].gameObject.transform.position, Quaternion.identity);
            Card.GetComponent<LOTWCard>().SetData(ListLOTWChosen[i]);
            Card.GetComponent<LOTWCard>().ToPos = Position[i];
            Card.GetComponent<LOTWCard>().CenterPos = Position[0];
            Card.GetComponent<Collider2D>().enabled = false;
            if (!ContainRed)
            {
                Card.GetComponent<LOTWCard>().NoRed = true;
            }
            Card.SetActive(true);
            ListCard.Add(Card);
        }
        for (int i=0;i<3;i++)
        {
            List<GameObject> ListTemp = new List<GameObject>();
            ListTemp.AddRange(ListCard);
            ListTemp.RemoveAt(i);
            ListCard[i].GetComponent<LOTWCard>().OtherCards.Add(ListTemp[0]);
            ListCard[i].GetComponent<LOTWCard>().OtherCards.Add(ListTemp[1]);
        }
        RedCardAlert.SetActive(ContainRed);
        StartCoroutine(EnableReroll());
    }

    private IEnumerator EnableReroll()
    {
        yield return new WaitForSeconds(1.5f);
        if (RerollChance > 0 && currentShard >= ShardCostReroll[3 - RerollChance])
        {
            // enable button reroll
            RerollButton.GetComponent<SpriteRenderer>().color = new Color(200 / 255f, 200 / 255f, 200 / 255f);
            RerollButton.GetComponent<Collider2D>().enabled = true;
        } else if (RerollChance <= 0) {
            RerollButton.GetComponent<SpriteRenderer>().color = new Color(100 / 255f, 100 / 255f, 100 / 255f);
            RerollButton.GetComponent<Collider2D>().enabled = true;
            RerollButton.GetComponent<LOTWButtons>().Noti = "Run out of reroll chances.";
        } else if (currentShard < ShardCostReroll[3 - RerollChance])
        {
            RerollButton.GetComponent<SpriteRenderer>().color = new Color(100 / 255f, 100 / 255f, 100 / 255f);
            RerollButton.GetComponent<Collider2D>().enabled = true; 
            RerollButton.GetComponent<LOTWButtons>().Noti = "Not enough shard.";
        }
    }
    public void RegenerateCard()
    {
        RerollButton.GetComponent<Collider2D>().enabled = false;
        // Update DB
        string check = FindObjectOfType<AccessDatabase>().UpdateSessionShard(SessionId, false, ShardCostReroll[3 - RerollChance]);
        if (check=="Success")
        {
            ResetShardInfo();
            RerollChance--;
            if (RerollChance == 0)
            {
                // disable button reroll
                RerollButton.GetComponent<SpriteRenderer>().color = new Color(100 / 255f, 100 / 255f, 100 / 255f);
                RerollButton.GetComponent<LOTWButtons>().Noti = "Run out of reroll chances.";
                RerollButton.transform.GetChild(1).GetComponent<TextMeshPro>().text = "";
                RerollButton.transform.GetChild(0).GetComponent<TextMeshPro>().text = "Reroll";
            }
            else
            {
                RerollButton.transform.GetChild(1).GetComponent<TextMeshPro>().text = "(" + RerollChance + " time" + (RerollChance > 1 ? "s" : "") + " left)";
                RerollButton.transform.GetChild(0).GetComponent<TextMeshPro>().text = "Reroll\n(" + ShardCostReroll[3 - RerollChance] + " <sprite index='0'>)";
            }

            RedCardAlert.SetActive(false);
            if (RerollChance > -1)
            {
                foreach (var card in ListCard)
                {
                    if (card != null)
                        card.GetComponent<LOTWCard>().Regenerate();
                }
                StartCoroutine(RegenCardMove());
            }
        } else if (check=="Fail")
        {
            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(transform.position,
                "Cannot add card to current session!", 3f);
        }
        
    }

    private IEnumerator RegenCardMove()
    {
        yield return new WaitForSeconds(1.5f);
        CardRegenerate.GetComponent<Rigidbody2D>().velocity = (Position[0].transform.position - RegenCardPos.transform.position)*2f;
        StopRegenCardTimer = 0.5f;
        yield return new WaitForSeconds(0.5f);
        CardRegenerate.transform.position = RegenCardPos.transform.position;
        GetRandom3Cards();
    }

    public void CardSelected(int CardId, GameObject go)
    {
        chosenId = CardId;
        chosenCard = go;
        // Enable button pick
        PickButton.GetComponent<SpriteRenderer>().color = new Color(200 / 255f, 200 / 255f, 200 / 255f);
        PickButton.GetComponent<Collider2D>().enabled = true;
    }

    public void ConfirmSelect()
    {
        chosenCard.GetComponent<LOTWCard>().PickCard();
        string check = FindObjectOfType<AccessDatabase>().AddCardToCurrentSession(PlayerPrefs.GetInt("PlayerID"), chosenId);
        if (check=="Fail")
        {
            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(transform.position,
                "Cannot add card to current session!", 3f);
        }
        FindObjectOfType<AccessDatabase>().AddSessionCurrentSaveData(PlayerPrefs.GetInt("PlayerID"), "Gameplay");
        // Disable button pick
        PickButton.GetComponent<SpriteRenderer>().color = new Color(100 / 255f, 100 / 255f, 100 / 255f);
        PickButton.GetComponent<Collider2D>().enabled = false;
        // disable button reroll
        RerollButton.GetComponent<SpriteRenderer>().color = new Color(100 / 255f, 100 / 255f, 100 / 255f);
        RerollButton.GetComponent<Collider2D>().enabled = false;
    }

    public void EnterGameplay()
    {
        FindObjectOfType<GameplayExteriorController>().GenerateBlackFadeClose(1f, 6f);
        StartCoroutine(WaitTeleport());
    }

    private IEnumerator WaitTeleport()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadSceneAsync("GameplayInterior");
        SceneManager.UnloadSceneAsync("GameplayExterior");
    }

    public void OpenAllCardsPopup()
    {
        PopupBG.SetActive(true);
        float InitScaleX = AllCardPopup.transform.localScale.x;
        AllCardPopup.transform.localScale = new Vector3(InitScaleX / 10f, InitScaleX / 10f, AllCardPopup.transform.localScale.z);
        AllCardPopup.SetActive(true);
        StartCoroutine(PopupAllCard(InitScaleX));
    }

    private IEnumerator PopupAllCard(float scale)
    {
        for (int i=0;i<45;i++)
        {
            AllCardPopup.transform.localScale = new Vector3(AllCardPopup.transform.localScale.x + scale / 50f, AllCardPopup.transform.localScale.x + scale / 50f, AllCardPopup.transform.localScale.z);
            yield return new WaitForSeconds(0.5f / 45);
        }
        AllCardPopup.GetComponent<LOTWAllCardPopup>().Open();
    }

    public void CloseAllCardsPopup()
    {
        AllCardPopup.GetComponent<LOTWAllCardPopup>().Close();
        PopupBG.SetActive(false);
    }

    public void OpenAllCardsOwnedPopup()
    {
        PopupBG.SetActive(true);
        float InitScaleX = CardOwnedPopup.transform.localScale.x;
        CardOwnedPopup.transform.localScale = new Vector3(InitScaleX / 10f, InitScaleX / 10f, CardOwnedPopup.transform.localScale.z);
        CardOwnedPopup.SetActive(true);
        StartCoroutine(PopupAllCardOwned(InitScaleX));
    }

    private IEnumerator PopupAllCardOwned(float scale)
    {
        for (int i = 0; i < 45; i++)
        {
            CardOwnedPopup.transform.localScale = new Vector3(CardOwnedPopup.transform.localScale.x + scale / 50f, CardOwnedPopup.transform.localScale.x + scale / 50f, CardOwnedPopup.transform.localScale.z);
            yield return new WaitForSeconds(0.5f / 45);
        }
        CardOwnedPopup.GetComponent<LOTWAllOwnedPopup>().Open();
    }

    public void CloseAllCardsOwnedPopup()
    {
        CardOwnedPopup.GetComponent<LOTWAllOwnedPopup>().Close();
        PopupBG.SetActive(false);
    }
    #endregion
    #region Set Data
    private void SetStageInfoData()
    {
        RerollChance = 3;
        ShardCostReroll = new int[] { 1, 2, 3 };
        datas = FindObjectOfType<AccessDatabase>().GetSessionInfoByPlayerId(PlayerPrefs.GetInt("PlayerID"));
        LOTWNoText.GetComponent<TextMeshPro>().text = "Luck of the wanderer\nNo. " + (int)datas["CurrentStage"];
        SessionShardText.transform.GetChild(1).GetComponent<TextMeshPro>().text = ((int)datas["SessionTimelessShard"]).ToString();
        currentShard = (int)datas["SessionTimelessShard"];
        SessionId = (int)datas["SessionID"];
        if (currentShard < ShardCostReroll[3-RerollChance])
        {
            RerollButton.GetComponent<SpriteRenderer>().color = new Color(100 / 255f, 100 / 255f, 100 / 255f);
            RerollButton.GetComponent<LOTWButtons>().Noti = "Not enough shard.";
        }
    }

    private void ResetShardInfo()
    {
        datas = FindObjectOfType<AccessDatabase>().GetSessionInfoByPlayerId(PlayerPrefs.GetInt("PlayerID"));
        SessionShardText.transform.GetChild(1).GetComponent<TextMeshPro>().text = ((int)datas["SessionTimelessShard"]).ToString();
        currentShard = (int)datas["SessionTimelessShard"];
    }
    #endregion
}
