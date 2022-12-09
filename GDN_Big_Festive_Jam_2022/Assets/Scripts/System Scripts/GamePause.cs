using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GamePause
{
    public static bool gamePaused;
    public static float deltaTime { get { return gamePaused ? 0 : Time.deltaTime; } }
    public static float fixedDeltaTime { get { return gamePaused ? 0 : Time.fixedDeltaTime; } }
}
