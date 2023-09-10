using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvertItemBox : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject Items;
    public GameObject Outer;
    public GameObject CircleOuter;
    public GameObject CircleInner;
    public GameObject OtherBox;
    public ConvertBoard board;
    #endregion
    #region NormalVariables
    private GameObject CurrentItemObject;
    private float maxSpinSpeed;
    private float currentSpinSpeed;
    private bool maxSpeed;
    private bool StartSpinning;
    private float increaseSpeedTimer;
    private bool CircleAlreadyMove;
    private float CircleEqualScale;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        maxSpinSpeed = 20f;
        currentSpinSpeed = 0f;
        StartSpinning = false;
        increaseSpeedTimer = 0f;
        maxSpeed = false;
        CircleAlreadyMove = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (StartSpinning)
        {
            if (increaseSpeedTimer<=0f)
            {
                if (currentSpinSpeed < maxSpinSpeed - 1f)
                {
                    currentSpinSpeed += 2f;
                    Color c = CircleOuter.GetComponent<SpriteRenderer>().color;
                    c.a += 0.1f;
                    CircleOuter.GetComponent<SpriteRenderer>().color = c;
                } else
                {
                    currentSpinSpeed = maxSpinSpeed;
                    Color c = CircleOuter.GetComponent<SpriteRenderer>().color;
                    c.a = 1f;
                    CircleOuter.GetComponent<SpriteRenderer>().color = c;
                    maxSpeed = true;
                    StartSpinning = false;
                }
                increaseSpeedTimer = 0.2f;
            }
            else
            {
                increaseSpeedTimer -= Time.deltaTime;
            }
            SpinOuter();
        }
        if (maxSpeed)
        {
            maxSpeed = false;
            Outer.gameObject.SetActive(false);
            StartCoroutine(MinimizeCircleOuter());
        }
        if (CircleAlreadyMove)
        {
            if (CircleInner.transform.position.x >= OtherBox.transform.position.x)
            {
                CircleAlreadyMove = false;
                CircleInner.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                CircleInner.transform.position = OtherBox.transform.position;
                StartCoroutine(InnerCircleGrow());
            }
        }
    }
    #endregion
    #region Show Item
    public void ShowItem(string item)
    {
        for (int i=0; i<Items.transform.childCount;i++)
        {
            if (Items.transform.GetChild(i).name.ToLower().Equals(item.Replace(" ","").ToLower()))
            {
                Items.transform.GetChild(i).gameObject.SetActive(true);
                CurrentItemObject = Items.transform.GetChild(i).gameObject;
                Color c = CurrentItemObject.GetComponent<SpriteRenderer>().color;
                c.a = 1;
                CurrentItemObject.GetComponent<SpriteRenderer>().color = c;
                break;
            }
        }
    }

    public void StartAnimation()
    {
        increaseSpeedTimer = 0;
        if (!Outer.activeSelf)
        {
            Outer.SetActive(true);
        }
        if (!CircleOuter.activeSelf)
        {
            CircleOuter.SetActive(true);
        }
        if (!CircleInner.activeSelf)
        {
            CircleInner.SetActive(true);
        }
        StartSpinning = true;
    }

    private IEnumerator MinimizeCircleOuter()
    {
        CircleEqualScale = CircleOuter.transform.localScale.x;
        int n = Mathf.RoundToInt((CircleOuter.transform.localScale.x - CircleInner.transform.localScale.x)/0.01f);
        for (int i = 0; i < n; i++)
        {
            CircleOuter.transform.localScale = new Vector3(
                CircleOuter.transform.localScale.x - 0.01f,
                CircleOuter.transform.localScale.y - 0.01f,
                CircleOuter.transform.localScale.z);
            Color c2 = CurrentItemObject.GetComponent<SpriteRenderer>().color;
            c2.a -= 1f/n;
            CurrentItemObject.GetComponent<SpriteRenderer>().color = c2;
            yield return new WaitForSeconds(0.02f);
        }
        for (int i=0;i<10;i++)
        {
            Color c = CircleInner.GetComponent<SpriteRenderer>().color;
            c.a += 0.1f;
            CircleInner.GetComponent<SpriteRenderer>().color = c;
            yield return new WaitForSeconds(0.002f);
        }
        CircleOuter.SetActive(false);
        //Move
        CircleAlreadyMove = true;
        CircleInner.GetComponent<Rigidbody2D>().velocity = (OtherBox.transform.position - CircleInner.transform.position) * 2f;
    }
    private void SpinOuter()
    {
        Outer.transform.Rotate(new Vector3(0, 0, currentSpinSpeed));
    }

    private IEnumerator InnerCircleGrow()
    {
        int n = Mathf.RoundToInt((CircleEqualScale - CircleInner.transform.localScale.x) / 0.01f);
        for (int i=0;i<n;i++)
        {
            CircleInner.transform.localScale = new Vector3(
                CircleInner.transform.localScale.x + 0.01f,
                CircleInner.transform.localScale.y + 0.01f,
                CircleInner.transform.localScale.z);
            Color c2 = CircleInner.GetComponent<SpriteRenderer>().color;
            c2.a -= 0.75f / n;
            CircleInner.GetComponent<SpriteRenderer>().color = c2;
            yield return new WaitForSeconds(0.02f);
        }
        Destroy(CircleInner,0.5f);
        // Complete Anim
        board.ConvertDone();
    }
    #endregion
}
