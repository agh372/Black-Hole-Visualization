using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Create circle object in scene view. Called via menu [GameObject] -> [3D Object] -> [Circle...].
/// </summary>
public class CreateCircleMenu : EditorWindow
{
    /// <summary>Determine which direction the circle drawn.</summary>
    int m_selectionGrid;    // 0: X-Y, 1: X-Z, 2: Y-Z

    /// <summary>Captions for selection grid.</summary>
    string[] m_axisSettings = { "X-Y", "X-Z", "Y-Z" };

    /// <summary>Determine how big the circle is.</summary>
    string m_radius = "4";

    /// <summary>Determine how many points involved to draw a circle.</summary>
    string m_vertexCount = "40";

    /// <summary>Determine how thick the circle is.</summary>
    string m_width = "0.3";

    [MenuItem("GameObject/3D Object/Circle...")]
    public static void ShowDialog()
    {
        EditorWindow.GetWindow<CreateCircleMenu>("Circle Settings");
    }

    /// <summary>
    /// Build GUI for options.
    /// </summary>
    private void OnGUI()
    {
        GUILayout.Label("Radius");
        m_radius = GUILayout.TextArea(m_radius);

        GUILayout.Label("Vertex Count");
        m_vertexCount = GUILayout.TextArea(m_vertexCount);

        GUILayout.Label("Width");
        m_width = GUILayout.TextArea(m_width);

        GUILayout.Label("Direction");
        m_selectionGrid = GUILayout.SelectionGrid(m_selectionGrid, m_axisSettings, 3, "Toggle");

        if (GUILayout.Button("Create"))
        {
            try
            {
                CreateCircleObject(float.Parse(m_radius), int.Parse(m_vertexCount), float.Parse(m_width), m_selectionGrid);
            }
            catch (System.Exception e)
            {
                Debug.LogErrorFormat("Error. Check parameters for circle settings. Detailed info:\r\n{0}", e.ToString());
            }
        }
    }

    /// <summary>
    /// Create circle object.
    /// </summary>
    /// <param name="radius"></param>
    /// <param name="vertexCount"></param>
    /// <param name="width"></param>
    /// <param name="axis">Which direction the circle lies. Should be 0, 1, or 2.</param>
    private void CreateCircleObject(float radius, int vertexCount, float width, int axis)
    {
        GameObject go = new GameObject("Circle");
        LineRenderer lineRenderer = go.AddComponent<LineRenderer>();

        // Prepare for trigonometric function.
        float deltaTheta = (2f * Mathf.PI) / vertexCount;
        float theta = 0f;
        lineRenderer.positionCount = vertexCount;

        // Add points to line renderer.
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            float a = radius * Mathf.Cos(theta);
            float b = radius * Mathf.Sin(theta);
            Vector3 pos = Vector3.zero;

            switch (axis)
            {
                case 0: // X-Y
                    pos = new Vector3(a, b, 0f);
                    break;
                case 1: // X-Z
                    pos = new Vector3(a, 0f, b);
                    break;
                case 2: // Y-Z
                    pos = new Vector3(0f, a, b);
                    break;
                default:
                    Debug.LogError("Invalid value for axis: " + axis);
                    break;
            }

            lineRenderer.SetPosition(i, pos);
            theta += deltaTheta;
        }

        // Configure other required properties.
        lineRenderer.loop = true;
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lineRenderer.receiveShadows = false;
        lineRenderer.useWorldSpace = false;
        lineRenderer.widthMultiplier = width;

        Material material = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Material>("Default-Material.mat");
        if (material)
            lineRenderer.material = material;
    }
}
