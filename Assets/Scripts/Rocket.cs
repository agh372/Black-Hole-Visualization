using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 10f;
    [SerializeField] float mainThrust = 10f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip explosion;
    [SerializeField] AudioClip win;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem explosionEngineParticles;
    [SerializeField] ParticleSystem winParticles;

    [SerializeField] float levelLoadDelay = 2f;
    [SerializeField] bool debugEnabled = true;

    Rigidbody rigidBody;
    AudioSource audioSource;

    int finishScene = 4;
    bool collisionsEnabled = true;








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


    public GameObject blackHole;

    public GameObject obj;

    float vel_angle;


    public Toggle rocketToggle;

    public Boolean rocketToggleBool;


    public Toggle blackHoleToggle;
    private bool blackHoleToggleBool;

    private bool firstTime = false;


    enum State
    {
        PreLaunch = 0,
        Alive = 1,
        Dying = 2,
        Transcending = 3
    }

    State state = State.PreLaunch;

    private Vector3 initialPosition;
    private Vector3 initialScale;

    public Transform rocket;


    // Start is called before the first frame update
    void Start()
    {

        if (Debug.isDebugBuild)
        {
            debugEnabled = true;
        }

        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        blackHoleToggleBool = true;
        rocketToggleBool = false;
        UpdateRocketToggle();

        UpdateBlackHoleToggle();

        initialPosition = rocket.transform.position;
        
    }

    public void UpdateRocketToggle()
    {
        rocketToggleBool = rocketToggle.isOn;
        if (blackHoleToggleBool && rocketToggleBool)
        {
            rigidBody.isKinematic = true;

        }
        else if(blackHoleToggleBool && !rocketToggleBool)
        {
            rigidBody.isKinematic = true;

        }
        else if (!blackHoleToggleBool && !rocketToggleBool)
        {
            rigidBody.isKinematic = true;

        }
        else if (!blackHoleToggleBool && rocketToggleBool)
        {
            rigidBody.isKinematic = false;

        }
    }

    public void UpdateBlackHoleToggle()
    {
        blackHoleToggleBool = blackHoleToggle.isOn;

        if (blackHoleToggleBool && rocketToggleBool)
        {
            rigidBody.isKinematic = true;

        }
        else if (blackHoleToggleBool && !rocketToggleBool)
        {
            rigidBody.isKinematic = true;

        }
        else if (!blackHoleToggleBool && !rocketToggleBool)
        {
            rigidBody.isKinematic = true;

        }
        else if (!blackHoleToggleBool && rocketToggleBool)
        {
            rigidBody.isKinematic = false;

        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided");
        switch (collision.gameObject.tag)
        {
            case "blackhole":
                //do nothing
                break;
        }
    }


    void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Collided");
        ResetRocket();

      
    }

    private void StartWinSequence()
    {
        state = State.Transcending;
        audioSource.PlayOneShot(win);
        winParticles.Play();
        Invoke("LoadNextScene", levelLoadDelay);
    }



    private void StartDeathSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(explosion);
        explosionEngineParticles.Play();
        Invoke("ReloadCurrentLevelWithDelay", levelLoadDelay);
    }

    void ReloadCurrentLevelWithDelay()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    void LoadNextScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.buildIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(currentScene.buildIndex + 1);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings);
        }
    }

    // Update is called once per frame
    void Update()
    {

        Vector2 playerPosScreen = Camera.main.WorldToScreenPoint(rocket.transform.position);

        if (playerPosScreen.x < 0.0f && !firstTime)
        {
            rocket.transform.position =
                Camera.main.ScreenToWorldPoint(
                    new Vector3(0.0f,
                                playerPosScreen.y,
                                rocket.transform.position.z - Camera.main.transform.position.z));

            initialPosition = rocket.transform.position;
            Debug.Log("initial position");
        }
        else
        {
            firstTime = true;
        }


        if (rocketToggleBool)
        {

            if (state == State.PreLaunch)
            {
                Thrust();
            }
            else if (state == State.Alive)
            {
                Thrust();
                Rotate();
            }
            if (debugEnabled == true)
            {
                RespondToDebugKeys();
            }
        }
        if (rocketToggleBool && blackHoleToggleBool)
        {


            x = obj.transform.position.x;
            y = obj.transform.position.y;


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

            //  Debug.Log(Fgrav);
            obj.transform.position = new Vector3(x, y, 0);
        }
        }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextScene();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsEnabled = !collisionsEnabled;
        }
    }

    private void Thrust()
    {

        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow))
        {
            ApplyThrust();
            if (state == State.PreLaunch) { state = State.Alive; }
        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust);

        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);

        }
        mainEngineParticles.Play();


    }

    private void Rotate()
    {
        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A) ||
        Input.GetKey(KeyCode.LeftArrow))
        {
            RotateManually(rotationThisFrame,0);

        }
        else if (Input.GetKey(KeyCode.D) ||
        Input.GetKey(KeyCode.RightArrow))
        {
            RotateManually(-rotationThisFrame,1);

        }

    }

    private void RotateManually(float rotationThisFrame,int dir)
    {
        rigidBody.freezeRotation = true; //take manual control of rotation
        transform.Rotate(Vector3.forward * rotationThisFrame);
        rigidBody.freezeRotation = false; //resume manual control of rotation
        if(dir == 1)
        {
            rotationThisFrame = -rotationThisFrame;
        }
        vx = 0.2f * (blackHole.transform.position - obj.transform.position).x;
        vy = 0.2f * (blackHole.transform.position - obj.transform.position).y;
        vel_angle = Mathf.Atan2(vy, vx);
        vx = c * Mathf.Cos(vel_angle);
        vy = c * Mathf.Sin(vel_angle);


    }


    public void ResetRocket()
    {
        Debug.Log("POSITION: " + initialPosition.x);
        rocket.position = initialPosition;
        rocket.rotation = new Quaternion(0,0,0,1);
        mainEngineParticles.Stop();

        vx = 0;
        vy = 0;
        deltaVx = 0;
        deltaVy = 0;
        rocketToggle.isOn = false;
    }


}