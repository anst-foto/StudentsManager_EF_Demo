using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using StudentsManager_EF_Demo.Desktop.Models;
using AppContext = StudentsManager_EF_Demo.Desktop.Models.AppContext;

namespace StudentsManager_EF_Demo.Desktop.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly AppContext _db;

    private int? _id;

    public int? Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    private string? _lastName;

    public string? LastName
    {
        get => _lastName;
        set => SetProperty(ref _lastName, value);
    }

    private string? _firstName;

    public string? FirstName
    {
        get => _firstName;
        set => SetProperty(ref _firstName, value);
    }

    private string? _faculty;

    public string? Faculty
    {
        get => _faculty;
        set => SetProperty(ref _faculty, value);
    }

    public ObservableCollection<Student> Students { get; } = [];

    private Student? _selectedStudent;

    public Student? SelectedStudent
    {
        get => _selectedStudent;
        set
        {
            var res = SetProperty(ref _selectedStudent, value);
            if (!res) return;

            Id = value?.Id;
            LastName = value?.LastName;
            FirstName = value?.FirstName;
            Faculty = value?.Faculty;
        }
    }

    private string? _searchText;

    public string? SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    public ICommand CommandLoad { get; }
    public ICommand CommandSave { get; }
    public ICommand CommandDelete { get; }
    public ICommand CommandClear { get; }
    public ICommand CommandSearch { get; }
    public ICommand CommandClearSearch { get; }

    public MainWindowViewModel()
    {
        _db = new AppContext();

        CommandClear = new LambdaCommand(
            execute: _ => Clear(),
            canExecute: _ => !string.IsNullOrWhiteSpace(LastName) ||
                             !string.IsNullOrWhiteSpace(FirstName) ||
                             !string.IsNullOrWhiteSpace(Faculty));
        CommandLoad = new LambdaCommand(_ => Load());
        CommandSearch = new LambdaCommand(
            execute: _ => Search(),
            canExecute: _ => !string.IsNullOrWhiteSpace(SearchText));
        CommandClearSearch = new LambdaCommand(
            execute: _ =>
            {
                SearchText = null;
                Load();
            },
            canExecute: _ => !string.IsNullOrWhiteSpace(SearchText));
        CommandSave = new LambdaCommand(
            execute: _ => Save(),
            canExecute: _ => !string.IsNullOrWhiteSpace(LastName) &&
                             !string.IsNullOrWhiteSpace(FirstName) &&
                             !string.IsNullOrWhiteSpace(Faculty));
        CommandDelete = new LambdaCommand(
            execute: _ => Delete(),
            canExecute: _ => SelectedStudent != null);
    }

    private void Clear()
    {
        SelectedStudent = null;

        LastName = null;
        FirstName = null;
        Faculty = null;
        Id = null;
    }

    private void Load()
    {
        _db.Students.Load();

        Students.Clear();
        foreach (var student in _db.Students)
        {
            Students.Add(student);
        }
    }

    private void Search()
    {
        var students = _db.Students.Where(s => s.LastName.Contains(SearchText!) ||
                                s.FirstName.Contains(SearchText!) ||
                                s.Faculty.Contains(SearchText!))
            .ToList();
        
        Students.Clear();
        foreach (var student in students)
        {
            Students.Add(student);
        }
    }

    private void Save()
    {
        if (Id == null)
        {
            _db.Students.Add(new Student
            {
                LastName = LastName!,
                FirstName = FirstName!, 
                Faculty = Faculty!
            });
        }
        else
        {
            var student = _db.Students.Single(s => s.Id == Id);
            student.LastName = LastName!;
            student.FirstName = FirstName!;
            student.Faculty = Faculty!;
        }
        
        _db.SaveChanges();
        
        Clear();
        Load();
    }

    private void Delete()
    {
        var student = _db.Students.Single(s => s.Id == Id);
        _db.Students.Remove(student);
        _db.SaveChanges();
        
        Clear();
        Load();
    }
}