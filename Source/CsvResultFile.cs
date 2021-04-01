using System.Globalization;
using System.IO;

using CsvHelper;

namespace ActLikeAI.Benchmark
{
    internal class CsvResultFile
    {
        public string FileName { get; init; }


        public CsvResultFile(string fileName)
        {
            FileName = fileName;
        }


        public ResultCollection? Load()
        {
            if (File.Exists(FileName))
            {
                using StreamReader stream = File.OpenText(FileName);
                using CsvReader csv = new(stream, CultureInfo.InvariantCulture); 
                var results = csv.GetRecords<Result>();

                return new ResultCollection(results);
            }
            else
                return null;            
        }


        public void Save(ResultCollection results)
        {
            using StreamWriter stream = File.CreateText(FileName);
            using CsvWriter csv = new(stream, CultureInfo.InvariantCulture);

            csv.WriteRecords(results);
        }
    }
}
