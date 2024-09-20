using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    float speed = 0.25f;
    float dirX;
    float dirY;
    [SerializeField] Transform cameraFollow;
    [SerializeField] Transform sprite_karakter;
    [SerializeField] Animator animator_karakter;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        cameraFollow.transform.position = new Vector3(transform.position.x, transform.position.y, -10f);

        FlipSprite();

        if (dirX != 0 || dirY != 0)
        {
            animator_karakter.SetBool("Walk", true);
        }
        else
        {
            animator_karakter.SetBool("Walk", false);
        }
    }

    void FixedUpdate()
    {
        dirX = Input.GetAxis("Horizontal") * speed;
        dirY = Input.GetAxis("Vertical") * speed;

        transform.Translate(dirX, dirY, 0);
    }

    void FlipSprite()
    {
        if (dirX < 0)
        {
            sprite_karakter.localScale = new Vector2(-1f, 1);
        }
        if (dirX > 0)
        {
            sprite_karakter.localScale = new Vector2(1f, 1);
        }
    }
}
