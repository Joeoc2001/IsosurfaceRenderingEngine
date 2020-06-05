﻿using SFB;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using System;

public class EquationLoader : MonoBehaviour
{
    public const string EXTENSION = "eqdata";

    public EquationHolder holder;

    public void LoadFile()
    {
        string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        ExtensionFilter[] filters = new ExtensionFilter [] { 
            new ExtensionFilter("Equation File", EXTENSION)
        };
        string[] files = StandaloneFileBrowser.OpenFilePanel("Select a file", documentsPath, filters, false);

        if (files.Length != 1)
        {
            return;
        }

        LoadFile(files[0]);
    }

    public void LoadFile(string path)
    {

    }
}
