using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    float speed = 30f;
    float timerToDestroy = 4f;
    public Vector3 goPos;
    public Vector3 mousePosition;
    float animationTimer = 0.6f;
    float velocityAnim = 4f;
    bool gotDirection = false;
    public Rigidbody2D rb;

    void Update()
    {
        if (animationTimer > 0)
        {
            animationTimer = animationTimer - Time.deltaTime;
            Vector2 positionX = new Vector2(transform.position.x + 6 * Time.deltaTime, transform.position.y);
            transform.position = Vector2.MoveTowards(positionX, goPos, velocityAnim * Time.deltaTime);
        }
        else if(gotDirection == false)
        {
            var screenPoint = Camera.main.WorldToScreenPoint(transform.localPosition);
            var offset = new Vector2(mousePosition.x - screenPoint.x, mousePosition.y - screenPoint.y);
            var angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            gotDirection = true;
            
        }
        else
        {
            //transform.position += transform.right * Time.deltaTime * speed;
            rb.velocity = transform.right * Time.deltaTime * speed * 300;
        }

        if (timerToDestroy <= 0)
        {
            Destroy(this.gameObject);
        }
        timerToDestroy = timerToDestroy - Time.deltaTime;
    }

    public void UpdatePos(Transform pos)
    {
        goPos = pos.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "inimigo" || collision.gameObject.tag == "raia")
        {
            collision.gameObject.GetComponent<Enemy>().isStuned = true;
            Destroy(this.gameObject);
        }
    }
}
