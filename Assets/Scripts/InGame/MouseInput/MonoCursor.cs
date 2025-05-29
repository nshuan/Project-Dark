using UnityEngine;
using UnityEngine.UI;

namespace InGame.MouseInput
{
    public class MonoCursor : MonoBehaviour
    {
        [SerializeField] private Image cooldown;

        public Image UICooldown => cooldown;
    }
}