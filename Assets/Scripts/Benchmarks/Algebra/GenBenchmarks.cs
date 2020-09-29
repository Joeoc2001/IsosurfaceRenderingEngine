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
    float _avgNumPerSecond = 1000;

    // Working values
    private readonly RandomExpressionGenerator _randomExpressionGenerator = new RandomExpressionGenerator();
    private float _startTime;
    private int _generated;

    private void Start()
    {
        ResetValues();

        baseProbSlider.onValueChanged.AddListener(e => ResetValues());
        maxDepthSlider.onValueChanged.AddListener(e => ResetValues());
    }

    void Update()
    {
        // Set values
        _randomExpressionGenerator.BaseProb = baseProbSlider.value;
        _randomExpressionGenerator.MaxDepth = (int)maxDepthSlider.value;

        // Calculate metrics
        float currentTime = Time.realtimeSinceStartup;
        float numPerSecond = _generated / (currentTime - _startTime);

        float avgNewRatio = 0.9f * Time.deltaTime;

        _avgNumPerSecond *= 1 - avgNewRatio;
        _avgNumPerSecond += numPerSecond * avgNewRatio;

        // Generate hashes
        int toGen = (int)Math.Max(1, _avgNumPerSecond / 60);
        for (int i = 0; i < toGen; i++)
        {
            _randomExpressionGenerator.Next();
        }
        _generated += toGen;

        // Update display
        Display();
    }

    private void ResetValues()
    {
        _generated = 0;
        _startTime = Time.realtimeSinceStartup;
    }

    private void Display()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append($"Average generated per second: {_avgNumPerSecond}\n");
        text.text = builder.ToString();
    }
}
