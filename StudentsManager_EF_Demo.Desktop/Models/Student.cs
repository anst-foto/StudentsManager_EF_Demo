namespace StudentsManager_EF_Demo.Desktop.Models;

public class Student
{
    public int Id { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string Faculty { get; set; }
    
    public string FullName => $"{LastName} {FirstName}";
    public string FullNameWithInitials => $"{LastName} {FirstName[0]}.";
}