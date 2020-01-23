using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateCircleScript : MonoBehaviour
{

    public Material mat;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void CreateCircleObject(float radius, int vertexCount, float width, int axis,string num = "")
    {
        GameObject go = new GameObject("Circle"+ num);
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

        //Material material = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Material>("Default-Material.mat");
            lineRenderer.material = Resources.Load<Material>("Metal");
    }
}
