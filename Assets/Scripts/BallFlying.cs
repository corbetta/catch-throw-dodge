using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallFlying : MonoBehaviour
{
    [SerializeField] private GameObject ballTrailPrefab, targetPrefab, ballFlyingPrefab, target;
    [SerializeField] private float trailTimerMax, trailTimer, gravity, gravityMax, targetTimerMax, targetTimer, targetLerpSpeed, targetPrecision;
    [SerializeField] private Vector3 velocity;

    private GameObject ballTrail;
    private Material myMat;
    private Rigidbody rb;
    private Vector3 targetNextPos, landingSpot;
    private MyColor myColor;

    public MyColor GetMyColor => myColor;

    void Start()
    {
        StartingLocationAndVelocity();
        trailTimer = trailTimerMax;
        rb = GetComponent<Rigidbody>();
        myMat = GetComponent<Renderer>().material;

        //set ball color
        float randomColor = Random.Range(0, 3);
        if (randomColor < 1) {
            myColor = MyColor.Blue;
            myMat.color = Constants.blue;
        }
        else if (randomColor < 2) {
            myColor = MyColor.Pink;
            myMat.color = Constants.pink;
        }
        else {
            myColor = MyColor.Yellow;
            myMat.color = Constants.yellow;
        }

        //create target and place it in the correct position
        target = Instantiate(targetPrefab);
        Vector2 amountToAdd = new Vector2(Random.Range(-4, 4), Random.Range(-4, 4));
        if (amountToAdd.magnitude > 4) amountToAdd = amountToAdd.normalized * 4;
        targetNextPos = landingSpot + new Vector3(amountToAdd.x, 0, amountToAdd.y);
        targetNextPos.y = 0;
        target.transform.position = targetNextPos;
        //set the pieces of the target to the color of this ball
        foreach (Renderer r in target.GetComponentsInChildren<Renderer>()) {
            r.material.color = myMat.color;
        }
    }

    void Update()
    {
        if (GameManager.Instance.GameOver) DestroyThisBall(false); //if the game is over, destroy all balls

        if (trailTimer <= 0) {
            ballTrail = Instantiate(ballTrailPrefab, transform.position, Quaternion.identity);
            ballTrail.GetComponent<Renderer>().material.color = myMat.color;
        }
        else trailTimer -= Time.deltaTime;

        if (transform.position.y < -1) {
            DestroyThisBall(true);
        }
    }

    void FixedUpdate() {
        if (velocity.y < -gravityMax) velocity.y = -gravityMax;
        else {
            velocity.y -= gravity * Time.deltaTime;
        }
        rb.MovePosition(transform.position + velocity * Time.deltaTime);
    }

    /// <summary>
    /// Set a random starting location and velocity along the edge of the map.
    /// </summary>
    void StartingLocationAndVelocity() {
        int startingFace = Random.Range(0, 4);
        switch (startingFace) {
            case (0):  //start on the top
                transform.position = new Vector3(Random.Range(-24, 24), 1, 25);
                if (transform.position.x < -10) velocity.x = Random.Range(0, 10);
                else if (transform.position.x < 10) velocity.x = Random.Range(-10, 10);
                else velocity.x = Random.Range(-10, 0);
                velocity.z = Random.Range(-20, -3);
                velocity.y = Random.Range(10, 20);
                break;
            case (1): //start on the bottom
                transform.position = new Vector3(Random.Range(-24, 24), 1, -25);
                if (transform.position.x < -10) velocity.x = Random.Range(0, 10);
                else if (transform.position.x < 10) velocity.x = Random.Range(-10, 10);
                else velocity.x = Random.Range(-10, 0);
                velocity.z = Random.Range(3, 20);
                velocity.y = Random.Range(10, 20);
                break;
            case (2): //start on the right
                transform.position = new Vector3(25, 1, Random.Range(-24, 24));
                if (transform.position.z < -10) velocity.z = Random.Range(0, 10);
                else if (transform.position.z < 10) velocity.z = Random.Range(-10, 10);
                else velocity.z = Random.Range(-10, 0);
                velocity.x = Random.Range(-20, -3);
                velocity.y = Random.Range(10, 20);
                break;
            case (3): //start on the left
                transform.position = new Vector3(-25, 1, Random.Range(-24, 24));
                if (transform.position.z < -10) velocity.z = Random.Range(0, 10);
                else if (transform.position.z < 10) velocity.z = Random.Range(-10, 10);
                else velocity.z = Random.Range(-10, 0);
                velocity.x = Random.Range(3, 20);
                velocity.y = Random.Range(10, 20);
                break;
        }
        //make sure that this ball would land inside the walls
        landingSpot = WhereWillThisBallLand(transform.position, velocity, targetPrecision);
        if (landingSpot.x > 24 || landingSpot.x < -24 || landingSpot.z > 24 || landingSpot.z < -24) {
            StartingLocationAndVelocity();
        }
    }

    /// <summary>
    /// Find out where a ball with a specific position and velocity will land
    /// </summary>
    Vector3 WhereWillThisBallLand(Vector3 position, Vector3 velocity, float precision) {
        while (position.y > 0) {
            if (velocity.y < -gravityMax) velocity.y = -gravityMax;
            else velocity.y -= gravity * precision;
            position += velocity * precision;
        }
        return position;
    }

    /// <summary>
    /// Destroy this ball and its target. If makeNewBall is true, create a new ballFlying before destroying this one.
    /// </summary>
    public void DestroyThisBall(bool makeNewBall) {
        if(makeNewBall) Instantiate(ballFlyingPrefab);
        Destroy(target);
        Destroy(gameObject);
    }

}
