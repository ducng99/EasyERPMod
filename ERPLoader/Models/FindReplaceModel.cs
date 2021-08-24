using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ERPLoader.Models
{
    class FindReplaceModel
    {
        public enum SearchTypeEnum
        {
            Exact, Regex
        }

        public struct FindReplaceTask
        {
            public SearchTypeEnum SearchType { get; set; }
            public string SearchFor { get; set; }
            public string ReplaceWith { get; set; }
        }

        public struct FileTask
        {
            public string FileName { get; set; }
            public IList<FindReplaceTask> Tasks { get; set; }
        }

        public string ErpFilePath { get; set; }
        public IList<FileTask> Tasks { get; set; }

        public static IList<FindReplaceModel> FromJson(string jsonString)
        {
            var options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }
            };

            return JsonSerializer.Deserialize<IList<FindReplaceModel>>(jsonString, options);
        }
    }
}
