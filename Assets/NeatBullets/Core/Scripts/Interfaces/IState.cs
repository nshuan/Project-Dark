namespace NeatBullets.Core.Scripts.Interfaces {
    
    public interface IState {
  
        public void Enter();

        public void Update();
    
        public void FixedUpdate();
    
        public void LateUpdate();
    
        public void Exit();

    }
}
