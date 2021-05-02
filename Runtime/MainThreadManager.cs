using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UDPNetwork
{
    public class MainThreadManager : MonoBehaviour, ObjectStateHandler
    {
        private static readonly List<Action> executeOnMainThread = new List<Action>();
        private static readonly List<Action> executeCopiedOnMainThread = new List<Action>();
        private static bool actionToExecuteOnMainThread = false;
        private ObjectState state;
        private bool isReady = true;

        private void Setup()
        {
            actionToExecuteOnMainThread = false;
            executeCopiedOnMainThread.Clear();
            executeOnMainThread.Clear();
        }

        private void OnEnable()
        {
            if (isReady)
            {
                Setup();
                isReady = false;
            }
        }

        private void Start()
        {

            if (state == null)
            {
                state = new ObjectState(new List<object>() { actionToExecuteOnMainThread, executeCopiedOnMainThread });
            }
            else
            {
                state.AddVariables(new List<object>() { actionToExecuteOnMainThread, executeCopiedOnMainThread });
            }

        }

        private void Update()
        {
            UpdateMain();
        }

        /// <summary>Sets an action to be executed on the main thread.</summary>
        /// <param name="_action">The action to be executed on the main thread.</param>
        public static void ExecuteOnMainThread(Action _action)
        {
            if (_action == null)
            {
                Debug.Log("No action to execute on main thread!");
                return;
            }

            lock (executeOnMainThread)
            {
                executeOnMainThread.Add(_action);
                actionToExecuteOnMainThread = true;
            }
        }

        /// <summary>Executes all code meant to run on the main thread. NOTE: Call this ONLY from the main thread.</summary>
        public static void UpdateMain()
        {
            if (actionToExecuteOnMainThread)
            {
                executeCopiedOnMainThread.Clear();
                lock (executeOnMainThread)
                {
                    executeCopiedOnMainThread.AddRange(executeOnMainThread);
                    executeOnMainThread.Clear();
                    actionToExecuteOnMainThread = false;
                }

                for (int i = 0; i < executeCopiedOnMainThread.Count; i++)
                {
                    executeCopiedOnMainThread[i]();
                }
            }
        }

        public void SetState(ObjectState state)
        {
            this.state = state;
        }

        public void RestoreToState()
        {
            if (state != null && state.StateIsSaved())
            {
                List<object> variables = state.GetVariables();
                actionToExecuteOnMainThread = (bool)variables[0];
                Setup();
            }
        }

        public object DeepCopy(object copying)
        {
            throw new NotImplementedException();
        }
    }
}