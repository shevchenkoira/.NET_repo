using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IraTask_1;

public class Collection<T> : IEnumerable where T:new()
{
    public List<Dictionary<string, object>> ObjCollection { get; set; }

    private readonly string _path;

    private readonly FileManager _fileManager;
    public Collection(string path)
    {
        _path = path;
        _fileManager = new FileManager(_path);
        ObjCollection = new List<Dictionary<string, Object>>();
        
        var tempList = _fileManager.DeSerialize();
        foreach (var curr in tempList)
        {
            try
            {
                ConvertDictionaryTo<T>(curr);
                ObjCollection.Add(curr);
            }
            catch (Exception e)
            {
                string errorMessage;
                if (e.InnerException is null)
                {
                    errorMessage = e.Message;
                }
                else
                {
                    errorMessage = e.InnerException.Message;
                }
                Console.WriteLine(errorMessage);
            }
        }

        ObjCollection = tempList;
    }

    public void Add(T objectToAdd)
    {
        ObjCollection.Add(this.ToDict(objectToAdd));
        _fileManager.Serialize(this.ObjCollection);
    }

    
    public T GetById(int id)
    {
        foreach (Dictionary<string, object> currentElement in this.ObjCollection)
        {
            try
            {
                if (Convert.ToInt32(currentElement["id"].ToString()) == id)
                {
                    return ConvertDictionaryTo<T>(currentElement);
                }
            }
            catch (KeyNotFoundException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        throw new Exception("Element with such id doesn't exist");
    }

    public void RemoveById(int id)
    {
        for(int i = 0; i < this.ObjCollection.Count; i++)
        {
            if (Convert.ToInt32(this.ObjCollection[i]["id"].ToString()) == id)
            {
                ObjCollection.Remove(ObjCollection[i]);
                _fileManager.Serialize(ObjCollection);
                break;
            }
        }
    }

    public void Edit(int id, string field, object? change)
    {
        bool isNumeric = int.TryParse(change.ToString(), out _);

        if (typeof(T).GetProperty(field).GetType().Name == "int" && isNumeric)
        {
            change = int.Parse(change.ToString());
        }
        
        Dictionary<string, object> objToEdit = this.ToDict(this.GetById(id));
        objToEdit[field] = change ?? throw new Exception("Error, you entered nothing to change");
        try
        {
            ConvertDictionaryTo<T>(objToEdit);
        }
        catch (Exception e)
        {
            throw new ValidationException(e.Message);
        }
        RemoveById(id);
        Add(ConvertDictionaryTo<T>(objToEdit));
    }

    public void Sort(string field = "id", string order = "asc")
    {
        bool flag = false;
        foreach (var curr in typeof(T).GetProperties())
        {
            if (field == curr.Name)
            {
                flag = true;
            }
        }

        if (!flag)
        {
            throw new ArgumentException("Field is not present in this class");
        }
        
        List<T> a = new List<T>();

        foreach (Dictionary<string, object> curr in ObjCollection)
        {
            a.Add(ConvertDictionaryTo<T>(curr));
        }

        var query = from obj in a
            orderby typeof(T).GetProperty(field).GetValue(obj)
            select obj;
        
        if (order == "desc")
        {
            query = from obj in a
                orderby typeof(T).GetProperty(field).GetValue(obj) descending 
                select obj;
        }

        a = query.ToList<T>();
        
        ObjCollection.Clear();
        foreach (var cur in a)
        {
            ObjCollection.Add(ToDict(cur));
        }


        foreach (Dictionary<string, object> curr in ObjCollection)
        {
            Console.WriteLine(JsonSerializer.Serialize(curr, new JsonSerializerOptions(){WriteIndented = true}));
        }
        _fileManager.Serialize(ObjCollection);
    }


    public void SearchBy(string searchQuery)
    {
        if (searchQuery is null)
        {
            throw new ArgumentException("Search query must be specified");
        }
        var quer = ObjCollection.Where(x => x.Values.Any(x => x.ToString().Contains(searchQuery)));

        foreach (var cur in quer)
        {
            Console.WriteLine(JsonSerializer.Serialize(cur, new JsonSerializerOptions(){WriteIndented = true}));
        }
        
        
    }

    public Dictionary<string, object> ToDict(T obj)
    {
        var json = JsonSerializer.Serialize(obj);
        var dictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        return dictionary;
    }
    
    public T this[int i]
    {
        get { return ConvertDictionaryTo<T>(this.ObjCollection[i]); }
        set { ObjCollection[i] = ToDict(value); }
    }
    
    public IEnumerator GetEnumerator()
    {
        return this.ObjCollection.GetEnumerator();
    }

    private T ConvertDictionaryTo<T>(IDictionary<string, object> dictionary) where T : new()
    {
        Type type = typeof (T);
        T ret = new T();

        StringBuilder errorCollection = new StringBuilder();

        foreach (var keyValue in dictionary)
        {
            try
            {
                type.GetProperty(keyValue.Key)
                    .SetValue(ret, Convert.ChangeType(keyValue.Value.ToString(),
                        typeof(T).GetProperty(keyValue.Key).PropertyType));
            }
            catch (Exception e)
            {
                string errorMessage;
                if (e.InnerException is null)
                {
                    errorMessage = e.Message;
                }
                else
                {
                    errorMessage = e.InnerException.Message;
                }
                Console.WriteLine(errorMessage);
                errorCollection.Append(" ");
            }
        }

        if (errorCollection.ToString() != "")
        {
            throw new ValidationException("");
        }
        
        return ret;
    }
}

public class FileManager
{
    private readonly JsonSerializerOptions _options;
    public string Path { get; set; }
    
    public FileManager(string path)
    {
        this._options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {WriteIndented = true};
        this.Path = path;
    }
    
    public void Serialize(List<Dictionary<string, object>> objCollection)
    {
        if (objCollection.Count == 0)
        {
            using (StreamWriter sw = new StreamWriter(Path))
            {
                sw.WriteLine("");
            }
        }
        using (StreamWriter sw = new StreamWriter(Path))
        {
            sw.WriteLine(JsonSerializer.Serialize(objCollection, _options));
        }
    }

    public List<Dictionary<string, object>> DeSerialize()
    {
        string json = File.ReadAllText(Path);
        if (json.Length == 0)
        {
            throw new Exception("Nothing to deserialize");
        }
        List<Dictionary<string, object>> objectsDict =
            JsonSerializer.Deserialize<List<Dictionary<string, object>>>(json, this._options);

        return objectsDict;
    }
}
