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

    public void Using(float distance )
    {
        key.transform.Rotate(0,0,distance *maxRotateSpeed );
        
        ballerina.transform.LookAt(Monster.MonsterRoot.GetMonsterPosition());
        var rotation = ballerina.transform.rotation.eulerAngles;
        rotation.x = 0;
        rotation.z = 0;
        ballerina.transform.eulerAngles = rotation;
        
    }
}
