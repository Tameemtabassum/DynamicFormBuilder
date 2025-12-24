using DynamicFormBuilder.Models;
using System.Collections.Generic;

namespace DynamicFormBuilder.Services.Interfaces
{
    public interface IParticipantsService
    {
        ParticipantsModel GetParticipantById(int id);
        List<ParticipantsModel> GetAllParticipants();
        void AddParticipant(ParticipantsModel participant);
        void UpdateParticipant(ParticipantsModel participant);
        void DeleteParticipant(int id);
        int Count();
        List<ParticipantsModel> GetPage(int page, int pageSize);



    }
}

