using System;
using System.Collections.Generic;
using System.Text;

namespace BusSchedule.Interfaces
{
    public interface IFileAccess
    {
        string GetLocalFilePath(string filename);
    }
}
