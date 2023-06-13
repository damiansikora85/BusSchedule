using BusSchedule.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusSchedule.Core.UI.Interfaces
{
    public interface ICardsManager
    {
        Task DeleteCard(ElectronicCardData cardData);
        Task<IList<ElectronicCardData>> GetCards();
        Task SaveCard(ElectronicCardData card);
    }
}
