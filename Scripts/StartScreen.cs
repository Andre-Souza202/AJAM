using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Floresta()
    {
        SceneManager.LoadScene(1);
    }

    public void Oceano()
    {
        SceneManager.LoadScene(2);
    }

    public void Cidade()
    {
        SceneManager.LoadScene(3);
    }
}
