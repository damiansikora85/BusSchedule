using System;
using System.Collections.Generic;
using System.Text;

namespace BusSchedule.Core.Exceptions
{
    public class ElectronicCardException : Exception
    {
        public ElectronicCardException(string message, System.Net.HttpStatusCode statusCode, string reasonPhrase) : base(message)
        {
        }
    }
}
