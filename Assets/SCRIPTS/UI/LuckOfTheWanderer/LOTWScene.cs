using System.Collections;
using System.Collections.Generic;
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
    #endregion
    #region NormalVariables
    private List<int> ListAllLOTW;
    private List<int> ListAllLOTWOwn;
    private List<int> ListLOTWChosen;
    private List<GameObject> ListCard;
    private bool ContainRed;
    private float RegenerateMovingTimer;
    private float StopRegenCardTimer;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        // Get All Cards Data
        ListAllLOTW = FindObjectOfType<AccessDatabase>().GetListIDAllLOTW(0);
        // Get Owned Cards Data

        // Random to get 3 cards
        ListLOTWChosen = new List<int>();
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
        if (Input.GetKeyDown(KeyCode.H))
        {
            RegenerateCard();
        }
    }
    #endregion
    #region LOTW Spawn
    private void GetRandom3Cards()
    {
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
            Debug.Log(ListTemp.Count);
            ListCard[i].GetComponent<LOTWCard>().OtherCards.Add(ListTemp[0]);
            ListCard[i].GetComponent<LOTWCard>().OtherCards.Add(ListTemp[1]);
        }
    }

    public void RegenerateCard()
    {
        foreach (var card in ListCard)
        {
            card.GetComponent<LOTWCard>().Regenerate();
        }
        StartCoroutine(RegenCardMove());
    }

    private IEnumerator RegenCardMove()
    {
        yield return new WaitForSeconds(1.5f);
        CardRegenerate.GetComponent<Rigidbody2D>().velocity = Position[0].transform.position - RegenCardPos.transform.position;
        StopRegenCardTimer = 1f;
        yield return new WaitForSeconds(1f);
        CardRegenerate.transform.position = RegenCardPos.transform.position;
        GetRandom3Cards();
    }
    #endregion
}
