using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace JsonData;


/// <summary>
/// A contract resolver that resolve private members.
/// </summary>
public class PrivateMembersCResolver : DefaultContractResolver
{
	protected override List<MemberInfo> GetSerializableMembers(Type objectType)
	{
		var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		return objectType.GetMembers(flags)
			.Where(m => m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property)
			.ToList();
	}
}

/// <summary>
/// The class containing all the methods to save things into a file.
/// </summary>
public static class JsonSave
{
    private static JsonSerializerSettings _settings = new()
    {
        Formatting = Formatting.Indented,
        ContractResolver = new PrivateMembersCResolver()
    };

    /// <summary>
    /// Set the default extension for files that doesn't have one yet.
    /// </summary>
    public static string DefaultFileExtension { get; set; } = ".json";

    /// <summary>
    /// A dictionary containing the objects to save, each path have a list of object associated.
    /// </summary>
    private static Dictionary<string, List<object>> _objectToSave = new Dictionary<string, List<object>>();

    /// <summary>
    /// Check if a key exist in _objectToSave, and create the pair of this key if it doesn't exist.
    /// </summary>
    /// <param name="key">The key to check</param>
    private static void SafeKey(string key)
    {
	    if (!_objectToSave.ContainsKey(key))
		    _objectToSave.Add(key, new List<object>());
    }
    
    /// <summary>
    /// Add an object to save.
    /// </summary>
    /// <param name="obj">The object to save</param>
    /// <param name="path">The destination</param>
    /// <remarks>The object is only saved when JsonSave.Save() is called with no parameter or with the corresponding path</remarks>
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
    
    /// <summary>
    /// Save the objects associated with a path
    /// </summary>
    /// <param name="path">The path associated with the objects you aim to save</param>
    /// <remarks>If the path is null, all the objects of all the paths will be saved.</remarks>
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
	
	/// <summary>
	/// Load a list of object from a json formated file.
	/// </summary>
	/// <param name="path">The source file</param>
	/// <typeparam name="T">The type that de deserializer need to reconstitute</typeparam>
	/// <returns>A List of T type object if the loading was successful. Or default/null if the file doesn't exist or the deserializer failed to reconstitute the T type</returns>
	public static List<T>? Load<T>(string path)
	{
		if (!File.Exists(path))
			return default;
		
		string json = File.ReadAllText(path);
		return JsonConvert.DeserializeObject<List<T>>(json, _settings);
	}

	/// <summary>
	/// Load an object from a json formated file.
	/// </summary>
	/// <param name="path">The source file</param>
	/// <param name="index">The index of the object in the file</param>
	/// <typeparam name="T">The type that de deserializer need to reconstitute</typeparam>
	/// <returns>A T type object if the loading was successful. Or default/null if the file doesn't exist or the deserializer failed to reconstitute the T type</returns>
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