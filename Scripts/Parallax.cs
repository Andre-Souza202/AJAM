using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Parallax : MonoBehaviour
{
    [SerializeField] private Image fundo;
    [SerializeField] private ScriptHandler scriptHandler;
    [SerializeField] private float velocidade;
    [SerializeField] private GameObject camera;
    [SerializeField] private bool isDecoration = false;
    private int gameState;
    private float pos;
    private float newPos;

    [SerializeField] private float velocidadeY;
    private float posY;
    private float newPosY;

    void Start()
    {
        pos = camera.transform.position.x;
        posY = camera.transform.position.y;
    }

    void FixedUpdate()
    {
        gameState = scriptHandler.GetGameState();
        if (Time.timeScale > 0)
        {
            MoveFundo();
        }
    }

    public void MoveFundo()
    {
        newPos = (camera.transform.position.x - pos) / Time.deltaTime;
        pos = camera.transform.position.x;

        newPosY = (camera.transform.position.y - posY) / Time.deltaTime;
        posY = camera.transform.position.y;

        transform.position = new Vector3(transform.position.x - velocidade * Time.deltaTime * newPos / 10, transform.position.y - velocidadeY * Time.deltaTime * newPosY / 10, 0);
        if (isDecoration == false)
        {
            if (transform.localPosition.x >= fundo.sprite.rect.width * 5f)
            {
                transform.localPosition = new Vector3(transform.localPosition.x - fundo.sprite.rect.width * 5f * 2, transform.localPosition.y, 0);
            }
            else if (transform.localPosition.x <= -fundo.sprite.rect.width * 5f)
            {
                transform.localPosition = new Vector3(transform.localPosition.x + fundo.sprite.rect.width * 5f * 2, transform.localPosition.y, 0);
            }
        }    
    }
}

