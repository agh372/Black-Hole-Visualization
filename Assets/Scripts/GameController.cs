using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour, IPointerClickHandler // 2
     , IDragHandler
     , IPointerEnterHandler
     , IPointerExitHandler
{
    public InputField GravitationalFieldInputField;
    public InputField SpeedLightInputField;
    int num = 0;

    public InputField MassInputField;

    public float x = 0; // Event horizon at x = 455, y = 250
    public float y = 0;

    public float c = 0.2f; // speed of light

    //vx = c;
    //vy = 0;

    public float vx = 0;
    public float vy;

    public float deltaVx = 0;
    public float deltaVy = 0;

    public float theta = 0;

    public float mass = 3000f;
    public float dt = 0.0000000000001f;

    public float x_massive = 0f;
    public float y_massive = 0f;

    public float M = 40f;
    public float G = 1f;

    private float prevG;

    public GameObject obj;



    float vel_angle;
    float pox;
    float poy;


    // Start is called before the first frame update

    public List<Vector3> history;

    public List<GameObject> pathGameObjects;

    public GameObject blackHole;

    GraphicRaycaster raycaster;
    PointerEventData pointerEventData;
    EventSystem eventSystem;
    bool isInteractingWithUI = false;
    public GameObject eventHorizon;

    int count = 0;


    public Transform visualizer;


    public Toggle blackHoleToggle;
    private bool blackHoleToggleBool;


    Vector3 playerPosScreen;
    Vector3 wrld;
    float half_szX;
    float half_szY;

    public GameObject myPrefab;
    public GameObject sampleObj;



  float StartPointX = 0;
 float StartPointY = 0;
 float ControlPointX = 20;
 float ControlPointY = 50;
 float EndPointX  = 50;
 float EndPointY = 0;
 float CurveX;
 float CurveY;

    float BezierTime = 0;
    GameObject prefab;
    public ParticleSystem particleSystem;

    public Text radiusText;

    void Start()
    {
        vy = c;
        DragIndicatorScript.onClickEvent += qwerty;
        raycaster = FindObjectOfType<GraphicRaycaster>();
        eventSystem = FindObjectOfType<EventSystem>();

        GravitationalFieldInputField.text = G.ToString();
        SpeedLightInputField.text = c.ToString();
        MassInputField.text = M.ToString();


        StartPointX = sampleObj.transform.position.x;
        StartPointY = sampleObj.transform.position.y;
        EndPointX = blackHole.transform.position.x;
        EndPointY = blackHole.transform.position.y;


        ControlPointX = sampleObj.transform.position.x + 5.0f;
        ControlPointY = sampleObj.transform.position.y + 5.0f;


    }


    public void UpdateBlackHoleToggle()
    {
        if (blackHoleToggle.isOn)
        {
            blackHoleToggleBool = true;
            G = prevG;
            GravitationalFieldInputField.text = G.ToString();

        }
        else
        {
            blackHoleToggleBool = false;
            G = 0;
        }
    }

    public void qwerty(Vector3 startPos, Vector3 endPos)
    {

        vx = 0.2f * (endPos.x - startPos.x);
        vy = 0.2f * (endPos.y - startPos.y);
        vel_angle = Mathf.Atan2(vy, vx);
        vx = c * Mathf.Cos(vel_angle);
        vy = c * Mathf.Sin(vel_angle);
        x = startPos.x;
        y = startPos.y;

        for (int i = 0; i > pathGameObjects.Count; i++)
        {
            pathGameObjects[i].transform.position = new Vector3(0, 0, 0);


        }
        history.Clear();

        count = 0;
    }
    // Update is called once per frame
    void Update()
    {

        Vector2 playerPosScreen = Camera.main.WorldToScreenPoint(visualizer.transform.position);
        if (playerPosScreen.x > Screen.width)
        {
            visualizer.transform.position =
                Camera.main.ScreenToWorldPoint(
                    new Vector3(Screen.width,
                                playerPosScreen.y,
                                visualizer.transform.position.z - Camera.main.transform.position.z));
        }
        else if (playerPosScreen.x < 0.0f)
        {
            visualizer.transform.position =
                Camera.main.ScreenToWorldPoint(
                    new Vector3(0.0f,
                                playerPosScreen.y,
                                visualizer.transform.position.z - Camera.main.transform.position.z));
        }




       

        x_massive = blackHole.transform.position.x;
        y_massive = blackHole.transform.position.y;

        vx += deltaVx;
        vy += deltaVy;

        // Update location
        x += vx * dt;
        y += vy * dt;

        // velocity is unchanged if there are no forces
        deltaVx = 0;
        deltaVy = 0;

        float r = Mathf.Sqrt((obj.transform.position.x - x_massive) * (obj.transform.position.x - x_massive) +
            (obj.transform.position.y - y_massive) * (obj.transform.position.y - y_massive));
        float Fgrav = G * M * mass / (r * r);

        theta = Mathf.Atan2(y - y_massive, x - x_massive);

        float Fx = -Fgrav * Mathf.Cos(theta);
        deltaVx = (Fx / mass) * dt; // = ax*dt;

        float Fy = -Fgrav * Mathf.Sin(theta);
        deltaVy = (Fy / mass) * dt; // = ay*dt;


        float scale = (Mathf.Sqrt((Fx * Fx) + (Fy * Fy)))/ 251060f;

        Vector3 scaleTransform = new Vector3(visualizer.localScale.x, visualizer.localScale.y, visualizer.localScale.z);
        scaleTransform.y = scale;
        visualizer.localScale = scaleTransform;
        ///Debug.Log("Magnitude: " + scale);

        obj.transform.position = new Vector3(x, y, 0);

        radiusText.text = "Radius: " + ((G * M) / (c * c)).ToString("F2");
        if (count % 2 == 0)
            history.Add(obj.transform.position);

        if (history.Count > 1)
        {
            int c = 0;
            for (int i = history.Count - 2; i > 0; i--)
            {
                if (c < 30)
                {
                    pathGameObjects[c].transform.position = history[i];

                }
                c++;
            }
        }
        count++;
        // UpdateShip();

    }


    public void OnPointerClick(PointerEventData eventData) // 3
    {
        Debug.Log("I was OnPointerClick");

    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("I'm being PointerEventData!");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("I'm being OnPointerEnter!");

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("I'm being OnPointerExit!");

    }


    public void SetGravitationalConstant(float speed)
    {
        if (GravitationalFieldInputField.text == "")
            G = 0;
        else
            G = float.Parse(GravitationalFieldInputField.text); //for float    }
       // Destroy(eventHorizon);
        Debug.Log("Radius: " + (G * M * mass / (c * c)));
        GetComponent<CreateCircleScript>().CreateCircleObject((G * M  / (c * c)), 40, 0.07f, 0,num.ToString());
        GameObject circle = GameObject.Find("Circle"+num.ToString());
        circle.transform.parent = blackHole.transform;
        eventHorizon = circle;

        for (int i = 0; i < blackHole.transform.childCount; i++)
        {
            if (i == blackHole.transform.childCount - 1)
                break;
            Destroy(blackHole.transform.GetChild(i).gameObject);

        }

        num++;
        prevG = G;

    }

    public void SetSpeedOfLight(float speed)
    {
        if (SpeedLightInputField.text == "")
            c = 0;
        else
            c = float.Parse(SpeedLightInputField.text); //for float    }

        Debug.Log("Radius: " + (G * M * mass / (c * c)));
        GetComponent<CreateCircleScript>().CreateCircleObject((G * M / (c * c)), 40, 0.07f, 0, num.ToString());
        GameObject circle = GameObject.Find("Circle" + num.ToString());
        circle.transform.parent = blackHole.transform;
        eventHorizon = circle;

        for (int i = 0; i < blackHole.transform.childCount; i++)
        {
            if (i == blackHole.transform.childCount - 1)
                break;
            Destroy(blackHole.transform.GetChild(i).gameObject);

        }

        num++;
    }

    public void SetMass(float speed)
    {
        if (MassInputField.text == "")
            M = 0;
        else
            M = float.Parse(MassInputField.text); 

        Debug.Log("Radius: " + (G * M * mass / (c * c)));
        GetComponent<CreateCircleScript>().CreateCircleObject((G * M / (c * c)), 40, 0.07f, 0, num.ToString());
        GameObject circle = GameObject.Find("Circle" + num.ToString());
        circle.transform.parent = blackHole.transform;
        eventHorizon = circle;

        for (int i = 0; i < blackHole.transform.childCount; i++)
        {
            if (i == blackHole.transform.childCount - 1)
                break;
            Destroy(blackHole.transform.GetChild(i).gameObject);

        }

        num++;
    }

    public void OnAddBlackHole()
    {
            prefab =   Instantiate(myPrefab, new Vector3(sampleObj.transform.position.x, sampleObj.transform.position.y, 0), Quaternion.identity);
        BezierTime = 0;

        StartCoroutine("SetGuard");
    }

         IEnumerator SetGuard()
     {
         while(BezierTime <= 1)
         {

            BezierTime = BezierTime + Time.deltaTime;

            if (BezierTime>= 1)
     {
                particleSystem.Play();
                StartCoroutine("SetParticle");

                CurveX = 0;
                CurveY = 0;
                Destroy(prefab);
            }

            if(prefab != null)
            {

           

            CurveX = (((1 - BezierTime) * (1 - BezierTime)) * StartPointX) + (2 * BezierTime * (1 - BezierTime) * ControlPointX) + ((BezierTime * BezierTime) * EndPointX);
            CurveY = (((1 - BezierTime) * (1 - BezierTime)) * StartPointY) + (2 * BezierTime * (1 - BezierTime) * ControlPointY) + ((BezierTime * BezierTime) * EndPointY);
            prefab.transform.position = new Vector3(CurveX, CurveY, 0);
            }
            yield return new WaitForSeconds(0.02f);
         }
     }


    IEnumerator SetParticle()
    {
        Debug.Log("STOOOP");
        yield return new WaitForSeconds(2f);
        particleSystem.Stop();

        M = M + 4;

        MassInputField.text = M.ToString();
    }


    void OnGUI()
    {
        if (GUI.Button(new Rect(1300, 10, 100, 30), "Analyze"))
        {
         //   Debug.Log("Scene2 loading: " + scenePaths[0]);
            SceneManager.LoadScene("Assets/BlackHole/TestScene/BlackHoleTestScene.unity", LoadSceneMode.Single);
        }
    }
}
