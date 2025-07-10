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

public class JsonSave
{
    private JsonSerializerSettings _settings = new()
    {
        Formatting = Formatting.Indented,
        ContractResolver = new PrivateMembersCResolver()
    };

    public string DefaultFileExtension { get; set; } = ".json";

    private Dictionary<string, List<object>> _objectToSave = new Dictionary<string, List<object>>();

    private void SafeKey(string key)
    {
	    if (!_objectToSave.ContainsKey(key))
		    _objectToSave.Add(key, new List<object>());
    }
    
    public void AddObjectToSave(object obj, string path)
    {
	    SafeKey(path);
        _objectToSave[path].Add(obj);
    }

    private void SavePath(string path)
    {
	    SafeKey(path);
	    string json = "";
	    
	    json += JsonConvert.SerializeObject(_objectToSave[path], _settings);
	    
	    if (!Path.HasExtension(path))
		    path += ".json";
	    
	    
	    File.WriteAllText(path, json);
	    Console.WriteLine($"Saved the {path} file");
    }
    public void Save(string? path = null)
    {
	    if (path == null)
		    foreach (var obj in _objectToSave)
			    SavePath(obj.Key);

	    else
	    {
		    SafeKey(path);
		    SavePath(path);
	    }
    }
}
