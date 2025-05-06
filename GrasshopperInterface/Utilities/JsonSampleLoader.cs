using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Utilities;

using Newtonsoft.Json;

namespace GrasshopperInterface.Utilities
{
    public class JsonSampleLoader
    {
        /// <summary>
        /// Loads all JSON files from the specified directory and returns them as a list of nested dictionaries.
        /// Each dictionary represents a JSON object, preserving any nested structures (e.g., "params", "objectives").
        /// </summary>
        /// <param name="directoryPath">Directory containing JSON files.</param>
        /// <returns>List of nested dictionaries parsed from the JSON files.</returns>
        public static List<Dictionary<string, object>> LoadJsonSamples(string directoryPath)
        {
            var samples = new List<Dictionary<string, object>>();

            // Check if directory exists
            if (!Directory.Exists(directoryPath))
                throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");

            // Get all JSON files
            var jsonFiles = Directory.GetFiles(directoryPath, "*.json");
            if (jsonFiles.Length == 0)
                throw new FileNotFoundException($"No JSON files found in directory: {directoryPath}");

            // Process each file
            foreach (var file in jsonFiles)
            {
                try
                {
                    // Read JSON file content
                    string jsonContent = File.ReadAllText(file);

                    // Check for empty content
                    if (string.IsNullOrWhiteSpace(jsonContent))
                    {
                        Console.WriteLine($"Warning: Empty JSON content in file {file}");
                        continue;
                    }

                    // Deserialize into a list of dictionaries
                    var data = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonContent);

                    // Add the data to the samples list (if successful)
                    if (data != null)
                    {
                        samples.AddRange(data);  // Add all objects in the array
                    }
                    else
                    {
                        Console.WriteLine($"Warning: Failed to deserialize JSON content from file {file}");
                    }
                }
                catch (Exception ex)
                {
                    // Detailed logging for the exception
                    Console.WriteLine($"Error reading file {file}: {ex.Message}");
                    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                }
            }

            return samples;
        }
    }
}
