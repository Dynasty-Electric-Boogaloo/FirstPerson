using UnityEngine;

public class MusicBoxObject : MonoBehaviour
{
    [SerializeField] private GameObject ballerina;
    [SerializeField] private GameObject key;
    [SerializeField] private GameObject picture;

    [SerializeField] private float maxRotateSpeed = 2;

    public void SetLevel(int level)
    {
        switch (level)
        {
            case 0: 
                ballerina.SetActive(false);
                key.SetActive(false);
                picture.SetActive(false);
                break;
            case 1:
                ballerina.SetActive(true);
                key.SetActive(false);
                picture.SetActive(false);
                break;
            case 2:
                ballerina.SetActive(true);
                key.SetActive(true);
                picture.SetActive(false);
                break;
            case 3:
                ballerina.SetActive(true);
                key.SetActive(true);
                picture.SetActive(true);
                break;
        }
    }

    public void Using(float distance, Vector3 lookingAtAngle)
    {
        key.transform.Rotate(0,0,maxRotateSpeed -distance);
        
        ballerina.transform.LookAt(Monster.MonsterRoot.GetMonsterPosition());
        var rotation = ballerina.transform.rotation.eulerAngles;
        rotation.x = 0;
        rotation.z = 0;
        ballerina.transform.eulerAngles = rotation;
    }
}
