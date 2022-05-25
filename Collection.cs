using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IraTask_1;

public class Collection<T> : IEnumerable where T:new()
{
    public List<T> ObjCollection { get; set; }

    private readonly string _path;

    private readonly FileManager<T> _fileManager;
    public Collection(string path)
    {
        _path = path;
        _fileManager = new FileManager<T>(_path);
        ObjCollection = new List<T>();
        
        var tempList = _fileManager.DeSerialize();
        foreach (var curr in tempList)
        {
            try
            {
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
        ObjCollection.Add(objectToAdd);
        _fileManager.Serialize(this.ObjCollection);
    }

    public T GetById(int id)
    {
        foreach (var currentElement in this.ObjCollection)
        {
            try
            {
                if (Convert.ToInt32(typeof(T).GetProperty("id").GetValue(currentElement)) == id)
                {
                    return currentElement;
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
            if (Convert.ToInt32(typeof(T).GetProperty("id").GetValue(this.ObjCollection[i])) == id)
            {
                ObjCollection.Remove(ObjCollection[i]);
                _fileManager.Serialize(ObjCollection);
                break;
            }
        }
    }

    public void Edit(int id, string field, object change)
    {
        bool isNumeric = int.TryParse(change.ToString(), out _);

        if (typeof(T).GetProperty(field).GetType().Name == "int" && isNumeric)
        {
            change = int.Parse(change.ToString());
        }

        T objToEdit = this.GetById(id);
        try
        {
            objToEdit.GetType().GetProperty(field).SetValue(objToEdit, change);
        }
        catch (Exception e)
        {
            throw new ValidationException(e.Message);
        }
        RemoveById(id);
        Add(objToEdit);
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

        foreach (var curr in ObjCollection)
        {
            a.Add(curr);
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
            ObjCollection.Add(cur);
        }


        foreach (var curr in ObjCollection)
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
        var quer = ObjCollection.Where(x => ToDict(x).Values.Any(x => x.ToString().ToLower().Contains(searchQuery)));

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
        get { return this.ObjCollection[i]; }
        set { ObjCollection[i] = value; }
    }
    
    public IEnumerator GetEnumerator()
    {
        return this.ObjCollection.GetEnumerator();
    }
}

public class FileManager<T> where T: new()
{
    private readonly JsonSerializerOptions _options;
    public string Path { get; set; }
    
    public FileManager(string path)
    {
        this._options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {WriteIndented = true};
        this.Path = path;
    }
    
    public void Serialize(List<T> objCollection)
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
            sw.WriteLine(JsonSerializer.Serialize(objCollection, this._options));
        }
    }

    public List<T> DeSerialize()
    {
        string json = File.ReadAllText(Path);
        if (json.Length == 0)
        {
            throw new Exception("Nothing to deserialize");
        }

        List<Dictionary<string, object>> objectsList =
            JsonSerializer.Deserialize<List<Dictionary<string, object>>>(json, this._options);

        List<T> returnObj = new List<T>();

        foreach (var cur in objectsList)
        {
            try
            {
                returnObj.Add(ConvertDictionaryTo<T>(cur));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Object with an id {cur["id"]} needs your attention");
                Console.WriteLine(e.Message);
                continue;
            }
        }
        
        return returnObj;
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
                if (e.InnerException != null)
                {
                    errorCollection.Append(e.InnerException.Message);
                    errorCollection.Append("\n");
                }
                continue;
            }
        }

        if (errorCollection.ToString() != "")
        {
            throw new ValidationException(errorCollection.ToString());
        }

        return ret;
    }
}
