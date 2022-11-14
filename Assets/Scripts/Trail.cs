using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trail : MonoBehaviour
{
    [SerializeField] private float lifeTimerMax;
    private float lifeTimer;
    private Material myMat;

    public void SetLifeTimerMax(float max) => lifeTimer = max;
    public float LifeTimerMax => lifeTimerMax;

    void Start()
    {
        lifeTimer = lifeTimerMax;
        myMat = GetComponent<Renderer>().material;
    }

    void FixedUpdate()
    {
        if (lifeTimer < 0) Destroy(gameObject);
        else {
            lifeTimer -= Time.deltaTime;
            myMat.color = new Color(myMat.color.r, myMat.color.g, myMat.color.b, lifeTimer / lifeTimerMax);
        }
    }
}
