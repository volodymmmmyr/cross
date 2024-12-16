using McMaster.Extensions.CommandLineUtils;
using ClassLibraryLab4;

namespace Lab4
{
    [Command(Name = "Lab4", Description = "Utility for running tasks from labs 1-3")]
    [Subcommand(typeof(VersionCommand), typeof(RunCommand), typeof(SetPathCommand))]
    public class LabApplication
    {
        public static int Main(string[] args)
        {
            var app = new CommandLineApplication<LabApplication>();
            app.Conventions.UseDefaultConventions();

            try
            {
                return app.Execute(args);
            }
            catch (CommandParsingException)
            {
                return 1;
            }
        }

        private void OnExecute(CommandLineApplication app)
        {
            DisplayHelp();
        }

        private static void DisplayHelp()
        {
            Console.WriteLine("Available commands:");
            Console.WriteLine("  version     - Displays app version and author info");
            Console.WriteLine("  run         - Runs a specific lab solution");
            Console.WriteLine("                Examples:");
            Console.WriteLine("                Lab4 run lab1 -i input.txt -o output.txt");
            Console.WriteLine("  set-path    - Sets the default directory for input/output files");
            Console.WriteLine("                Example:");
            Console.WriteLine("                Lab4 set-path -p /path/to/folder");
            Console.WriteLine("  help        - Shows this help message");
        }

        [Command("version", Description = "Displays app version and author info")]
        public class VersionCommand
        {
            private void OnExecute()
            {
                Console.WriteLine("Developer: Dmytro Mazur");
                Console.WriteLine("Version: 1.0.0");
            }
        }

        [Command("run", Description = "Runs a specific lab solution")]
        public class RunCommand
        {
            [Argument(0, "lab", "Specifies which lab to run (lab1, lab2, lab3)")]
            public string Lab { get; set; }

            [Option("-i|--input", "Path to the input file", CommandOptionType.SingleValue)]
            public string InputFile { get; set; }

            [Option("-o|--output", "Path to the output file", CommandOptionType.SingleValue)]
            public string OutputFile { get; set; }

            private void OnExecute()
            {
                var defaultPath = Environment.GetEnvironmentVariable("DEFAULT_PATH") ?? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

                var inputPath = InputFile ?? Path.Combine(defaultPath, "INPUT.txt");
                var outputPath = OutputFile ?? Path.Combine(defaultPath, "OUTPUT.txt");

                if (!File.Exists(inputPath))
                {
                    Console.WriteLine($"Error: File not found at {inputPath}");
                    return;
                }

                switch (Lab?.ToLower())
                {
                    case "lab1":
                        Lab1.ExecuteLab1(inputPath, outputPath);
                        break;
                    case "lab2":
                        Lab2.ExecuteLab2(inputPath, outputPath);
                        break;
                    case "lab3":
                        Lab3.ExecuteLab3(inputPath, outputPath);
                        break;
                    default:
                        Console.WriteLine("Invalid lab specified. Please choose lab1, lab2, or lab3.");
                        return;
                }

                Console.WriteLine($"Execution completed. Results saved to {outputPath}");
            }
        }

        [Command("set-path", Description = "Sets the default directory for input/output files")]
        public class SetPathCommand
        {
            [Option("-p|--path", "Specifies the directory for input/output files", CommandOptionType.SingleValue)]
            public string Directory { get; set; }

            private void OnExecute()
            {
                if (string.IsNullOrWhiteSpace(Directory))
                {
                    Console.WriteLine("Error: Directory path is not specified.");
                    return;
                }

                try
                {
                    SetEnvironmentPath("DEFAULT_PATH", Directory);
                    Console.WriteLine($"Default directory set to: {Directory}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error setting default directory: {ex.Message}");
                }
            }

            private static void SetEnvironmentPath(string variable, string path)
            {
                if (OperatingSystem.IsWindows())
                {
                    Environment.SetEnvironmentVariable(variable, path, EnvironmentVariableTarget.Machine);
                }
                else
                {
                    var configFile = OperatingSystem.IsLinux() ? "/etc/environment" : "/etc/paths";

                    if (!File.Exists(configFile))
                    {
                        throw new InvalidOperationException("System configuration file not found.");
                    }

                    using (var writer = File.AppendText(configFile))
                    {
                        writer.WriteLine($"{variable}={path}");
                    }
                }
            }
        }
    }
}
