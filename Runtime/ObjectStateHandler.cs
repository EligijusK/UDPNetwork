using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace UDPNetwork
{
    public interface ObjectStateHandler
    {
        public void SetState(ObjectState state);

        public void RestoreToState();

        public object DeepCopy(object copying);

    }
}