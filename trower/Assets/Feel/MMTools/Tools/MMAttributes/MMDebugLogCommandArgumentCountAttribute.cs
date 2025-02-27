﻿using System;

namespace MoreMountains.Tools
{
    /// <summary>
    /// An attribute to add to static methods to they can be called via the MMDebugMenu's command line
    /// </summary>
    [AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false)]
    public class MMDebugLogCommandArgumentCountAttribute : System.Attribute
    {
        public readonly int ArgumentCount;

        public MMDebugLogCommandArgumentCountAttribute(int argumentCount)
        {
            this.ArgumentCount = argumentCount;
        }
    }
}