using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvilCar
{
    class Administrator : UserType
    {
        public Administrator(string name) : base(name) {}

        public bool Admin_Create(string name, string password)
        {
            throw new NotImplementedException();
        }

        public void Admin_Read(string name)
        {
            throw new NotImplementedException();
        }

        public bool FleetManager_Create(string name, string password, string branch)
        {
            throw new NotImplementedException();
        }

        // Delete fleet manager if there are more than one fleet manager for a branch
        public bool FleetManager_Delete(string name)
        {
            throw new NotImplementedException();
        }

        // An asynchronous e-mail with the new password will be sent
        // E-mail will be mocked by an asynchronous Console.WriteLine task that lasts about 5-10 seconds
        public bool FleetManager_UpdatePassword(string name, string password)
        {
            throw new NotImplementedException();
        }

        public void FleetManager_Read(string name)
        {
            throw new NotImplementedException();
        }

        public void UpdateProfile()
        {
            throw new NotImplementedException();
        }

        public bool Branch_Create(string name)
        {
            throw new NotImplementedException();
        }
    }
}
