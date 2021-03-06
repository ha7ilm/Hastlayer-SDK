﻿using System.Threading.Tasks;
using Hast.Transformer.Abstractions.SimpleMemory;

namespace Hast.Samples.SampleAssembly
{
    /// <summary>
    /// A massively parallel algorithm that is well suited to be accelerated with Hastlayer.
    /// </summary>
    /// <remarks>
    /// This is the same code as in Hast.Samples.Demo (https://github.com/Lombiq/Hastlayer-Demo) but also added here for
    /// convenience.
    /// </remarks>
    public class ParallelAlgorithm
    {
        public const int Run_InputUInt32Index = 0;
        public const int Run_OutputUInt32Index = 0;

        public const int MaxDegreeOfParallelism = 280;


        public virtual void Run(SimpleMemory memory)
        {
            var input = memory.ReadUInt32(Run_InputUInt32Index);
            var tasks = new Task<uint>[MaxDegreeOfParallelism];

            for (uint i = 0; i < MaxDegreeOfParallelism; i++)
            {
                tasks[i] = Task.Factory.StartNew(
                    indexObject =>
                    {
                        var index = (uint)indexObject;
                        uint result = input + index * 2;

                        var even = true;
                        for (int j = 2; j < 9999999; j++)
                        {
                            if (even)
                            {
                                result += index;
                            }
                            else
                            {
                                result -= index;
                            }

                            even = !even;
                        }

                        return result;
                    },
                    i);
            }

            // Task.WhenAny() can be used too.
            Task.WhenAll(tasks).Wait();

            uint output = 0;
            for (int i = 0; i < MaxDegreeOfParallelism; i++)
            {
                output += tasks[i].Result;
            }

            memory.WriteUInt32(Run_OutputUInt32Index, output);
        }
    }


    public static class ParallelAlgorithmExtensions
    {
        public static uint Run(this ParallelAlgorithm algorithm, uint input)
        {
            var memory = new SimpleMemory(1);
            memory.WriteUInt32(ParallelAlgorithm.Run_InputUInt32Index, input);
            algorithm.Run(memory);
            return memory.ReadUInt32(ParallelAlgorithm.Run_OutputUInt32Index);
        }
    }
}
