[System.Serializable]

public class WeatherInfo
{
    public float latitude;
    public float longitude;
    public float generationtime_ms;
    public float utc_offset_seconds;
    public string timezone;
    public string timezone_abbreviation;
    public float elevation;
    public CurrentWeather current_weather;

    [System.Serializable]
    public class CurrentWeather
    {
        public float temperature;
        public float windspeed;
        public float winddirection;
        public int weathercode;
        public string time;
    }
}