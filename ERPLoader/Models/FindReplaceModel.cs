using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ERPLoader.Models
{
    public class FindReplaceModel
    {
        public enum SearchTypeEnum
        {
            Exact, Regex
        }

        public class FindReplaceTask
        {
            public SearchTypeEnum SearchType { get; set; } = SearchTypeEnum.Exact;
            public string SearchFor { get; set; } = "";
            public string ReplaceWith { get; set; } = "";

            public FindReplaceTask Clone()
            {
                return new FindReplaceTask
                {
                    SearchType = SearchType,
                    SearchFor = SearchFor,
                    ReplaceWith = ReplaceWith
                };
            }
        }

        public class FileTask
        {
            public string FileName { get; set; } = "";
            public List<FindReplaceTask> Tasks { get; set; }
        }

        public string ErpFilePath { get; set; }
        public List<FileTask> Tasks { get; set; }

        public static IList<FindReplaceModel> FromJson(string jsonString)
        {
            var options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }
            };

            return JsonSerializer.Deserialize<IList<FindReplaceModel>>(jsonString, options);
        }

        public static string ToJson(IList<FindReplaceModel> models)
        {
            return JsonSerializer.Serialize(models, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
