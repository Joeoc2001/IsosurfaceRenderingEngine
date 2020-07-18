using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using Algebra;
using System.Runtime.InteropServices;

public class VoxelPriorGenTask : PriorGenTask
{
    private readonly FeelerNodeSetJob job;
    private readonly VoxelChunk chunk;
    private readonly SDF sdf;

    public VoxelPriorGenTask(VoxelChunk chunk, SDF sdf)
    {
        int resolution = (1 << chunk.Quality) + 2; // Overscan by 2 so we can gen edges properly
        float delta = Chunk.SIZE / (1 << chunk.Quality);
        Vector3 offset = new Vector3(delta / 2, delta / 2, delta / 2);
        job = new FeelerNodeSetJob()
        {
            // Get a pointer to the distance function
            Function = new FunctionPointer<Equation.ExpressionDelegate>(Marshal.GetFunctionPointerForDelegate(sdf.Dist)),

            Resolution = resolution,
            Delta = delta,
            Origin = -offset,
            SamplingOffset = chunk.transform.position,

            Target = new NativeArray<FeelerNode>(resolution * resolution * resolution, Allocator.TempJob)
        };

        this.chunk = chunk;
        this.sdf = sdf;
    }

    public override IPriorGenTaskHandle Schedule()
    {
        JobHandle handle = job.Schedule();
        return new PriorGenTaskJobHandle(handle);
    }

    public override void AfterFinished()
    {
        // Extract completed datas
        FeelerNode[] feelerNodes = new FeelerNode[job.Target.Length];
        job.Target.CopyTo(feelerNodes);
        int resolution = job.Resolution;
        FeelerNodeSet feelerNodeSet = new FeelerNodeSet(resolution, feelerNodes);

        // Clean up
        job.Target.Dispose();

        // Update chunk's mesh
        chunk.GenerateMesh(feelerNodeSet, sdf.Grad);
    }
}
