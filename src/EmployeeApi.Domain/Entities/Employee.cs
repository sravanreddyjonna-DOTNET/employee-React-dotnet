namespace EmployeeApi.Domain.Entities;

public class Employee
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public decimal Salary { get; private set; }
    public string Dept { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;

    // Parameterless constructor reserved for EF Core materialization.
    private Employee() { }

    public Employee(string name, decimal salary, string dept, string email, string phone)
    {
        Name = name;
        Salary = salary;
        Dept = dept;
        Email = email;
        Phone = phone;
    }
}
