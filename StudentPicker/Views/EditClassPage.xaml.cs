using StudentPicker.Models;
using StudentPicker.Services;

namespace StudentPicker.Views;

public partial class EditClassPage : ContentPage
{
    private string _className;
    private List<Student> _students = new();

    public EditClassPage(string className)
    {
        InitializeComponent();
        _className = className;
        LoadClassData();
    }

    private async void LoadClassData()
    {
        _students = await FileService.LoadClassAsync(_className);
        StudentListView.ItemsSource = null;
        StudentListView.ItemsSource = _students;
    }

    private void OnAddStudent(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(NewStudentEntry.Text))
        {
            _students.Add(new Student { Name = NewStudentEntry.Text });
            NewStudentEntry.Text = string.Empty;
            StudentListView.ItemsSource = null;
            StudentListView.ItemsSource = _students;
        }
    }

    private void OnDeleteStudent(object sender, EventArgs e)
    {
        var student = (sender as Button)?.CommandParameter as Student;
        if (student != null)
        {
            _students.Remove(student);
            StudentListView.ItemsSource = null;
            StudentListView.ItemsSource = _students;
        }
    }

    private async void OnSave(object sender, EventArgs e)
    {
        await FileService.SaveClassAsync(_className, _students);
        await DisplayAlert("Zapisano", "Zmieniono listê uczniów", "OK");
        await Navigation.PopAsync();
    }
}
