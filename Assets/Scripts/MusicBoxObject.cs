using UnityEngine;

public class MusicBoxObject : MonoBehaviour
{
    [SerializeField] private GameObject ballerina;
    [SerializeField] private GameObject key;
    [SerializeField] private GameObject picture;

    [SerializeField] private float maxRotateSpeed = 2;

    public void SetLevel(int level)
    {
        ballerina.SetActive(level >= 1);
        key.SetActive(level >= 2);
        picture.SetActive(level >= 3);
    }

    public void Using(Vector3 diff, Vector3 forward)
    {
        key.transform.Rotate(0,0, (1 - Mathf.Clamp01(diff.magnitude)) * maxRotateSpeed );
        
        if(diff.magnitude > 1) 
            return;
        
        var direction = diff;
        direction.y = 0;
        direction.Normalize();
        ballerina.transform.localRotation = Quaternion.Euler(0, Vector3.SignedAngle(-forward, direction, Vector3.up), 0);
    }
}
