using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace IraTask_1;

public class Menu<T> where T:new()
{
    public Collection<T> ObjCollection;
    private Dictionary<string, Func<object>> _moduleCollection;

    public Menu(string path)
    {
        ObjCollection = new Collection<T>(path);
        
        _moduleCollection = new Dictionary<string, Func<object>>();
        
        _moduleCollection["Add"] = () => { Add();
            return 0;
        };
        _moduleCollection["Remove"] = () => { RemoveById();
            return 0;
        };
        _moduleCollection["Edit"] = () => { EditById();
            return 0;
        };
        _moduleCollection["Sort"] = () => { SortByField();
            return 0;
        };
        _moduleCollection["Search"] = () => { SearchBy();
            return 0;
        };
        _moduleCollection["Get all"] = () => { GetAll();
            return 0;
        };
    }

    public void Run()
    {
        string helpString = "Available commands:\n" +
                      "'Add': Add new record to the collection\n" +
                      "'Remove': Remove item from the collection by id\n" +
                      "'Edit': Edit item from the collection by id\n" +
                      "'Sort': Sort items in the collection by field\n" +
                      "'Search': Search items in the collection by any search query\n"+
                      "'Get all': Get all collection records\n"+
                      "Type 'exit' to stop program execution\n";

        string input;
        while (true)
        {
            Console.WriteLine(helpString);
            input = ValidateInput("Command line cannot be blank");

            if (input == "exit")
            {
                break;
            }
            
            if (_moduleCollection.Keys.Contains(input))
            {
                _moduleCollection[input]();
            }
            else
            {
                Console.WriteLine("Such command doesn't exist, try again\n");
                continue;
            }
        }
    }

    private void Add()
    {
        Dictionary<string, object> objectToAdd = new Dictionary<string, object>();
        foreach (var cur in typeof(T).GetProperties())
        {
            Console.WriteLine($"Enter {cur.Name}: ");
            string input = ValidateInput($"You have entered wrong {cur.Name} try again: ");

            objectToAdd[cur.Name] = input;
        }

        try
        {
            T obj = ConvertDictionaryTo<T>(objectToAdd);
            ObjCollection.Add(obj);
        }
        catch (ValidationException e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine("Change wrong fields");
        }
        
    }

    private void GetById(int id)
    {
        Console.WriteLine(JsonSerializer.Serialize(ObjCollection.GetById(id), new JsonSerializerOptions(){WriteIndented = true}));
    }

    private void RemoveById()
    {
        int id;
        try
        {
            id = Convert.ToInt32(ValidateInput("You have entered wrong id"));
        }
        catch (Exception e)
        {
            Console.WriteLine("Wrong id type");
            return;
        }
        ObjCollection.RemoveById(id);
    }

    private void EditById()
    {
        Console.WriteLine("Enter id: ");
        int id;
        try
        {
            id = Convert.ToInt32(ValidateInput("You have entered wrong id"));
        }
        catch (Exception e)
        {
            Console.WriteLine("Wrong id type");
            return;
        }
        
        try
        {
            ObjCollection.GetById(id);
        }
        catch (Exception e)
        {
            Console.WriteLine("User with such id doesn't exist");
            return;
        }
        
        Console.WriteLine("Here is the object you want to change");
        GetById(id);
        
        List<PropertyInfo> propertyInfos = typeof(T).GetProperties().ToList();
        
        Console.WriteLine("Available fields to edit: ");
        int i = 1;
        foreach (var cur in propertyInfos)
        {
            Console.WriteLine($"{i}) {cur.Name}: ");
        }
        Console.WriteLine("Choose one of this: ");
        
        bool flag = false;

        string field = "";
        
        while (!flag)
        {
            field = Console.ReadLine();
            if (field is null)
            {
                Console.WriteLine("Cannot be blank line: ");
                continue;
            }
            foreach (var cur in propertyInfos)
            {
                if (field == cur.Name)
                {
                    flag = true;
                }
            }

            if (!flag)
            {
                Console.WriteLine("Enter other field - typed one doesn't exist");
            }
        }

        string change = "";
        
        Console.WriteLine("Enter change:");
        change = Console.ReadLine();

        try
        {
            ObjCollection.Edit(id, field, change);
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

    private void SortByField()
    {
        Console.WriteLine("Available fields to edit: ");
        int i = 1;
        foreach (var cur in typeof(T).GetProperties())
        {
            Console.WriteLine($"{i}) {cur.Name}: ");
            i++;
        }
        Console.WriteLine("Enter name of field: ");
        string field = ValidateInput("Wrong field format, try again");
        bool isValid = false;
        
        foreach (var currentProp in typeof(T).GetProperties())
        {
            if (field == currentProp.Name)
            {
                isValid = true;
                break;
            }
        }

        if (!isValid)
        {
            Console.WriteLine("You have entered wrong field name, try again later");
            return;
        }
        Console.WriteLine("Now enter order - 'desc' or 'asc': ");
        string order = ValidateInput("Wrong order format, try again");

        ObjCollection.Sort(field, order);
        foreach (var curr in ObjCollection)
        {
            Console.WriteLine(JsonSerializer.Serialize(curr, new JsonSerializerOptions(){WriteIndented = true}));
        }
    }

    private void SearchBy()
    {
        Console.WriteLine("Enter search query: ");
        string searchQuery = ValidateInput("You have entered wrong search query, try another:\n");
        ObjCollection.SearchBy(searchQuery);
    }

    private void GetAll()
    {
        foreach (var currentElement in this.ObjCollection)
        {
            Console.WriteLine(JsonSerializer.Serialize(currentElement, new JsonSerializerOptions(){WriteIndented = true}));
        }
    }

    private string ValidateInput(string errorMessage)
    {
        string input = Console.ReadLine();
        while (input == "")
        {
            Console.WriteLine(errorMessage);
            input = Console.ReadLine();
        }

        return input;
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