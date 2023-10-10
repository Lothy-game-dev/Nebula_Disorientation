using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceZoneStar : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject Explosion;
    public GameObject Range;
    #endregion
    #region NormalVariables
    public float currentHP;
    public float maxHP;
    private bool alreadyDestroy;
    private float RotateSpeed;
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
        transform.Rotate(new Vector3(0, 0, 2 + RotateSpeed));
        transform.GetChild(0).Rotate(new Vector3(0, 0, -2));
        if (currentHP <= 0f)
        {
            if (!alreadyDestroy)
            {
                alreadyDestroy = true;
                StartCoroutine(StarDestroy());
            }
        }
    }
    #endregion
    #region Initialize
    public void InitializeStar(Vector2 EndPos, float Speed)
    {
        Vector2 MovingVector = EndPos - new Vector2(transform.position.x, transform.position.y);
        GetComponent<Rigidbody2D>().velocity = MovingVector / MovingVector.magnitude * Speed;
        Destroy(gameObject, 25f);
        maxHP = 30000;
        currentHP = maxHP;
    }

    public IEnumerator StarDestroy()
    {
        GetComponent<Collider2D>().enabled = false;
        float Radius = (Range.transform.position - transform.position).magnitude;
        for (int i=0; i<10; i++)
        {
            RotateSpeed = i;
            GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity * (10-i)/10;
            Color c = GetComponent<SpriteRenderer>().color;
            c.r -= 1 / 10f;
            c.b -= 1 / 10f;
            c.g -= 1 / 10f;
            GetComponent<SpriteRenderer>().color = c;
            if (i == 5) Destroy(transform.GetChild(0).gameObject);
            for (int k=0; k< i/2 + 1; k++)
            {
                GameObject expl = Instantiate(Explosion, new Vector2(transform.position.x + Random.Range(-Radius, Radius), transform.position.y + Random.Range(-Radius, Radius)), Quaternion.identity);
                expl.SetActive(true);
                Destroy(expl, 0.3f);
            }
            yield return new WaitForSeconds(0.1f);
        }
        Destroy(gameObject);
    }
    #endregion
}
