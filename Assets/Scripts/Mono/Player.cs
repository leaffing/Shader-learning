#if !ECS
using UnityEngine;
public class Player : MonoBehaviour
{
    public bool Dead;
    private float speed;

    void Start() => speed = 5;

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector3 vector = new Vector3(x, y, 0).normalized * speed * Time.deltaTime;
        transform.position += vector;
    }
}
#endif