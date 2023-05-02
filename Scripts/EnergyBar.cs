using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
    public Slider slider;
    public ScriptHandler scriptHandler;
    private float energyReset;
    private float energyTimer = 0.08f;

    void Update()
    {
        //Energia se perde lentamente com o tempo
        LoseEnergy(energyTimer*Time.deltaTime);

        if(slider.value <= 0)
        {
            scriptHandler.GameOver();

        }
    }

    public void MaxEnergy(float maxEnergy)
    {
        energyReset = maxEnergy;
    }

    //Para colocar ENERGIA MAXIMA
    public void SetMaxEnergy()
    {
        slider.maxValue = energyReset;
        slider.value = energyReset;
    }

    //Para COLOCAR UM VALOR ESPECIFICO na energia
    public void SetEnergy(float energy)
    {
        slider.value = energy;
    }

    //Para DIMINUIR a energia por um certo valor
    public void LoseEnergy(float energy)
    {
        slider.value = slider.value - energy;
    }

    //Para AUMENTAR a energia por um certo valor
    public void GainEnergy(float energy)
    {
        slider.value = slider.value + energy;
    }
    
    //Para MUDAR O VALOR PERDIDO COM O TEMPO da energia;
    public void setEnergyTimer(float newTimer)
    {
        energyTimer = newTimer;
    }
}
