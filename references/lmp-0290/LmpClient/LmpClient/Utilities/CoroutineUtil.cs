// Decompiled with JetBrains decompiler
// Type: LmpClient.Utilities.CoroutineUtil
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System;
using System.Collections;
using UnityEngine;

namespace LmpClient.Utilities
{
  public class CoroutineUtil
  {
    public static void StartConditionRoutine(
      string routineName,
      Action action,
      Func<bool> condition,
      float maxFrameTries)
    {
      MainSystem.Singleton.StartCoroutine(CoroutineUtil.RoutineWithCondition(routineName, action, condition, maxFrameTries));
    }

    public static void StartFrameDelayedRoutine(string routineName, Action action, int framesDelay) => MainSystem.Singleton.StartCoroutine(CoroutineUtil.DelayFrames(routineName, action, framesDelay));

    public static void StartDelayedRoutine(string routineName, Action action, float delayInSec) => MainSystem.Singleton.StartCoroutine(CoroutineUtil.DelaySeconds(routineName, action, delayInSec));

    public static void ExecuteFrameAction(string routineName, Action action, int amountOfFrames) => MainSystem.Singleton.StartCoroutine(CoroutineUtil.RunForFrames(routineName, action, amountOfFrames));

    public static void ExecuteAction(string routineName, Action action, float amountOfSeconds) => MainSystem.Singleton.StartCoroutine(CoroutineUtil.RunForSeconds(routineName, action, amountOfSeconds));

    private static IEnumerator RunForFrames(
      string routineName,
      Action action,
      int amountOfFrames)
    {
      while (amountOfFrames > 0)
      {
        try
        {
          action();
        }
        catch (Exception ex)
        {
          LunaLog.LogError(string.Format("Error in run coroutine: {0}. Details {1}", (object) routineName, (object) ex));
        }
        action();
        --amountOfFrames;
        yield return (object) 0;
      }
    }

    private static IEnumerator RunForSeconds(
      string routineName,
      Action action,
      float amountOfSeconds)
    {
      while ((double) amountOfSeconds > 0.0)
      {
        try
        {
          action();
        }
        catch (Exception ex)
        {
          LunaLog.LogError(string.Format("Error in run coroutine: {0}. Details {1}", (object) routineName, (object) ex));
        }
        amountOfSeconds -= Time.deltaTime;
        yield return (object) 0;
      }
    }

    private static IEnumerator DelaySeconds(
      string routineName,
      Action action,
      float delayInSec)
    {
      if ((double) delayInSec > 0.0)
        yield return (object) new WaitForSeconds(delayInSec);
      try
      {
        action();
      }
      catch (Exception ex)
      {
        LunaLog.LogError(string.Format("Error in delayed coroutine: {0}. Details {1}", (object) routineName, (object) ex));
      }
    }

    private static IEnumerator DelayFrames(
      string routineName,
      Action action,
      int framesToDelay)
    {
      int frames = 0;
      while (frames < framesToDelay)
      {
        ++frames;
        yield return (object) 0;
      }
      try
      {
        action();
      }
      catch (Exception ex)
      {
        LunaLog.LogError(string.Format("Error in delayed coroutine: {0}. Details {1}", (object) routineName, (object) ex));
      }
    }

    private static IEnumerator RoutineWithCondition(
      string routineName,
      Action action,
      Func<bool> condition,
      float maxFrameTries)
    {
      int tries = 0;
      while (!condition() && (double) tries < (double) maxFrameTries)
      {
        ++tries;
        yield return (object) 0;
      }
      if (condition())
      {
        try
        {
          action();
        }
        catch (Exception ex)
        {
          LunaLog.LogError(string.Format("Error in coroutine: {0}. Details {1}", (object) routineName, (object) ex));
        }
      }
    }
  }
}
