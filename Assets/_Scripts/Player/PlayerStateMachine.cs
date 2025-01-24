namespace _Scripts.PlayerState

{
    public class PlayerStateMachine
    {
        public PlayerState CurrentState {get; private set;}
    
        //Function that is called at the start of the game
        public void Initialize(PlayerState startingState)
        {
            CurrentState = startingState;
            CurrentState.Enter();
        }
    
        //Function that is called whenever you change a state
        public void ChangeState(PlayerState newState)
        {
            CurrentState.Exit();
            CurrentState = newState;
            CurrentState.Enter();
        }
    }
}
