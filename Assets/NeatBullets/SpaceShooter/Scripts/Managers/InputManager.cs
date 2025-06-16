using UnityEngine;
using UnityEngine.InputSystem;

namespace NeatBullets.SpaceShooter.Scripts.Managers {
    
    // Hub for easy access to InputActions
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance;
    
        public PlayerInput Input;
        public InputAction PauseAction;
    
        public (InputAction moveAction, InputAction aimAction) PlayerActionMap;
        private InputAction _moveAction;
        private InputAction _aimAction;
    
        
        private void Awake()
        {
            if (Instance == null) {
                Instance = this;
            }
            else {
                Destroy(gameObject);
            }
        
            Input = GetComponent<PlayerInput>();
            Input.actions.FindActionMap("UI").Enable();
            Input.actions.FindActionMap("Player").Enable();

            _moveAction = Input.actions.FindAction("Move");
            _aimAction = Input.actions.FindAction("Aim");
            PauseAction = Input.actions.FindAction("Pause");

            PlayerActionMap = (_moveAction, _aimAction);
        }
    }
}
