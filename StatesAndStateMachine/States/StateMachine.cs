using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace HomeworkTwo
{
    public class StateMachine<T>
    {
        #region Member Variables
        private T owner;
        private State<T> currentState;
        private State<T> globalState;
        private State<T> previousState;
        #endregion

        #region Constructor
        public StateMachine(T t)
        {
            owner = t;
            currentState = null;
            globalState = null;
            previousState = null;
        }
        #endregion

        #region Public Methods

        public void SetCurrentState(State<T> state) { currentState = state; }
        public void SetGlobalState(State<T> state) { globalState = state; }
        public void SetPreviousState(State<T> state) { previousState = state; }

        /// <summary>
        /// Update the FSM
        /// </summary>
        public void Update()
        {
            // If the global state exists, execute it
            if (globalState != null)
                globalState.Execute(owner);

            // If the current state exists, execute it
            if (currentState != null)
                currentState.Execute(owner);
        }

        public bool HandleMessage(Telegram msg)
        {
            if (currentState != null && currentState.OnMessage(owner, msg))
                return true;

            if (globalState != null && globalState.OnMessage(owner, msg))
                return true;

            return false;
        }

        public void ChangeState(State<T> newState)
        {
            Debug.Assert(newState != null);

            previousState = currentState;
            currentState.Exit(owner);

            currentState = newState;
            currentState.Enter(owner);
        }

        public void RevertToPreviousState()
        {
            ChangeState(previousState);
        }

        public bool IsInState(State<T> state)
        {
            return state.GetType() == currentState.GetType();
        }

        #if DEBUG
        public string GetNameOfCurrentState() { return currentState.ToString(); }
        #endif

        #endregion
    }
}
