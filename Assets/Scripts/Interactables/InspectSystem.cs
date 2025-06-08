using System;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class InspectSystem : MonoBehaviour
{
    private static InspectSystem _instance;

    [SerializeField] private Camera cam;
    [SerializeField] private Transform point;
    [SerializeField] private TMP_Text commentText;
    [SerializeField] private List<GameObject> showcasePrefab = new List<GameObject>();
    private readonly List<GameObject> _showcase= new List<GameObject>();
    private GameObject _current;
    
    public static bool isOn() => _instance && _instance.cam.gameObject.activeSelf;
    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        if(!_instance)
            return;

        _instance.cam.gameObject.SetActive(false);

        foreach (var obj in showcasePrefab)
        {
            var newObject = Instantiate(obj, point);
            _showcase.Add(newObject);
            newObject.SetActive(false);
            newObject.layer = LayerMask.NameToLayer("Inspect");
            newObject.AddComponent<BoxCollider>();
            newObject.AddComponent<ShowCaseObject>();
            
            foreach (var child in newObject.GetComponentsInChildren<MeshRenderer>())
                child.gameObject.layer = LayerMask.NameToLayer("Inspect");
        }
        _instance.commentText.text = "";
    }
    
    public static void Show (int index, string comment = "")
    {
        if(!_instance)
            return;
        
        _instance.cam.gameObject.SetActive(true);
        _instance._showcase[index].SetActive(true);
        _instance._showcase[index].transform.rotation = Quaternion.identity;
        _instance.commentText.text = comment;
        _instance._current =  _instance._showcase[index];
        
        UiManager.SetInspect();
    }
    
    
    public static void Hide ()
    {
        if(!_instance)
            return;

        if(_instance._current)
            _instance. _current.SetActive(false);
        
        _instance.cam.gameObject.SetActive(false);
        _instance.commentText.text = "";
        PauseManager.PauseGame(false, false);
    }
}
