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
    float avgNumPerSecond = 100;
    float hashFunctionQuality = 0;

    // Working values
    private readonly RandomEquationGenerator randomEquationGenerator = new RandomEquationGenerator();
    private float batchStartTime;
    private int sampled;
    private readonly Dictionary<int, int> seenHashes = new Dictionary<int, int>();

    void Update()
    {
        // Set values
        randomEquationGenerator.baseProb = baseProbSlider.value;
        randomEquationGenerator.maxDepth = (int)maxDepthSlider.value;

        // Calculate metrics
        float currentTime = Time.realtimeSinceStartup;
        float numPerSecond = sampled / (currentTime - batchStartTime);

        float avgNewRatio = 0.9f * Time.deltaTime;

        avgNumPerSecond *= 1 - avgNewRatio;
        avgNumPerSecond += numPerSecond * avgNewRatio;

        // Check if done
        if (sampled >= totalSampledSlider.value)
        {
            ResetMetrics();
        }

        // Generate hashes
        int toGen = (int)Math.Max(1, avgNumPerSecond / 60);
        int max = (int)totalSampledSlider.value - sampled;
        toGen = Math.Min(toGen, max);
        for (int i = 0; i < toGen; i++)
        {
            Equation newEq = randomEquationGenerator.Next();
            int hash = newEq.GetHashCode();

            if (!seenHashes.ContainsKey(hash))
            {
                seenHashes.Add(hash, 0);
            }
            seenHashes[hash] += 1;
        }
        sampled += toGen;

        // Update display
        Display();
    }

    private void ResetMetrics()
    {
        float sum = 0;
        foreach (int hash in seenHashes.Keys)
        {
            int i = seenHashes[hash];
            sum += (i * (i + 1)) / 2;
        }
        float n = sampled;
        float m = (float)int.MaxValue * 2 + 2;
        hashFunctionQuality = sum / ((n / (2 * m)) * (n + 2 * m - 1)); // From red dragon book

        batchStartTime = Time.realtimeSinceStartup;
        sampled = 0;
        seenHashes.Clear();
    }

    private void Display()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append($"Average hashed per second: {avgNumPerSecond}\n");
        builder.Append($"Hash function quality: {hashFunctionQuality}\n");
        builder.Append($"Sampled so far: {sampled}\n");
        text.text = builder.ToString();
    }
}
