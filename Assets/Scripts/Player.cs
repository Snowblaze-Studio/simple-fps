using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

class Player : MonoBehaviour
{
    [SerializeField]
    private Slider hitPointsSlider;

    private int hitPoints;

    public void SetHitPoints(int value)
    {
        hitPoints = value;
    }

    public void TakeHit(int damage)
    {
        hitPoints -= damage;
        hitPointsSlider.value = hitPoints;
    }
}