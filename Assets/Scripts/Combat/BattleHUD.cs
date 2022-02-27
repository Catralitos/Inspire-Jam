using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Combat
{
    public class BattleHUD : MonoBehaviour
    {
        public TextMeshProUGUI nameText;
        public Slider hpSlider;
        public TextMeshProUGUI hpText;
        public TextMeshProUGUI typeText;

        
        private int _maxHp;
    
        public void SetHUD(Unit unit)
        {
            nameText.text = unit.unitName;
            SetHP(unit);
            SetType(unit);
        }

        private void SetHP(Unit unit)
        {
            hpSlider.maxValue = unit.maxHp;
            hpSlider.value = unit.currentHp;
            hpText.text = unit.currentHp + "/" + unit.maxHp;
        }

        private void SetType(Unit unit)
        {                    
            typeText.text = unit.currentType.ToString();
            switch (unit.currentType)
            {
                case Unit.Type.Neutral:
                    typeText.color = Color.grey;
                    break;
                case Unit.Type.Meat:
                    typeText.color = Color.red;
                    break;
                case Unit.Type.Vegetables:
                    typeText.color = Color.green;
                    break;
                case Unit.Type.Fish:
                    typeText.color = Color.blue;
                    break;
                default:
                    break;
            }
        }
   
    }
}
