using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] private MyColor myColor;
    private bool flashing, moving, movingDown;
    private float defaultPosY;
    private float halfMapSize = 21;

    void Start() {
        defaultPosY = transform.position.y;
        movingDown = true;
        Material myMat = GetComponent<Renderer>().material;
        if (myColor == MyColor.Blue) myMat.color = Constants.blue;
        else if (myColor == MyColor.Pink) myMat.color = Constants.pink;
        else if (myColor == MyColor.Yellow) myMat.color = Constants.yellow;
    }

    private void FixedUpdate() {
        if (moving) {
            if (movingDown) {
                if (transform.position.y > defaultPosY - transform.localScale.y) {
                    transform.position -= new Vector3(0, 3f * Time.deltaTime, 0);
                }
                else {
                    movingDown = false;
                    NewGoalPosition();
                }
            }
            else { //moving up
                if (transform.position.y < defaultPosY) {
                    transform.position += new Vector3(0, 3f * Time.deltaTime, 0);
                }
                else {
                    movingDown = true;
                    moving = false;
                    transform.position = new Vector3(transform.position.x, defaultPosY, transform.position.z);
                }

            }
        }
    }

    private void NewGoalPosition() {
        transform.position = new Vector3(Random.Range(-halfMapSize, halfMapSize), transform.position.y, Random.Range(-halfMapSize, halfMapSize));
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.GetComponent<BallThrown>()) {
            if (myColor == other.gameObject.GetComponent<BallThrown>().myColor) {
                GameManager.Instance.points++;
                moving = true;
                ChangeWeather();
            }
            Destroy(other.gameObject);
        }
    }

    /// <summary>
    /// Change the weather to match the weather at this goal's position
    /// </summary>
    private void ChangeWeather() {
        Vector2 longitudeAndLatitude = Utilities.GameLocationToLongitudeLatitude(transform.position.x, transform.position.z);
        WeatherInfo weatherInfo = Utilities.GetWeatherInfo(longitudeAndLatitude.x, longitudeAndLatitude.y);
        Weather weather = Utilities.GetWeatherFromWeatherInfo(weatherInfo);
        WeatherEffects weatherEffects = GameObject.FindObjectOfType<WeatherEffects>();
        weatherEffects.SetWeather(weather);
    }
}
