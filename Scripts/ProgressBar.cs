using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] Slider slider;


    void Start()
    {
        slider = GetComponent<Slider>();
    }

    void Update()
    {
        slider.value = player.transform.position.x;
    }
}
