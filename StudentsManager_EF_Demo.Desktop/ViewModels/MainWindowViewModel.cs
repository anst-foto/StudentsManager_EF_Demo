using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using StudentsManager_EF_Demo.Desktop.Models;
using AppContext = StudentsManager_EF_Demo.Desktop.Models.AppContext;

namespace StudentsManager_EF_Demo.Desktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly AppContext _db;

    [Reactive] private int? _id;

    [Reactive] private string? _lastName;

    [Reactive] private string? _firstName;

    [Reactive] private string? _faculty;


    public ObservableCollection<Student> Students { get; } = [];

    [Reactive] private Student? _selectedStudent;
    

    [Reactive] private string? _searchText;
    
    
    public ReactiveCommand<Unit, Unit> CommandSave { get; }
    public ReactiveCommand<Unit, Unit> CommandDelete { get; }
    public ReactiveCommand<Unit, Unit> CommandClear { get; }

    public MainWindowViewModel()
    {
        _db = new AppContext();

        this.WhenAnyValue(vm => vm.SelectedStudent)
            .Subscribe(s =>
            {
                Id = s?.Id;
                LastName = s?.LastName;
                FirstName = s?.FirstName;
                Faculty = s?.Faculty;
            });
        this.WhenAnyValue(vm => vm.SearchText)
            .Select(query => query?.Trim())
            .WhereNotNull()
            .Subscribe(search =>
            {
                var students = _db.Students.Where(s =>
                    s.LastName.ToLower().Contains(search.ToLower()) ||
                    s.FirstName.ToLower().Contains(search.ToLower()) ||
                    s.Faculty.ToLower().Contains(search.ToLower()));
        
                Students.Clear();
                foreach (var student in students)
                {
                    Students.Add(student);
                }
            });

        var canExecuteCommandClear = 
            this.WhenAnyValue(vm => vm.LastName,
                vm => vm.FirstName, 
                vm => vm.Faculty,
                (p1, p2, p3) => !string.IsNullOrWhiteSpace(p1) ||
                                !string.IsNullOrWhiteSpace(p2) || 
                                !string.IsNullOrWhiteSpace(p3));
        var canExecuteCommandSave = 
            this.WhenAnyValue(vm => vm.LastName, 
                vm => vm.FirstName, 
                vm => vm.Faculty,
                (p1, p2, p3) => !string.IsNullOrWhiteSpace(p1) &&
                                !string.IsNullOrWhiteSpace(p2) && 
                                !string.IsNullOrWhiteSpace(p3));
        var canExecuteCommandDelete = 
            this.WhenAnyValue(
                vm => vm.Id,
                vm => vm.SelectedStudent,
                (p1, p2) => p1 is not null || p2 is not null);

        CommandClear = ReactiveCommand.Create(
            execute: Clear,
            canExecute: canExecuteCommandClear );
        
        CommandSave = ReactiveCommand.Create(
            execute: Save,
            canExecute: canExecuteCommandSave);
        CommandDelete = ReactiveCommand.Create(
            execute: Delete,
            canExecute: canExecuteCommandDelete);
    }

    private void Clear()
    {
        SelectedStudent = null;

        _lastName = null;
        FirstName = null;
        Faculty = null;
        Id = null;
    }

    [ReactiveCommand]
    private void Load()
    {
        _db.Students.Load();

        Students.Clear();
        foreach (var student in _db.Students)
        {
            Students.Add(student);
        }
    }

    private void Save()
    {
        if (_id == null)
        {
            _db.Students.Add(new Student
            {
                LastName = _lastName!,
                FirstName = FirstName!, 
                Faculty = Faculty!
            });
        }
        else
        {
            var student = _db.Students.Single(s => s.Id == _id);
            student.LastName = _lastName!;
            student.FirstName = FirstName!;
            student.Faculty = Faculty!;
        }
        
        _db.SaveChanges();
        
        Clear();
        Load();
    }

    private void Delete()
    {
        var student = _db.Students.Single(s => s.Id == _id);
        _db.Students.Remove(student);
        _db.SaveChanges();
        
        Clear();
        Load();
    }
}