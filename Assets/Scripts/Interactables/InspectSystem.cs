using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;

namespace Interactables
{
    public class InspectSystem : MonoBehaviour
    {
        private static InspectSystem _instance;

        [SerializeField] private Camera cam;
        [SerializeField] private Transform point;
        [SerializeField] private TMP_Text commentText;
        [SerializeField] private List<GameObject> showcasePrefab = new List<GameObject>();
        [SerializeField] private int inspectLayer;
        private readonly List<GameObject> _showcase= new List<GameObject>();
        private GameObject _current;
    
        public static bool isOn() => _instance && _instance.cam.gameObject.activeSelf;
        private void Awake()
        {
            if (_instance == null)
                _instance = this;
        }
        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }
    
        private void Start()
        {
            if(!_instance)
                return;

            _instance.cam.gameObject.SetActive(false);
            var renderers = new List<Renderer>();

            foreach (var obj in showcasePrefab)
            {
                var newObject = Instantiate(obj, point);
                _showcase.Add(newObject);
                newObject.SetActive(false);
                newObject.layer = inspectLayer;

                newObject.transform.localPosition = -GetObjectBounds(newObject).center;
            
                foreach (var child in newObject.GetComponentsInChildren<MeshRenderer>())
                    child.gameObject.layer = inspectLayer;
            }
            _instance.commentText.text = "";
            return;

            Bounds GetObjectBounds(GameObject boundObject)
            {
                boundObject.GetComponentsInChildren(renderers);
                var bounds = renderers[0].bounds;

                for (var i = 1; i < renderers.Count; i++)
                {
                    bounds.Encapsulate(renderers[i].bounds);
                }

                bounds.center -= boundObject.transform.position;
                return bounds;
            }
        }
    
        public static void Show (int index, string comment = "")
        {
            if(!_instance)
                return;
        
            _instance.cam.gameObject.SetActive(true);
            _instance._showcase[index].SetActive(true);
            _instance.point.localRotation = Quaternion.identity;
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
}
