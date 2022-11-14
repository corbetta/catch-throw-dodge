using UnityEngine;
using System.Net;
using System.IO;

public static class Utilities
{
    /// <summary>
    /// Given a latitude and longitude, return current weather information using the open-meteo API
    /// </summary>
    public static WeatherInfo GetWeatherInfo(float latitude, float longitude) {
        //cast latitude and longitude to strings
        string latitudeString = latitude.ToString();
        string longitudeString = longitude.ToString();

        //concatenate URL + latitude + longitude
        string requestURL = "https://api.open-meteo.com/v1/forecast?latitude=" + latitudeString + "&longitude=" + longitudeString + "&current_weather=true";

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestURL);
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonInfo = reader.ReadToEnd();
        Debug.Log(jsonInfo);
        return JsonUtility.FromJson<WeatherInfo>(jsonInfo);
        //open-meteo API documentation: https://open-meteo.com/en/docs
    }

    /// <summary>
    /// Given an in-world location, return a real-world longitude and latitude based on the world map
    /// </summary>
    public static Vector2 GameLocationToLongitudeLatitude(float locationX, float locationZ) {
        float longitude; 
        float latitude;

        longitude = (locationX / (Constants.levelGroundSize.x / 2)) * 180;
        latitude = (locationZ / (Constants.levelGroundSize.y / 2)) * 90;

        return new Vector2(longitude, latitude);   
    }

    /// <summary>
    /// Find the weatherCode from weatherInfo and turn it into a Weather enum. 
    /// weatherCode follows the WMO weather interpretation codes that can be found at the bottom of this page: https://open-meteo.com/en/docs 
    /// </summary>
    public static Weather GetWeatherFromWeatherInfo(WeatherInfo weatherInfo) {
        
        float weatherCode = weatherInfo.current_weather.weathercode;

        Debug.Log("This is the weather code: " + weatherCode);

        //check for three weather conditions - cloudy, raining, or snowing
        if (weatherCode > 1 && weatherCode < 50) {
            return Weather.Cloudy;
        }
        else if ((weatherCode >= 50 && weatherCode < 70) || (weatherCode >= 80 && weatherCode < 85) || (weatherCode > 90)) {
            return Weather.Raining;
        }
        else if ((weatherCode >= 70 && weatherCode < 80) || (weatherCode >= 85 && weatherCode < 90)) {
            return Weather.Snowing;
        }
        
        //if none of these weather conditions are true, return sunny
        return Weather.Sunny;
    }
}
