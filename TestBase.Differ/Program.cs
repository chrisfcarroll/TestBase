using System.Text.RegularExpressions;
using TestBase;

if (args.Length is 3 
    && args[0] is "--"
    && !string.IsNullOrWhiteSpace(args[1]) 
    && !string.IsNullOrWhiteSpace(args[2]) 
    && File.Exists(args[1]) 
    && File.Exists(args[2]))
{
    var left= File.ReadAllText(args[1]);
    var right= File.ReadAllText(args[2]);
    DiffFormatter.UseColour = args.Length < 4 || !Regex.IsMatch(args[3], "^(-n|--no-col(ou?r)?)");
    Console.WriteLine( Differ.Diff(left,right).Message );
}
else if(args.Length is 2 or 3)
{
    DiffFormatter.UseColour = args.Length is 2 || !Regex.IsMatch(args[2], "^(-n|--no-col(ou?r)?)");
    Console.WriteLine( Differ.Diff(args[0],args[1]).Message );
}
else
{
    Console.WriteLine(
        """
        Usage: 
            TestBase.Differ <string1> <string2> [-nc | --no-col[our] ]
            TestBase.Differ -- <file1> <file2> [-nc | --no-col[our] ]
        """);
}
