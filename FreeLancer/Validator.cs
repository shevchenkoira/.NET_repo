
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace programming_task_1;

public static class FreelanceValidator
{
    public static void CheckId(string? val)
    {
        int value;
        string message;
        if (!Int32.TryParse(val, out value))
        {
            message = "Invalid type of id. Must be integer";
            throw new ValidationException(message);
        }

        if (value < 0)
        {
            message = "Invalid id value. Must be unsigned integer";
            throw new ValidationException(message);
        }
    }

    public static void CheckAvailability(string? availability)
    {
        int avail = 0;
        if (!Int32.TryParse(availability, out avail))
        {
            const string message = "Invalid type of availability. Must be integer";
            throw new ValidationException(message);
        }

        if (Convert.ToInt32(availability) > 40)
        {
            const string message = "Availability can't be more than 40";
            throw new ValidationException(message);
        }
    }

    public static void CheckSalary(string? salary)
    {
        int sal;
        string message;
        if (!Int32.TryParse(salary, out sal))
        {
            message = "Invalid type of salary. Must be integer";
            throw new ValidationException(message);
        }

        if (sal < 0)
        {
            message = "Invalid salary value. Must be unsigned integer";
            throw new ValidationException(message);
        }
    }

    public static void CheckName(string? name)
    {
        if (name.Any(char.IsDigit))
        {
            const string message = "Name shouldn't contain numbers.";
            throw new ValidationException(message);
        }
    }

    public static void CheckEmail(string? email)
    {
        var emailAttributes = new List<string>() {"@", ".com"};
        if (email != null && (!email.Contains(emailAttributes[0]) || !email.Contains(emailAttributes[1])))
        {
            const string message = "Email isn't correct";
            throw new ValidationException(message);
        }

    }

    public static void CheckPhoneNumber(string? phoneNumber)
    {
        string substr = "+380";
        if (phoneNumber != null && (!phoneNumber.StartsWith(substr) || phoneNumber.Length != 13))
        {
            const string message = "Phone number isn't correct";
            throw new ValidationException(message);
        }
    }

    public static void CheckPosition(string? position)
    {
        var positions = new List<string>() {"FE Developer", "BE Developer", "DevOps"};
        if (!positions.Contains(position))
        {
            var message = "Invalid position";
            throw new ValidationException(message);
        }
    }
    
}