using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;

//[BurstCompile]
public struct FeelerNodeSetJob : IJob
{
    public FunctionPointer<Equation.ExpressionDelegate> Function;

    public int Resolution;
    public float Delta;
    public float3 Origin;

    [WriteOnly]
    public NativeArray<FeelerNode> Target;

    public void Execute()
    {
        Equation.ExpressionDelegate expression = Function.Invoke;
        for (int x = 0; x < Resolution; x++)
        {
            for (int y = 0; y < Resolution; y++)
            {
                for (int z = 0; z < Resolution; z++)
                {
                    // Construct position
                    float3 position = Origin + (Delta * new float3(x, y, z));

                    // Get value
                    VariableSet variableSet = new VariableSet(position);
                    float value = expression(variableSet);

                    // Place in target array
                    int index = (x * Resolution + y) * Resolution + z;
                    Target[index] = new FeelerNode(position, value);
                }
            }
        }
    }
}
