using System.Runtime.Versioning;
using DarkModeSwitcher.Enums;
using Figgle;
using Microsoft.Win32;
using Spectre.Console;

namespace DarkModeSwitcher;

[SupportedOSPlatform("windows")]
static class Program
{
    static void Main()
    {
        try
        {
            PrintHeader();
            var response = ShowMenu();
            Execute(response).Wait();
        }
        catch (Exception e)
        {
            AnsiConsole.WriteException(e, ExceptionFormats.ShortenEverything);
            Environment.Exit((int) ExitCodes.Failure);
        }
    }

    static void PrintHeader()
    {
        var logoText = FiggleFonts.Standard.Render("Dark   Mode   Switcher");
        AnsiConsole.MarkupLine($"[maroon]{logoText}[/]");
        AnsiConsole.MarkupLine("Press [green]Ctrl+C[/] to exit or pick [green]Exit[/] from the menu.");
    }

    static string ShowMenu()
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Switch Mode?")
                .AddChoices("Dark", "Light", "Complete Light", "Exit"));

        return choice;
    }

    static async Task Execute(string action)
    {
        AnsiConsole.WriteLine();
        switch (action)
        {
            case "Dark":
                AnsiConsole.MarkupLine($"[royalblue1]Enabling[/] Dark Mode...");
                await WriteToWindowsRegistry(appsLightMode: false, systemLightMode: false);

                break;

            case "Light":
                AnsiConsole.MarkupLine($"[royalblue1]Enabling[/] Light Mode...");
                await WriteToWindowsRegistry(appsLightMode: true, systemLightMode: false);

                break;

            case "Complete Light":
                AnsiConsole.MarkupLine($"[royalblue1]Enabling[/] Complete Light Mode...");
                await WriteToWindowsRegistry(appsLightMode: true, systemLightMode: true);

                break;

            case "Exit":
                AnsiConsole.MarkupLine("[green]Farewell.[/]");
                Environment.Exit((int) ExitCodes.Success);
                break;
        }

        AnsiConsole.MarkupLine($"[green]Done.[/]");
        AnsiConsole.WriteLine();
    }

    static async Task WriteToWindowsRegistry(bool appsLightMode, bool systemLightMode)
    {
        const string path = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize";
        await Task.Run(() =>
        {
            var rk = Registry.CurrentUser.OpenSubKey(path, true);
            rk?.SetValue("AppsUseLightTheme", appsLightMode, RegistryValueKind.DWord);
            rk?.SetValue("SystemUsesLightTheme", systemLightMode, RegistryValueKind.DWord);
        });
    }
}