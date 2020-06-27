using Algebra;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GenBenchmarks : MonoBehaviour
{
    // Input
    public Slider baseProbSlider;
    public Slider maxDepthSlider;

    // Output
    public TMPro.TMP_Text text;

    // Displayed values
    float avgNumPerSecond = 1000;

    // Working values
    private readonly RandomEquationGenerator randomEquationGenerator = new RandomEquationGenerator();
    private float startTime;
    private int generated;

    private void Start()
    {
        ResetValues();

        baseProbSlider.onValueChanged.AddListener(e => ResetValues());
        maxDepthSlider.onValueChanged.AddListener(e => ResetValues());
    }

    void Update()
    {
        // Set values
        randomEquationGenerator.baseProb = baseProbSlider.value;
        randomEquationGenerator.maxDepth = (int)maxDepthSlider.value;

        // Calculate metrics
        float currentTime = Time.realtimeSinceStartup;
        float numPerSecond = generated / (currentTime - startTime);

        float avgNewRatio = 0.9f * Time.deltaTime;

        avgNumPerSecond *= 1 - avgNewRatio;
        avgNumPerSecond += numPerSecond * avgNewRatio;

        // Generate hashes
        int toGen = (int)Math.Max(1, avgNumPerSecond / 60);
        for (int i = 0; i < toGen; i++)
        {
            randomEquationGenerator.Next();
        }
        generated += toGen;

        // Update display
        Display();
    }

    private void ResetValues()
    {
        generated = 0;
        startTime = Time.realtimeSinceStartup;
    }

    private void Display()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append($"Average generated per second: {avgNumPerSecond}\n");
        text.text = builder.ToString();
    }
}
