using System;
using System.Security.Permissions;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;


namespace IraTask_1;


public class UnsignedInteger: ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        int valueToCheck = ((int?)value) ?? throw new Exception("Wrong number type");
        if (valueToCheck <= 0)
        {
            return false;
        }
        return true;
    }
}

public class CheckRegNumber: ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        string valueToCheck = (string)value ?? throw new Exception("Wrong parameter type");
        Regex regex = new Regex("^[A-Z]{2}[0-9]{4}[A-Z]{2}");
        if (!regex.IsMatch(valueToCheck))
        {
            return false;
        }
        return true;
    }
}

public class CheckDateOfRegistration : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        DateTime valueToCheck = (DateTime) value;

        if (valueToCheck > DateTime.Now)
        {
            return false;
        }
        
        
        return true;
    }
}

public class CheckVinCode : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        string valueToCheck = (string?)value ?? throw new Exception("Wrong parameter type");
        Regex regex = new Regex("^[A-Z0-9]{17}");
        
        if (!regex.IsMatch(valueToCheck))
        {
            return false;
        }
        return true;
    }
}

public class CheckCar : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        string valueToCheck = (string?)value ?? throw new Exception("Wrong parameter type");

        foreach (var curr in RegistrationCertificate.GetCarsList())
        {
            if (curr == valueToCheck)
            {
                return true;
            }
        }

        return false;
    }
}


public class RegistrationCertificate
{
    public static string[] GetCarsList()
    {
        string[] carsList = {"Audi", "Mercedes", "Tesla"};
        return carsList;
    }

    private int _id;
    [UnsignedInteger(ErrorMessage = "Wrong parameter type: must be unsigned integer")]
    [Required]
    public int id
    {
        get
        {
            return this._id;
        }
        set
        {
            ValidateProperty(value, "id");
            this._id = value;
        }
    }
    private string _registrationNumber;
    [CheckRegNumber(ErrorMessage = "Wrong registration number format: must be AA0000AA")] [Required]
    public string registrationNumber
    {
        get
        {
            return _registrationNumber;
        }
        set
        {
            ValidateProperty(value, "registrationNumber");
            this._registrationNumber = value;
        }
    }

    private DateTime _dateOfRegistration;
    [Required]
    [CheckDateOfRegistration(ErrorMessage = "Date must be lower that today")]
    public DateTime dateOfRegistration
    {
        get
        {
            return _dateOfRegistration;
        }
        set
        {
            ValidateProperty(value, "dateOfRegistration");
            this._dateOfRegistration = value;
        }
    }
    private string _vinCode;
    [CheckVinCode(ErrorMessage = "Wrong vincode format")]
    [Required]
    public string vinCode
    {
        get
        {
            return this._vinCode;
        }
        set
        {
            ValidateProperty(value, "vinCode");
            this._vinCode = value;
        }
    }

    private string _car;
    [CheckCar(ErrorMessage = "Car doesn't exist")]
    [Required]
    public string car
    {
        get
        {
            return this._car;
        }
        set
        {
            ValidateProperty(value, "car");
            this._car = value;
        }
    }

    private int _yearOfManufacture;

    [Range(1980, 2022)]
    [Required]
    public int yearOfManufacture
    {
        get
        {
            return this._yearOfManufacture;
        }
        set
        {
            ValidateProperty(value, "yearOfManufacture");
            this._yearOfManufacture = value;
        }
    }

    private void Validate(object objectToValidate, List<ValidationResult> results)
    {
        ValidationContext context = new ValidationContext(objectToValidate, null, null);
        bool isValid = Validator.TryValidateObject(objectToValidate, context, results, true);
        if (!isValid)
        {
            foreach (var curr in results)
            {
                Console.WriteLine(curr);
            }
            throw new ValidationException();
        }
    }

    private void ValidateProperty(object? value, string propName)
    {
        ValidationContext context = new ValidationContext(this){MemberName = propName};
        List<ValidationResult> results = new List<ValidationResult>();
        bool isValid = Validator.TryValidateProperty(value, context, results);
        if (!isValid)
        {
            throw new ValidationException(results[0].ErrorMessage);
        }
    }
    public RegistrationCertificate()
    {
        
    }

    public RegistrationCertificate(int id, string regNumber, string dateOfReg, string vinCode, string carName,
        int yearOfManufacture)
    {
        List<ValidationResult> validationResults = new List<ValidationResult>();
        this.id = id;
        this.registrationNumber = regNumber;
        this.vinCode = vinCode;
        this.car = carName;
        this.yearOfManufacture = yearOfManufacture;
        try
        {
            this.dateOfRegistration = DateTime.ParseExact(dateOfReg, "dd.MM.yyyy", CultureInfo.InvariantCulture);
        }
        catch (Exception e)
        {
            validationResults.Add(new ValidationResult(e.Message));
        }
        Validate(this, validationResults);
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions(){WriteIndented = true});
    }
}