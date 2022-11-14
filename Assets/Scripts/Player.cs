using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed, dashMultiplier, dashTimerMax, dashTimer, dashTrailTimerMax, dashTrailTimer, 
        diveMultiplier, diveDescel, diveTimerMax, diveTimer, diveTrailTimerMax, diveTrailTimer, diveVerticalMovementTimer, diveRotationTimer, diveRotationTimerMax;
    [SerializeField] private GameObject dashTrailPrefab, ballThrownPrefab, ballFlyingPrefab;
    
    private MyColor myColor;
    private int numberOfBalls;
    private Rigidbody rb;
    private Vector3 movement;
    private GameObject dashTrail, ballThrown, marker, arrow;
    private bool dashing, diving;
    private Material myMat;
    private float horizontal, vertical;
    private Transform visualChild;
    private BoxCollider boxCollider;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        myColor = MyColor.Blank;
        visualChild = transform.Find("Visual");
        myMat = visualChild.gameObject.GetComponent<Renderer>().material;
        marker = transform.Find("Marker").gameObject;
        arrow = marker.transform.Find("Arrow").gameObject;
        marker.SetActive(false);
    }

    void Update() {
        //ROTATE PLAYER LOGIC
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        if (Mathf.Abs(horizontal) + Mathf.Abs(vertical) > .3f) {
            float angle = 90 - (Mathf.Atan2(vertical, horizontal) * Mathf.Rad2Deg);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, angle, transform.eulerAngles.z);
        }
        //DASH START LOGIC
        if (Input.GetButtonDown("FaceButtonLeft") && dashTimer <= 0 && movement.magnitude > .04f && dashing == false && diveTimer <= 0 && diving == false) {
            dashTimer = dashTimerMax;
            movement *= dashMultiplier;
            dashing = true;
        }
        //DIVE START LOGIC
        if (Input.GetButtonDown("FaceButtonTop") && diveTimer <= 0 && movement.magnitude > .04f && diving == false) {
            diveTimer = diveTimerMax;
            diveRotationTimer = 0;
            diveVerticalMovementTimer = 0;
            if (dashing) movement /= dashMultiplier; //if I'm dashing when I start the dive, reset the movement to a non-dashing speed before we do anything else
            movement *= diveMultiplier;
            diving = true;
            //stop dash
            dashTimer = 0;
            dashing = false;
        }
        //THROW START LOGIC - Create a ballThrown facing the direction the player is facing. Give it a color. Then get the next ballFlying going.
        if (Input.GetButtonDown("FaceButtonBottom") && numberOfBalls > 0) {
            numberOfBalls--;
            ballThrown = Instantiate(ballThrownPrefab, new Vector3(transform.position.x, .5f, transform.position.z), transform.rotation);
            ballThrown.GetComponent<BallThrown>().ColorBall(myColor);
            Instantiate(ballFlyingPrefab);
        }
        if (numberOfBalls > 0) marker.SetActive(true);
        else {
            marker.SetActive(false);
            myColor = MyColor.Blank;
            myMat.color = Constants.blank;
        }
        //DEBUG - latitude longitude test
        if (Input.GetButtonDown("FaceButtonRight")) {
            Vector2 longitudeAndLatitude = Utilities.GameLocationToLongitudeLatitude(transform.position.x, transform.position.z);
            WeatherInfo weatherInfo = Utilities.GetWeatherInfo(longitudeAndLatitude.x, longitudeAndLatitude.y);
            Weather weather = Utilities.GetWeatherFromWeatherInfo(weatherInfo);
            WeatherEffects weatherEffects = GameObject.FindObjectOfType<WeatherEffects>();
            weatherEffects.SetWeather(weather);
        }
    }

        void FixedUpdate()
    {
        //DIVING LOGIC
        if (diveTimer > 0) {
            diveTimer -= Time.fixedDeltaTime;
            diveVerticalMovementTimer += Time.fixedDeltaTime * (Mathf.PI * diveTimerMax * 3);

            //move player up and down in a parabola
            if (diveVerticalMovementTimer < Mathf.PI) {
                visualChild.position = new Vector3(transform.position.x, Mathf.Sin(diveVerticalMovementTimer) /2, transform.position.z);
            }
            //rotate visual
            if (diveRotationTimer < diveRotationTimerMax) {
                diveRotationTimer += Time.fixedDeltaTime;
                visualChild.eulerAngles = visualChild.eulerAngles + (visualChild.right * 120 * Time.fixedDeltaTime);
            }

            //descelerate over time
            movement -= movement * diveDescel * Time.fixedDeltaTime;

            //grow box collider ans move it backwards as I move forward
            boxCollider.size += new Vector3(0, 0, movement.magnitude /2);
            boxCollider.center -= new Vector3(0, 0, movement.magnitude/2);

            //create a trail behind the dive
            if (diveTrailTimer < diveTimer && diveTrailTimer > 0) diveTrailTimer -= Time.fixedDeltaTime;
            else {
                dashTrail = Instantiate(dashTrailPrefab, transform.position, transform.rotation);
                if (dashTrail.GetComponent<Trail>().LifeTimerMax > dashTimer) dashTrail.GetComponent<Trail>().SetLifeTimerMax(diveTimer); //make sure no dashTrail survives beyond the dash
                diveTrailTimer = diveTrailTimerMax;
            }
        }

        //DASHING LOGIC
        else if (dashTimer > 0) {
            dashTimer -= Time.fixedDeltaTime;
            if (dashTrailTimer < dashTimer && dashTrailTimer > 0) dashTrailTimer -= Time.fixedDeltaTime;
            else {
                dashTrail = Instantiate(dashTrailPrefab, transform.position, transform.rotation);
                if (dashTrail.GetComponent<Trail>().LifeTimerMax > dashTimer) dashTrail.GetComponent<Trail>().SetLifeTimerMax(dashTimer); //make sure no dashTrail survives beyond the dash
                dashTrailTimer = dashTrailTimerMax;
            }
        }
        //MOVEMENT LOGIC - only control movement if not dashing nor diving
        else { 
            if (diving) { //logic for a dive just ending
                diving = false;
                diveTrailTimer = 0;
                movement = Vector3.zero;
                visualChild.position = transform.position;
                visualChild.rotation = transform.rotation;
                boxCollider.size = new Vector3(1, 1, 1);
                boxCollider.center = Vector3.zero;
            }

            if (dashing) { //logic for a dash just ending
                dashing = false;
                dashTrailTimer = 0;
                movement = Vector3.zero;
            }

            movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")) * speed * Time.fixedDeltaTime;
        }
        rb.MovePosition(transform.position + movement);
    }

    private void OnTriggerEnter(Collider other) {
        //LOGIC FOR COLLIDING WITH A FLYING BALL
        if (other.gameObject.GetComponent<BallFlying>()) {
            numberOfBalls = 1;
            myColor = other.gameObject.GetComponent<BallFlying>().GetMyColor;
            if (myColor == MyColor.Blue) {
                myMat.color = Constants.blue;
            }
            else if (myColor == MyColor.Pink) {
                myMat.color = Constants.pink;
            }
            else if (myColor == MyColor.Yellow) {
                myMat.color = Constants.yellow;
            }
            marker.GetComponent<Renderer>().material.color = myMat.color;
            arrow.GetComponent<Renderer>().material.color = myMat.color;
            other.gameObject.GetComponent<BallFlying>().DestroyThisBall(false);
        }
    }
}
