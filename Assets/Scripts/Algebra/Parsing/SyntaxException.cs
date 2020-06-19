using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Algebra.Parsing
{
    public class SyntaxException : Exception
    {
        public SyntaxException(string message) : base(message)
        {
        }
    }
}