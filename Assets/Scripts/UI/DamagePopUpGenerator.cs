using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopUpGenerator : MonoBehaviour
{
    private Camera cam;
    public static DamagePopUpGenerator current;
    public GameObject prefab;


    private void Awake()
    {
        cam = Camera.main;
        current = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.forward = cam.transform.forward;
    }

    public void CreatePopUp(Vector3 position, string text)
    {
        var popup = Instantiate(prefab, position, Quaternion.identity);
        var temp = popup.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        temp.text = text;

        Destroy(popup, 1f);
    }
}
