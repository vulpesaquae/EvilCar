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
        listusers,
        readuser,
        createuser,
        updateuser,
        createbranch,
        listfleets,
        createfleet,
        deletefleet,
        listcars,
        createcar,
        deletecar,
        calculatecosts
    }

    public struct CommandDescription
    {
        public string Description { get; }
        public string Arguments { get; }
        public UserRole Role { get; }

        public CommandDescription(string description, UserRole role, string arguments = "")
        {
            Description = description;
            Role = role;
            Arguments = arguments;
        }
    }
}