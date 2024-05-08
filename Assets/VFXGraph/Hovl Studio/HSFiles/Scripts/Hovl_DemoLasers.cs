using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using System;
using UnityEngine;

public class Hovl_DemoLasers : MonoBehaviour
{
    private PlayerController playerController;
    private Animator animator;
    private AudioSource audioSource;
    public AudioClip laserSFX;


    private bool playerPower;
    public GameObject AimPos;
    public GameObject FirePoint;
    private bool isShooting = false;
    public Camera Cam;
    public Transform cam;
    public float MaxLength;
    public GameObject[] Prefabs;

    private Ray RayMouse;
    private Vector3 direction;
    private Quaternion rotation;

    [Header("GUI")]
    private float windowDpi;

    private int Prefab;
    private GameObject Instance;
    private Hovl_Laser LaserScript;
    private Hovl_Laser2 LaserScript2;

    //Double-click protection
    private float buttonSaver = 0f;

    void Start ()
    {
        audioSource = GetComponent<AudioSource>();
        cam = Camera.main.transform;
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        //LaserEndPoint = new Vector3(0, 0, 0);
        if (Screen.dpi < 1) windowDpi = 1;
        if (Screen.dpi < 200) windowDpi = 1;
        else windowDpi = Screen.dpi / 200f;
        Counter(0);
    }

    void Update()
    {
        
        //Enable lazer
        if (Input.GetMouseButtonDown(0) && playerController.beamIsActivated && playerController.manaSystem.GetMana() >= 0)
        {
            isShooting = true;
            audioSource.clip = laserSFX;
            audioSource.loop = true;
            audioSource.Play();
            StartCoroutine(PlayCastAnimation());
            Destroy(Instance);
            if (playerPower) //True == Dark Type Beam
            {
                Prefab = 0; //Dark Type Beam
            }
            else if (!playerPower) //False == Light Type Beam
            {
                Prefab = 1; // Light Type Beam
            }
            Instance = Instantiate(Prefabs[Prefab], FirePoint.transform.position, FirePoint.transform.rotation);
            Instance.transform.parent = transform;
            LaserScript = Instance.GetComponent<Hovl_Laser>();
            LaserScript2 = Instance.GetComponent<Hovl_Laser2>();
        }

        //Disable lazer prefab
        if (Input.GetMouseButtonUp(0) && playerController.beamIsActivated)
        {
            audioSource.clip = null;
            audioSource.loop = false;
            audioSource.Stop();
            isShooting = false;
            animator.SetBool("isCasting", false);
            playerController.SetBoolCasting(false);
            if (LaserScript) LaserScript.DisablePrepare();
            if (LaserScript2) LaserScript2.DisablePrepare();
            Destroy(Instance,1);
        }

        //To change lazers
        //if ((Input.GetKey(KeyCode.A) || Input.GetAxis("Horizontal") < 0) && buttonSaver >= 0.4f)// left button
        //{
        //    buttonSaver = 0f;
        //    Counter(-1);
        //}
        //if ((Input.GetKey(KeyCode.D) || Input.GetAxis("Horizontal") > 0) && buttonSaver >= 0.4f)// right button
        //{
        //    buttonSaver = 0f;
        //    Counter(+1);         
        //}

        buttonSaver += Time.deltaTime;

        //Current fire point
        if (cam != null && isShooting && playerController.manaSystem.GetMana() > 0)
        {
            playerController.manaSystem.Consume(20 * Time.deltaTime);
            RaycastHit hit; //DELATE THIS IF YOU WANT TO USE LASERS IN 2D
            var mousePos = Input.mousePosition;
            RayMouse = Cam.ScreenPointToRay(mousePos);

            //ADD THIS IF YOU WANT TO USE LASERS IN 2D: RaycastHit2D hit = Physics2D.Raycast(RayMouse.origin, RayMouse.direction, MaxLength);
            if (Physics.Raycast(FirePoint.transform.position, gameObject.transform.forward, out hit, MaxLength)) //CHANGE THIS IF YOU WANT TO USE LASERRS IN 2D: if (hit.collider != null)
            {
                if (hit.collider.tag == "Enemy")
                {
                    
                        hit.collider.GetComponent<HealthSystem>().Damage(playerController.GetPlayerDamage() * 3 * Time.deltaTime);
                    
                }
                //RotateToMouseDirection(gameObject, hit.point);
                //LaserEndPoint = hit.point;
            }
            else
            {
                //var pos = RayMouse.GetPoint(MaxLength);
                //RotateToMouseDirection(gameObject, pos);
                //LaserEndPoint = pos;
            }
        }
        else
        {
            
        }
    }


    IEnumerator PlayCastAnimation()
    {
        animator.SetBool("isCasting", true);
        playerController.SetBoolCasting(true);
        yield return new WaitForSeconds(1);
    }

    public void PlayerPowerType(bool value)
    {
        playerPower = value;
    }


    //GUI Text
    //void OnGUI()
    //{
    //    GUI.Label(new Rect(10 * windowDpi, 5 * windowDpi, 400 * windowDpi, 20 * windowDpi), "Use the keyboard buttons A/<- and D/-> to change lazers!");
    //    GUI.Label(new Rect(10 * windowDpi, 20 * windowDpi, 400 * windowDpi, 20 * windowDpi), "Use left mouse button for shooting!");
    //}

    //To change prefabs (count - prefab number)
    void Counter(int count)
    {
        Prefab += count;
        if (Prefab > Prefabs.Length - 1)
        {
            Prefab = 0;
        }
        else if (Prefab < 0)
        {
            Prefab = Prefabs.Length - 1;
        }
    }
  
    //To rotate fire point
    void RotateToMouseDirection (GameObject obj, Vector3 destination)
    {
        direction = destination - obj.transform.position;
        rotation = Quaternion.LookRotation(direction);     
        obj.transform.localRotation = Quaternion.Lerp(obj.transform.rotation, rotation, 1);
    }
}
