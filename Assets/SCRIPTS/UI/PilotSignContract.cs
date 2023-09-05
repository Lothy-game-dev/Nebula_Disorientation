using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PilotSignContract : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public TMP_InputField InputField;
    public GameObject SignButton;
    public GameObject[] Warning;
    public GameObject SignWord;
    #endregion
    #region NormalVariables
    private float InitScaleX;
    private bool alreadySign;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void OnEnable()
    {
        // Initialize variables
        if (name == "Sign")
        {
            alreadySign = false;
            InitScaleX = transform.localScale.x;
            transform.localScale = new Vector3(transform.localScale.x / 10f, transform.localScale.y, transform.localScale.z);
            GetComponent<BoxCollider2D>().enabled = false;
            StartCoroutine(ButtonAnimation());
        }
        else if (name == "SignInputField")
        {
            Color c = GetComponent<SpriteRenderer>().color;
            c.a = 0f;
            GetComponent<SpriteRenderer>().color = c;
            StartCoroutine(ButtonAnimation());
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (name=="SignInputField")
        {
            if (InputField.isFocused)
            {
                if (InputField.text.Length < 8f || InputField.text.Length > 16f)
                {
                    foreach (var warn in Warning)
                    {
                        if (!warn.activeSelf)
                        {
                            warn.SetActive(true);
                        }
                        Color c = warn.GetComponent<SpriteRenderer>().color;
                        c.g = (8 - InputField.text.Length) / 8;
                        c.b = (8 - InputField.text.Length) / 8;
                        warn.GetComponent<SpriteRenderer>().color = c;
                    }
                    if (SignButton.activeSelf) SignButton.SetActive(false);
                } 
                else
                {
                    foreach (var warn in Warning)
                    {
                        if (warn.activeSelf)
                        {
                            warn.SetActive(false);
                            Color c = warn.GetComponent<SpriteRenderer>().color;
                            c.g = 1;
                            c.b = 1;
                            warn.GetComponent<SpriteRenderer>().color = c;
                        }
                    }
                    if (!SignButton.activeSelf) SignButton.SetActive(true);
                }
                
            }
        }
        else if (name=="Sign" && !alreadySign)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                CheckAndSign();
            }
        }
    }
    #endregion
    #region Sign Contract
    private void OnMouseEnter()
    {
        alreadySign = false;
    }
    private void OnMouseDown()
    {
        if (name == "Sign" && !alreadySign)
        {
            CheckAndSign();
        }
    }

    private void CheckAndSign()
    {
        if (!alreadySign)
        {
            alreadySign = true;
            Debug.Log("Signed");
            // Sign 
        }
    }

    private IEnumerator ButtonAnimation()
    {
        if (name=="Sign")
        {
            for (int i = 0; i < 10; i++)
            {
                if (transform.localScale.x < InitScaleX)
                {
                    transform.localScale = new Vector3(transform.localScale.x + InitScaleX / 10f, transform.localScale.y, transform.localScale.z);
                    yield return new WaitForSeconds(0.1f);
                }
                else break;
            }
            SignWord.SetActive(true);
            GetComponent<BoxCollider2D>().enabled = true;
        } else if (name == "SignInputField")
        {
            for (int i = 0; i < 10; i++)
            {
                Color c = GetComponent<SpriteRenderer>().color;
                c.a += 0.1f;
                GetComponent<SpriteRenderer>().color = c;
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
    #endregion
}
