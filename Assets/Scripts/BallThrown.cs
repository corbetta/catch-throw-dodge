using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallThrown : MonoBehaviour
{
    public float speed, trailTimer, trailTimerMax;
    public MyColor myColor;
    public GameObject ballTrailPrefab;
    Rigidbody rb;
    Material myMat;
    GameObject ballTrail;

    void Start()
    {
        trailTimer = trailTimerMax;
        rb = GetComponent<Rigidbody>();
        myMat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        if (GameManager.Instance.GameOver) Destroy(gameObject); //if the game is over, destroy all balls

        if (trailTimer <= 0) {
            ballTrail = Instantiate(ballTrailPrefab, transform.position, Quaternion.identity);
            ballTrail.GetComponent<Renderer>().material.color = myMat.color;
            ballTrail.GetComponent<Trail>().SetLifeTimerMax(0.1f);
          //  trailTimer = trailTimerMax;
        }
        else trailTimer -= Time.deltaTime;

        if (myColor == MyColor.Blue) {
            myMat.color = Constants.blue;
        }
        else if (myColor == MyColor.Pink) {
            myMat.color = Constants.pink;
        }
        else if (myColor == MyColor.Yellow) {
            myMat.color = Constants.yellow;
        }
    }

    private void FixedUpdate() {
        rb.MovePosition(transform.position + (transform.forward * speed * Time.deltaTime));
    }

    public void ColorBall(MyColor newColor) {
        myColor = newColor;
    }
}
