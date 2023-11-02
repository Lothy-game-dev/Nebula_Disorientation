using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UECSessionBackButton : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject FromScene;
    public GameObject ToScene;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
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
    #region OnMouseDown
    private void OnMouseDown()
    {
        FindObjectOfType<SoundSFXGeneratorController>().GenerateSound("ButtonClick");
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        FindObjectOfType<GameplayExteriorController>().GenerateBlackFadeClose(1f, 2f);
        yield return new WaitForSeconds(1.5f);
        FindObjectOfType<GameplayExteriorController>().GenerateBlackFadeOpen(ToScene.transform.position, 1f);
        FindObjectOfType<GameplayExteriorController>().ChangeToScene(ToScene);
        FromScene.SetActive(false);
        if (ToScene.GetComponent<UECSessionScene>()!=null)
        {
            ToScene.GetComponent<UECSessionScene>().ResetEconomyData();
        }
    }
    #endregion
}
