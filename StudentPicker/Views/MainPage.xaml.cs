using StudentPicker.Models;
using StudentPicker.Services;
using Microsoft.Maui.Controls;

namespace StudentPicker.Views;

public partial class MainPage : ContentPage
{
    private List<Student> students = new();
    private Random rand = new();
    private int luckyNumber = 0;
    private bool luckyGenerated = false;


    public MainPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await RefreshClassList();
        await GenerateLuckyNumberIfNeeded();
    }

    private async Task GenerateLuckyNumberIfNeeded()
    {
        if (luckyGenerated) return;

        var classNames = await FileService.GetAllClassNamesAsync();
        int maxStudents = 0;

        foreach (var className in classNames)
        {
            var list = await FileService.LoadClassAsync(className);
            if (list.Count > maxStudents)
                maxStudents = list.Count;
        }

        if (maxStudents > 0)
        {
            luckyNumber = rand.Next(1, maxStudents + 1);
            LuckyLabel.Text = $"🎉 Szczęśliwy numerek: {luckyNumber}";
            luckyGenerated = true;
        }
        else
        {
            LuckyLabel.Text = "Brak uczniów w szkole";
        }
    }

    private async void LoadListClicked(object sender, EventArgs e)
    {
        var className = ClassPicker.SelectedItem?.ToString();
        if (string.IsNullOrEmpty(className)) return;

        students = await FileService.LoadClassAsync(className);

        for (int i = 0; i < students.Count; i++)
        {
            students[i].ClassNumber = i + 1;
        }

        StudentList.ItemsSource = students;
    }


    private async void PickStudentClicked(object sender, EventArgs e)
    {
        var available = students.Where(s => s.ClassNumber != luckyNumber).ToList();

        if (available.Count == 0)
        {
            await DisplayAlert("Błąd", "Nie można wylosować ucznia, ponieważ wszyscy mają szczęśliwy numerek.", "OK");
            return;
        }

        var selected = available[rand.Next(available.Count)];
        ResultLabel.Text = $"Wylosowano: {selected.Name} (Nr: {selected.ClassNumber})";

        await FileService.SaveClassAsync(ClassPicker.SelectedItem.ToString(), students);
    }

    private async void OnAddClass(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddClassPage());
    }




    private async void OnEditClass(object sender, EventArgs e)
    {
        if (ClassPicker.SelectedItem is string selectedClass)
        {
            await Navigation.PushAsync(new EditClassPage(selectedClass));
        }
        else
        {
            await DisplayAlert("Błąd", "Wybierz klasę do edycji!", "OK");
        }
    }

    private async Task RefreshClassList()
    {
        var classList = await FileService.GetAllClassNamesAsync();
        ClassPicker.ItemsSource = classList;
    }

}
