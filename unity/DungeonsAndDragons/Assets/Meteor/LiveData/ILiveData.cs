﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Meteor
{
    public interface ILiveData
    {
        Coroutine Connect(string url);
		Method Call (string methodName, params object[] arguments);
		Method<TResponseType> Call<TResponseType> (string methodName, params object[] arguments);
		Subscription Subscribe (string publishName, params object[] arguments);
        int GetCurrentRequestId();
    }
}
