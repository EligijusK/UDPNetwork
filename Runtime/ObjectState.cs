using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDPNetwork
{

    [System.Serializable]
    public class ObjectState
    {
        private List<object> variables;
        private bool stateIsSaved = false;

        public ObjectState(List<object> arguments)
        {
            variables = new List<object>();
            variables.AddRange(arguments);
            stateIsSaved = true;
        }

        public ObjectState()
        {
            variables = new List<object>();
            stateIsSaved = false;
        }

        public void SetVariables(List<object> arguments)
        {
            variables = new List<object>();
            variables.AddRange(arguments);
            stateIsSaved = true;
        }

        public void AddVariables(List<object> arguments)
        {
            variables.AddRange(arguments);
            stateIsSaved = true;
        }

        public bool StateIsSaved()
        {
            return stateIsSaved;
        }

        public List<object> GetVariables()
        {
            return variables;
        }

    }
}