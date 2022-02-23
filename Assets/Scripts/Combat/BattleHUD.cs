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

        private int _maxHp;
    
        public void SetHUD(Unit unit)
        {
            _maxHp = unit.maxHp;
            nameText.text = unit.unitName;
            hpSlider.maxValue = unit.maxHp;
            hpSlider.value = unit.currentHp;
            hpText.text = unit.currentHp + "/" + unit.maxHp;
        }

        public void SetHP(int hp)
        {
            hpSlider.value = hp;
            hpText.text = hp + "/" + _maxHp;
        }
   
    }
}
