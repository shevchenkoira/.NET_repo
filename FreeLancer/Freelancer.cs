using System.ComponentModel.DataAnnotations;

namespace programming_task_1;

public class Freelancer
{
    private int _id;

    public int id
    {
        get
        {
            return this._id;
        }
        set
        {
            FreelanceValidator.CheckId(value.ToString());

            this._id = value;
        }
    }

    private string _name;

    public string name
    {
        get
        {
            return this._name;
        }
        set
        {
            FreelanceValidator.CheckName(value);

            this._name = value;
        }
    }

    private string _email;

    public string email
    {
        get
        {
            return this._email;
        }
        set
        {
            FreelanceValidator.CheckEmail(value.ToString());

            this._email = value;
        }
    }

    private string _phoneNumber;

    public string phoneNumber
    {
        get
        {
            return this._phoneNumber;
        }
        set
        {
            FreelanceValidator.CheckPhoneNumber(value.ToString());

            this._phoneNumber = value;
        }
    }

    private int _availability;

    public int availability
    {
        get
        {
            return this._availability;
        }
        set
        {
            FreelanceValidator.CheckAvailability(value.ToString());

            this._availability = value;
        }
    }

    private int _salary;

    public int salary
    {
        get
        {
            return this._salary;
        }
        set
        {
            FreelanceValidator.CheckSalary(value.ToString());
            this._salary = value;
        }
    }

    private string _position;

    public string position
    {
        get
        {
            return this._position;
        }
        set
        {
            FreelanceValidator.CheckPosition(value.ToString());

            this._position = value;
        }
    }

    public Freelancer() { }

    public Freelancer(int id, string name, string email, string phoneNumber, int availability,
        int salary, string position)
    {
        this.id = id;
        this.name = name;
        this.email = email;
        this.phoneNumber = phoneNumber;
        this.availability = availability;
        this.salary = salary;
        this.position = position;
    }

    public override string ToString()
    {
        return "Id: " + this.id + "\n"
            + "Name: " + this.name + "\n"
            + "Email: " + this.email + "\n"
            + "PhoneNumber: " + this.phoneNumber + "\n"
            + "Availability: " + this.availability + "\n"
            + "Salary: " + this.salary + "\n"
            + "Position: " + this.position + "\n";
    }
}

