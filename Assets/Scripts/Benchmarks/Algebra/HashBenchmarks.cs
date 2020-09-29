using Algebra;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class HashBenchmarks : MonoBehaviour
{
    // Input
    public Slider totalSampledSlider;
    public Slider baseProbSlider;
    public Slider maxDepthSlider;

    // Output
    public TMPro.TMP_Text text;

    // Displayed values
    float _avgNumPerSecond = 1000;
    float _hashFunctionQuality = 0;

    // Working values
    private readonly RandomExpressionGenerator _randomExpressionGenerator = new RandomExpressionGenerator();
    private float _batchStartTime;
    private int _sampled;
    private readonly Dictionary<int, HashSet<Expression>> _seenHashes = new Dictionary<int, HashSet<Expression>>();

    void Update()
    {
        // Set values
        _randomExpressionGenerator.BaseProb = baseProbSlider.value;
        _randomExpressionGenerator.MaxDepth = (int)maxDepthSlider.value;

        // Calculate metrics
        float currentTime = Time.realtimeSinceStartup;
        float numPerSecond = _sampled / (currentTime - _batchStartTime);

        float avgNewRatio = 0.9f * Time.deltaTime;

        _avgNumPerSecond *= 1 - avgNewRatio;
        _avgNumPerSecond += numPerSecond * avgNewRatio;

        // Check if done
        if (_sampled >= totalSampledSlider.value)
        {
            ResetMetrics();
        }

        // Generate hashes
        int toGen = (int)Math.Max(1, _avgNumPerSecond / 60);
        int max = (int)totalSampledSlider.value - _sampled;
        toGen = Math.Min(toGen, max);
        for (int i = 0; i < toGen; i++)
        {
            Expression newEq = _randomExpressionGenerator.Next();
            int hash = newEq.GetHashCode();

            if (!_seenHashes.ContainsKey(hash))
            {
                _seenHashes.Add(hash, new HashSet<Expression>());
            }
            _seenHashes[hash].Add(newEq);
        }
        _sampled += toGen;

        // Update display
        Display();
    }

    private void ResetMetrics()
    {
        float sum = 0;
        foreach (int hash in _seenHashes.Keys)
        {
            int i = _seenHashes[hash].Count;
            sum += (i * (i + 1)) / 2;
        }
        float n = _sampled;
        float m = (float)int.MaxValue * 2 + 2;
        _hashFunctionQuality = sum / ((n / (2 * m)) * (n + 2 * m - 1)); // From red dragon book

        _batchStartTime = Time.realtimeSinceStartup;
        _sampled = 0;
        _seenHashes.Clear();
    }

    private void Display()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append($"Average hashed per second: {_avgNumPerSecond}\n");
        builder.Append($"Hash function quality: {_hashFunctionQuality}\n");
        builder.Append($"Sampled so far: {_sampled}\n");
        text.text = builder.ToString();
    }
}
