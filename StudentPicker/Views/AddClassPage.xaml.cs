using StudentPicker.Models;
using StudentPicker.Services;

namespace StudentPicker.Views;

public partial class AddClassPage : ContentPage
{
    private List<Student> _students = new();

    public AddClassPage()
    {
        InitializeComponent();
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

    private async void OnSaveClass(object sender, EventArgs e)
    {
        var className = ClassNameEntry.Text?.Trim();
        if (string.IsNullOrEmpty(className))
        {
            await DisplayAlert("B³¹d", "Podaj nazwê klasy!", "OK");
            return;
        }

        bool exists = await FileService.ClassExistsAsync(className);
        if (exists)
        {
            await DisplayAlert("B³¹d", "Taka klasa ju¿ istnieje!", "OK");
            return;
        }

        await FileService.SaveClassAsync(className, _students);
        await DisplayAlert("Sukces", $"Dodano klasê {className}", "OK");
        await Navigation.PopAsync();
    }
}
