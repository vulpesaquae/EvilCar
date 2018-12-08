namespace Entities
{
    public enum CommandNames
    {
        help,
        quit,
        createadmin,
        readadmin,
        createmanager,
        deletemanager,
        updatemanager,
        readmanager,
        updateprofile
    }

    public struct CommandDescription
    {
        public string description;
        public string arguments;
        public UserRole role;

        public CommandDescription(string description, UserRole role, string arguments = "")
        {
            this.description = description;
            this.role = role;
            this.arguments = arguments;
        }
    }
}