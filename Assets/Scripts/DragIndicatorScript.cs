using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragIndicatorScript : MonoBehaviour
{

    Vector3 startPos;
    Vector3 endPos;
    Camera camera;
    LineRenderer lr;

    Vector3 camOffset = new Vector3(0, 0, 10);

    [SerializeField] AnimationCurve ac;

    public Collider rightCube;
    public Collider leftCube;


    public delegate void OnClickEvent(Vector3 startPos, Vector3 endPos);
    public static event OnClickEvent onClickEvent;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {


        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

                if (lr == null)
            {
                lr = gameObject.AddComponent<LineRenderer>();
            }
            lr.enabled = true;
            lr.positionCount = 2;
            lr.sortingOrder = 2;
            startPos = camera.ScreenToWorldPoint(Input.mousePosition) + camOffset;
            lr.SetPosition(0, startPos);
            lr.useWorldSpace = true;
            lr.widthCurve = ac;
            lr.numCapVertices = 10;
        }
        if (Input.GetMouseButton(0))
        {
            endPos = camera.ScreenToWorldPoint(Input.mousePosition) + camOffset;
            Vector3 pos = endPos - startPos;

           if(lr != null && lr.enabled)
            {
                lr.SetPosition(1, startPos + pos.normalized);

            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if(lr!= null && lr.enabled)
            {
                onClickEvent?.Invoke(startPos, endPos);
                lr.enabled = false;

            }
        }
    }
}