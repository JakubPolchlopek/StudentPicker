using System.Text.Json;
using StudentPicker.Models;
using System.Diagnostics;

namespace StudentPicker.Services;

public static class FileService
{
    private static string DirectoryPath =>
        Path.Combine(FileSystem.AppDataDirectory, "Classes");

    private static string GetClassFilePath(string className) =>
        Path.Combine(DirectoryPath, $"{className}.json");

    public static async Task<bool> ClassExistsAsync(string className)
    {
        string path = GetClassFilePath(className);
        return File.Exists(path);
    }

    public static async Task SaveClassAsync(string className, List<Student> students)
    {
        if (!Directory.Exists(DirectoryPath))
            Directory.CreateDirectory(DirectoryPath);

        string json = JsonSerializer.Serialize(students);
        string path = GetClassFilePath(className);
        await File.WriteAllTextAsync(path, json);
    }

    public static async Task<List<Student>> LoadClassAsync(string className)
    {
        string path = GetClassFilePath(className);
        if (!File.Exists(path))
            return new List<Student>();

        string json = await File.ReadAllTextAsync(path);
        return JsonSerializer.Deserialize<List<Student>>(json) ?? new List<Student>();
    }

    public static async Task<List<string>> GetAllClassNamesAsync()
    {
        if (!Directory.Exists(DirectoryPath))
            return new List<string>();

        var files = Directory.GetFiles(DirectoryPath, "*.json");
        return files.Select(file => Path.GetFileNameWithoutExtension(file)).ToList();
    }
}
