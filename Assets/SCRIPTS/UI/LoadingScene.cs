using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public Slider LoadingSlider;
    public float LoadingTime;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        LoadingSlider.maxValue = 100;
        StartCoroutine(LoadingAnimation(LoadingTime));
        // Initialize variables
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Loading animation
    // Group all function that serve the same algorithm
    private IEnumerator LoadingAnimation(float second)
    {
        for (int i = 0; i < 40; i++)
        {
            LoadingSlider.value += 100f/40;
            Debug.Log(LoadingSlider.value);
            yield return new WaitForSeconds(second/40);
        }
    }
    #endregion
    #region Generate loading scene
    public void GenerateLoadingScene(float second)
    {

    }
    // Group all function that serve the same algorithm
    #endregion
}
