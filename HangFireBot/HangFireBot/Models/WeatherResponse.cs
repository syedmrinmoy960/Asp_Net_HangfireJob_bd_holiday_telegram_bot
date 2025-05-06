public class WeatherResponse
{
    public Main Main { get; set; }
    public List<Weather> Weather { get; set; }
}

public class Main
{
    public double Temp { get; set; }  // Temperature in Celsius
}

public class Weather
{
    public string Description { get; set; }  // Weather condition (e.g., "clear sky", "storm")
}
