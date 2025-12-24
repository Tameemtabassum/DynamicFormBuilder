using DocumentFormat.OpenXml.InkML;
using DynamicFormBuilder.Data.DBContext;
using DynamicFormBuilder.Models;
using DynamicFormBuilder.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DynamicFormBuilder.Services.Implementations
{
    public class ParticipantsService : IParticipantsService
    {
        private readonly ApplicationDBContext _dbContext;

        public ParticipantsService(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<ParticipantsModel> GetAllParticipants()
        {
      
            return _dbContext.ParticipantsModels?.ToList() ?? new List<ParticipantsModel>();
        }
        public int Count()
        {
            return _dbContext.ParticipantsModels.Count();
        }

        public List<ParticipantsModel> GetPage(int page, int pageSize)
        {
            return _dbContext.ParticipantsModels
                .OrderBy(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public ParticipantsModel GetParticipantById(int id)
        {
            return _dbContext.ParticipantsModels.Find(id);
        }

        public void AddParticipant(ParticipantsModel participant)
        {
            if (participant == null) return;

            _dbContext.ParticipantsModels.Add(participant);
            _dbContext.SaveChanges();
        }

        public void UpdateParticipant(ParticipantsModel participant)
        {
            if (participant == null) return;

            var existing = _dbContext.ParticipantsModels.Find(participant.Id);
            if (existing == null) return;

            
            _dbContext.Entry(existing).CurrentValues.SetValues(participant);
            _dbContext.SaveChanges();
        }

        public void DeleteParticipant(int id)
        {
            var participant = _dbContext.ParticipantsModels.Find(id);
            if (participant == null) return;

            _dbContext.ParticipantsModels.Remove(participant);
            _dbContext.SaveChanges();
        }

     



}
    }
