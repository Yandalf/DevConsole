namespace com.SolePilgrim.DevConsole
{
    /// <summary>DevConsole Command Input and Output data.</summary>
    public struct DevConsoleEntry
	{
		public string Input		{ get; private set; }
		public string Output	{ get; private set; }

		static private readonly int _paddingSpaces = 4;


		public DevConsoleEntry(string input, string output)
		{
			Input = input;
			Output = output;
		}

		public override string ToString()
		{
			return Input +
				(string.IsNullOrEmpty(Output) ? "" :
				"\n" + new string(' ', _paddingSpaces) + Output.Replace("\n", "\n" + new string(' ', _paddingSpaces)));
		}
	}
}