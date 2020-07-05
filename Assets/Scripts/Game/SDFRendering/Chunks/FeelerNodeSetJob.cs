using Algebra;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

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
        //Equation.ExpressionDelegate expression = Function.Invoke;
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
                    float value = Function.Invoke(variableSet);

                    // Guard for invalid values
                    if (float.IsNaN(value))
                    {
                        value = 0;
                    }

                    // Place in target array
                    int index = (x * Resolution + y) * Resolution + z;
                    Target[index] = new FeelerNode(position, value);
                }
            }
        }
    }
}
