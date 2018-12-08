namespace Entities
{
    public enum CommandNames
    {
        help,
        quit,
        listadmins,
        readmanager,
        listmanagers,
        readadmin,
        createadmin,
        createmanager,
        deletemanager,
        updatemanager,
        updateprofile,
        readuser,
        createuser,
        updateuser,
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