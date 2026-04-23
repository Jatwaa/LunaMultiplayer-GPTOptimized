// Decompiled with JetBrains decompiler
// Type: LmpClient.DebugDrawer
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LmpClient
{
  [KSPAddon]
  public class DebugDrawer : MonoBehaviour
  {
    private static readonly List<DebugDrawer.Line> Lines = new List<DebugDrawer.Line>();
    private static readonly List<DebugDrawer.Point> Points = new List<DebugDrawer.Point>();
    private static readonly List<DebugDrawer.Trans> Transforms = new List<DebugDrawer.Trans>();
    public Material LineMaterial;

    public static void DebugLine(Vector3 start, Vector3 end, Color col) => DebugDrawer.Lines.Add(new DebugDrawer.Line(start, end, col));

    public static void DebugPoint(Vector3 start, Color col) => DebugDrawer.Points.Add(new DebugDrawer.Point(start, col));

    public static void DebugTransforms(Transform t) => DebugDrawer.Transforms.Add(new DebugDrawer.Trans(t.position, t.up, t.right, t.forward));

    private void Start()
    {
      Object.DontDestroyOnLoad((Object) this);
      if (!Object.op_Implicit((Object) this.LineMaterial))
      {
        Material material = new Material(Shader.Find("Hidden/Internal-Colored"));
        ((Object) material).hideFlags = (HideFlags) 61;
        this.LineMaterial = material;
        this.LineMaterial.SetInt("_SrcBlend", 1);
        this.LineMaterial.SetInt("_DstBlend", 0);
        this.LineMaterial.SetInt("_Cull", 0);
        this.LineMaterial.SetInt("_ZWrite", 0);
        this.LineMaterial.SetInt("_ZWrite", 8);
      }
      this.StartCoroutine(this.EndOfFrameDrawing());
    }

    private IEnumerator EndOfFrameDrawing()
    {
      Debug.Log((object) "DebugDrawer starting");
      while (true)
      {
        Camera cam;
        do
        {
          yield return (object) new WaitForEndOfFrame();
          cam = DebugDrawer.GetActiveCam();
        }
        while (Object.op_Equality((Object) cam, (Object) null));
        try
        {
          ((Component) this).transform.position = Vector3.zero;
          GL.PushMatrix();
          this.LineMaterial.SetPass(0);
          Matrix4x4 projectionMatrix = Matrix4x4.Perspective(cam.fieldOfView, cam.aspect, cam.nearClipPlane, float.MaxValue);
          GL.LoadProjectionMatrix(projectionMatrix);
          GL.MultMatrix(cam.worldToCameraMatrix);
          GL.Begin(1);
          foreach (DebugDrawer.Line line1 in DebugDrawer.Lines)
          {
            DebugDrawer.Line line = line1;
            DebugDrawer.DrawLine(line.Start, line.End, line.Color);
            line = new DebugDrawer.Line();
          }
          foreach (DebugDrawer.Point point1 in DebugDrawer.Points)
          {
            DebugDrawer.Point point = point1;
            DebugDrawer.DrawPoint(point.Pos, point.Color);
            point = new DebugDrawer.Point();
          }
          foreach (DebugDrawer.Trans transform in DebugDrawer.Transforms)
          {
            DebugDrawer.Trans t = transform;
            DebugDrawer.DrawTransform(t.Pos, t.Up, t.Right, t.Forward);
            t = new DebugDrawer.Trans();
          }
          projectionMatrix = new Matrix4x4();
        }
        catch (Exception ex)
        {
          Debug.Log((object) ("EndOfFrameDrawing Exception" + ex?.ToString()));
        }
        finally
        {
          GL.End();
          GL.PopMatrix();
          DebugDrawer.Lines.Clear();
          DebugDrawer.Points.Clear();
          DebugDrawer.Transforms.Clear();
        }
        cam = (Camera) null;
      }
    }

    private static Camera GetActiveCam()
    {
      if (!Object.op_Implicit((Object) HighLogic.fetch))
        return Camera.main;
      if (HighLogic.LoadedSceneIsEditor && Object.op_Implicit((Object) EditorLogic.fetch))
        return EditorLogic.fetch.editorCamera;
      return HighLogic.LoadedSceneIsFlight && Object.op_Implicit((Object) PlanetariumCamera.fetch) && Object.op_Implicit((Object) FlightCamera.fetch) ? (MapView.MapIsEnabled ? PlanetariumCamera.Camera : FlightCamera.fetch.mainCamera) : Camera.main;
    }

    private static void DrawLine(Vector3 origin, Vector3 destination, Color color)
    {
      GL.Color(color);
      GL.Vertex(origin);
      GL.Vertex(destination);
    }

    private static void DrawRay(Vector3 origin, Vector3 direction, Color color)
    {
      GL.Color(color);
      GL.Vertex(origin);
      GL.Vertex(Vector3.op_Addition(origin, direction));
    }

    private static void DrawTransform(
      Vector3 position,
      Vector3 up,
      Vector3 right,
      Vector3 forward,
      float scale = 1f)
    {
      DebugDrawer.DrawRay(position, Vector3.op_Multiply(up, scale), Color.green);
      DebugDrawer.DrawRay(position, Vector3.op_Multiply(right, scale), Color.red);
      DebugDrawer.DrawRay(position, Vector3.op_Multiply(forward, scale), Color.blue);
    }

    private static void DrawPoint(Vector3 position, Color color, float scale = 1f)
    {
      DebugDrawer.DrawRay(Vector3.op_Addition(position, Vector3.op_Multiply(Vector3.up, scale * 0.5f)), Vector3.op_Multiply(Vector3.op_UnaryNegation(Vector3.up), scale), color);
      DebugDrawer.DrawRay(Vector3.op_Addition(position, Vector3.op_Multiply(Vector3.right, scale * 0.5f)), Vector3.op_Multiply(Vector3.op_UnaryNegation(Vector3.right), scale), color);
      DebugDrawer.DrawRay(Vector3.op_Addition(position, Vector3.op_Multiply(Vector3.forward, scale * 0.5f)), Vector3.op_Multiply(Vector3.op_UnaryNegation(Vector3.forward), scale), color);
    }

    private struct Line
    {
      public readonly Vector3 Start;
      public readonly Vector3 End;
      public readonly Color Color;

      public Line(Vector3 start, Vector3 end, Color color)
      {
        this.Start = start;
        this.End = end;
        this.Color = color;
      }
    }

    private struct Point
    {
      public readonly Vector3 Pos;
      public readonly Color Color;

      public Point(Vector3 pos, Color color)
      {
        this.Pos = pos;
        this.Color = color;
      }
    }

    private struct Trans
    {
      public readonly Vector3 Pos;
      public readonly Vector3 Up;
      public readonly Vector3 Right;
      public readonly Vector3 Forward;

      public Trans(Vector3 pos, Vector3 up, Vector3 right, Vector3 forward)
      {
        this.Pos = pos;
        this.Up = up;
        this.Right = right;
        this.Forward = forward;
      }
    }
  }
}
