using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyntaxException : Exception
{
    public SyntaxException(string message) : base(message)
    {
    }
}
