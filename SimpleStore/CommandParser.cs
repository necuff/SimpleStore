namespace SimpleStore
{
    public static class CommandParser
    {        
        public static Command Parse(ReadOnlySpan<char> input)
        {            
            if (input.IsEmpty)
                return new Command();

            input = input.Trim();

            int index1 = input.IndexOf(" ");
            if(index1 < 0)
                return new Command();

            ReadOnlySpan<char> command = input.Slice(0, index1);
            ReadOnlySpan<char> key = new ReadOnlySpan<char>();
            ReadOnlySpan<char> value = new ReadOnlySpan<char>();

            input = input.Slice(index1 + 1).TrimStart();
            int index2 = input.IndexOf(" ");
            
            if(index2 >= 0)
            {
                key = input.Slice(0, index2);
                value = input.Slice(index2 + 1).TrimStart();
            } else
            {
                key = input;
            }

            var resultCommand = new Command() { command = command, key = key, value = value };

            if(CommandIsValid(resultCommand))
                return resultCommand;
            else
                return new Command();
        }

        private static bool CommandIsValid(Command command)
        {
            if (!command.command.Equals("get", StringComparison.OrdinalIgnoreCase) 
                && !command.command.Equals("set", StringComparison.OrdinalIgnoreCase)
                && !command.command.Equals("delete", StringComparison.OrdinalIgnoreCase)
                )
                return false;

            if(
                (command.command.Equals("get", StringComparison.OrdinalIgnoreCase) 
                    || command.command.Equals("delete", StringComparison.OrdinalIgnoreCase)
                ) 
                && (command.key.IsEmpty || !command.value.IsEmpty ))
                return false;
            
            
            if(command.command.Equals("set", StringComparison.OrdinalIgnoreCase) 
                && (command.key.IsEmpty || command.value.IsEmpty))
                return false;
                        
            if(command.value.IndexOf(" ") > 0)
                return false;

            return true;
        }
    }

    public ref struct Command()
    {
        public ReadOnlySpan<char> command;
        public ReadOnlySpan<char> key;
        public ReadOnlySpan<char> value;
    }
}
