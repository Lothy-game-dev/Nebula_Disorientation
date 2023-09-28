using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    #endregion
    #region NormalVariables
    private List<int> ListAllLOTW;
    private List<int> ListAllLOTWOwn;
    private List<int> ListLOTWChosen;
    private List<GameObject> ListCard;
    private bool ContainRed;
    private float RegenerateMovingTimer;
    private float StopRegenCardTimer;
    private int RerollChance;
    private int chosenId;
    private GameObject chosenCard;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        // Get All Cards Data
        ListAllLOTW = FindObjectOfType<AccessDatabase>().GetListIDAllLOTW(0);
        RerollChance = 3;
        // Get Owned Cards Data

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
        // enable button reroll
        RerollButton.GetComponent<SpriteRenderer>().color = new Color(200 / 255f, 200 / 255f, 200 / 255f);
        RerollButton.GetComponent<Collider2D>().enabled = true;
    }
    public void RegenerateCard()
    {
        RerollButton.GetComponent<Collider2D>().enabled = false;
        RerollChance--;
        if (RerollChance == 0)
        {
            // disable button reroll
            RerollButton.GetComponent<SpriteRenderer>().color = new Color(100 / 255f, 100 / 255f, 100 / 255f);
            RerollButton.GetComponent<Collider2D>().enabled = false;
            RerollButton.transform.GetChild(1).GetComponent<TextMeshPro>().text = "";
        } else
        RerollButton.transform.GetChild(1).GetComponent<TextMeshPro>().text = "(" + RerollChance + " time" + (RerollChance > 1 ? "s" : "") + " left)";
        RedCardAlert.SetActive(false);
        foreach (var card in ListCard)
        {
            if (card!=null)
            card.GetComponent<LOTWCard>().Regenerate();
        }
        StartCoroutine(RegenCardMove());

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
            Debug.Log("Fail");
        }
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
}
