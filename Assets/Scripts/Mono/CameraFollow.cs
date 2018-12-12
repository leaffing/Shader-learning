#if !ECS
using UnityEngine;
public class CameraFollow : MonoBehaviour
{
    private GameObject player;

    void Start() => player = GameObject.FindWithTag("Player");

    void Update() => transform.position = new Vector3(player.transform.position.x, 
        player.transform.position.y, gameObject.transform.position.z);
}
#endif