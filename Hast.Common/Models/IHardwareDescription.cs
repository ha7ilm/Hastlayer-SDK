﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Hast.Layer
{
    /// <summary>
    /// A warning (i.e. an issue that doesn't necessarily make the result wrong but you should know about it) during 
    /// transformation.
    /// </summary>
    public interface ITransformationWarning
    {
        /// <summary>
        /// Gets the code, i.e. the identifier of the warning's type.
        /// </summary>
        string Code { get; }

        /// <summary>
        /// Gets the message describing the issue behind the warning.
        /// </summary>
        string Message { get; }
    }


    /// <summary>
    /// Describes the hardware created from a transformed assembly, i.e. a circuit-level description of the implemented 
    /// logic.
    /// </summary>
    public interface IHardwareDescription
    {
        /// <summary>
        /// The hardware description language used.
        /// </summary>
        string Language { get; }

        /// <summary>
        /// Gets a collection of the full name (including the full namespace of the parent type(s) as well as their 
        /// return type and the types of their - type - arguments of hardware entry members (that are accessible as 
        /// hardware implementation from the host) and their corresponding numerical IDs on the hardware.
        /// </summary>
        IReadOnlyDictionary<string, int> HardwareEntryPointNamesToMemberIdMappings { get; }

        /// <summary>
        /// Writes out the hardware description's source code to the given <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> to write the source code to.</param>
        Task WriteSource(Stream stream);

        /// <summary>
        /// Gets warnings noted during transformation (i.e. issues that don't necessarily make the result wrong but you 
        /// should know about them). 
        /// </summary>
        IEnumerable<ITransformationWarning> Warnings { get; }
    }


    public static class HardwareDescriptionExtensions
    {
        /// <summary>
        /// Writes out the hardware description's source code to a file under the given path.
        /// </summary>
        /// <param name="filePath">The full path where the file should be written to.</param>
        public static Task WriteSource(this IHardwareDescription hardwareDescription, string filePath)
        {
            using (var fileStream = File.Create(filePath))
            {
                return hardwareDescription.WriteSource(fileStream);
            }
        }
    }
}
