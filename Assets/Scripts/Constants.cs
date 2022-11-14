using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MyColor { Blue, Pink, Yellow, Blank };
public enum Weather { Sunny, Cloudy, Raining, Snowing };
public static class Constants
{
    public static Color blue = new Color(.63f, .66f, .9f);
    public static Color pink = new Color(.9f, .63f, .8f);
    public static Color yellow = new Color(.9f, .9f, .63f);
    public static Color blank = new Color(.85f, .85f, .85f);
    public static Vector2 levelGroundSize = new Vector2(48f, 48f);
}
