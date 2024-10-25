using CommandLine;

namespace ConsoleApp
{
    /// <summary>
    /// Definition of the various options availabe on the command line
    /// </summary>
    /// <see cref="https://github.com/commandlineparser/commandline"/>
    public class CommandLineOptions
    {
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }
    }
}
