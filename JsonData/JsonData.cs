using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace JsonData;


public class PrivateMembersCResolver : DefaultContractResolver
{
	protected override List<MemberInfo> GetSerializableMembers(Type objectType)
	{
		// Récupère tous les champs et propriétés publics/privés d'instance
		var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		return objectType.GetMembers(flags)
			.Where(m => m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property)
			.ToList();
	}
}

public static class JsonData
{
    private static JsonSerializerSettings _settings = new()
    {
        Formatting = Formatting.Indented,
        ContractResolver = new PrivateMembersCResolver()
    };

    public static string DefaultFileExtension { get; set; } = ".json";

    private static Dictionary<string, List<object>> _objectToSave = new Dictionary<string, List<object>>();

    private static void SafeKey(string key)
    {
	    if (!_objectToSave.ContainsKey(key))
		    _objectToSave.Add(key, new List<object>());
    }
    
    public static void AddObjectToSave(object obj, string path)
    {
	    SafeKey(path);
        _objectToSave[path].Add(obj);
    }

    private static void SavePath(string path)
    {
	    SafeKey(path);
	    string json = "";
	    
	    json += JsonConvert.SerializeObject(_objectToSave[path], _settings);
	    
	    if (!Path.HasExtension(path))
		    path += ".json";
	    
	    
	    File.WriteAllText(path, json);
	    Console.WriteLine($"Saved the {path} file");
    }
    public static void Save(string? path = null)
    {
	    if (path == null)
		    foreach (var obj in _objectToSave)
			    SavePath(obj.Key);

	    else
	    {
		    SafeKey(path);
		    SavePath(path);
	    }
	    _objectToSave.Clear();
    }
}


public static class JsonLoad
{
	private static JsonSerializerSettings _settings = new()
	{
		Formatting = Formatting.Indented,
		ContractResolver = new PrivateMembersCResolver()
	};
	
	public static List<T>? Load<T>(string path)
	{
		if (!File.Exists(path))
			return default;
		
		string json = File.ReadAllText(path);
		return JsonConvert.DeserializeObject<List<T>>(json, _settings);
	}

	public static T? Load<T>(string path, int index)
	{
		if (!File.Exists(path))
			return default;
		
		string json = File.ReadAllText(path);
		List<T>? list = JsonConvert.DeserializeObject<List<T>>(json, _settings);

;
		if (list == null || index >= list.Count)
			return default;
		
		return list[index];
	}
}