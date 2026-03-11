using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;
using System.Threading;

namespace RegalEdu.Application.Student.Commands
{
    public class UpdateStudentCommand : IRequest<Result>
    {
        public required StudentModel StudentModel { get; set; }
    }

    public class UpdateStudentCommandHandler : IRequestHandler<UpdateStudentCommand, Result>
    {
        private readonly IRegalEducationDbContext _db;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public UpdateStudentCommandHandler(IRegalEducationDbContext db, IMapper mapper, ILocalizationService localizer)
        {
            _db = db; _mapper = mapper; _localizer = localizer;
        }

        public async Task<Result> Handle(UpdateStudentCommand request, CancellationToken ct)
        {
            var m = request.StudentModel;

            var entity = await _db.Students
                .Include(s => s.Contacts)
                .Include(s => s.StudentActivity)
                .Include(s => s.StudentNote)
                .Include(s => s.StudentCourse)
                .Include(s => s.Enrollments)
                .FirstOrDefaultAsync(x => x.Id == m.Id, ct);

            if (entity == null)
                return Result.Failure(_localizer.Format(LocalizationKey.EntityNotFound, EntityName.Student));

            //_mapper.Map(m, entity);
            var contactToDelete = await _db.Contact.Where(s => s.StudentId == m.Id).ToListAsync(ct);
            if (contactToDelete.Any()) { _db.Contact.RemoveRange(contactToDelete); }
            var studentActivityDelete = await _db.StudentActivity.Where(s => s.StudentId == m.Id).ToListAsync(ct);
            if (studentActivityDelete.Any()) { _db.StudentActivity.RemoveRange(studentActivityDelete); }
            var studentNoteDelete = await _db.StudentNote.Where(s => s.StudentId == m.Id).ToListAsync(ct);
            if (studentNoteDelete.Any()) { _db.StudentNote.RemoveRange(studentNoteDelete); }
            var studentCourseDelete = await _db.StudentCourse.Where(s => s.StudentId == m.Id).ToListAsync(ct);
            if (studentCourseDelete.Any()) { _db.StudentCourse.RemoveRange(studentCourseDelete); }
            await _db.SaveChangesAsync();
            // Sync Contacts
            if (m.Contacts != null)
            {
                foreach (var contact in m.Contacts)
                {
                    var eContact = new Contact
                    {
                        FullName = contact.FullName,
                        Phone = contact.Phone,
                        Email = contact.Email,
                        Gender = contact.Gender,
                        Relationship = contact.Relationship,
                        Username = contact.Username,
                        Note = contact.Note,
                        Address = contact.Address,
                        StudentId = m.Id,
                    };
                    await _db.Contact.AddAsync(eContact);

                }
            }

            // Sync Activities
            if (m.StudentActivity != null)
            {
                foreach (var activity in m.StudentActivity)
                {
                    var eActivity = new StudentActivity
                    {
                        Type = activity.Type,
                        Content = activity.Content,
                        ActivityDate = activity.ActivityDate,
                        EmployeeId = activity.EmployeeId,
                        Results = activity.Results,
                        NextAction = activity.NextAction,
                        CallLogURL = activity.CallLogURL,
                        CallId = activity.CallId,
                        UserID = activity.UserID,
                        StatusCode = activity.StatusCode,
                        ReasonFailed = activity.ReasonFailed,
                        StudentId = m.Id,
                    };

                    await _db.StudentActivity.AddAsync(eActivity);

                }
            }

            // Sync Notes
            if (m.StudentNote != null)
            {
                foreach (var note in m.StudentNote)
                {
                    var eNote = new StudentNote
                    {
                        EmployeeId = note.EmployeeId,
                        NoteDate = note.NoteDate,
                        NoteContext = note.NoteContext,
                        StudentId = m.Id,
                    };

                    await _db.StudentNote.AddAsync(eNote);

                }
            }

            // Sync Courses
            if (m.StudentCourse != null)
            {
                foreach (var coures in m.StudentCourse)
                {
                    var eCoures = new StudentCourse
                    {
                        CourseId = coures.CourseId,
                        CourseName = coures.CourseName,
                        InterestLevel = coures.InterestLevel,
                        Reason = coures.Reason,

                        StudentId = m.Id,
                    };

                    await _db.StudentCourse.AddAsync(eCoures);
                }
            }

            // Sync Enrollments
            if (m.Enrollments != null)
            {
                foreach (var enroll in m.Enrollments)
                {
                    var eEnroll = new Enrollment
                    {
                        ClassId = enroll.ClassId,
                        Fee = enroll.Fee,
                        Discount = enroll.Discount,
                        FinalFee = enroll.FinalFee,
                        PaymentCourseStatus = enroll.PaymentCourseStatus,
                        StudentId = m.Id,
                    };

                    await _db.Enrollments.AddAsync(eEnroll);

                }
            }
            entity.FullName = m.FullName;
            entity.Phone = m.Phone;
            entity.Email = m.Email;
            entity.Gender = m.Gender;
            entity.Address = m.Address;
            entity.Status = m.Status;
            entity.StudentStatus = m.StudentStatus;
            entity.StudentCode = m.StudentCode;
            entity.EnglishName = m.EnglishName;
            entity.Age = m.Age;
            entity.Priority = m.Priority;
            entity.ExpectedStartDate = m.ExpectedStartDate;
            entity.ExpectedBudget = m.ExpectedBudget;
            entity.BirthDate = m.BirthDate;
            entity.EmployeeId = m.EmployeeId;
            entity.LeadSource = m.LeadSource;
            entity.Reason = m.Reason;
            entity.CurrentLevel = m.CurrentLevel;
            entity.LearningGoal = m.LearningGoal;
            //  await _db.SaveChangesAsync(ct);
            // Scalar fields & FKs
          //  await _db.SaveChangesAsync(ct);
          // Scalar fields & FKs

            // if(entity!=null)
            _db.Students.Update(entity);
            var ok = await _db.SaveChangesAsync(ct) > 0;
            return ok
                ? Result.Success(_localizer.Format(LocalizationKey.MSG_UPDATE_SUCCESS, EntityName.Student))
                : Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, EntityName.Student));
        }

        /// Helper sync add/update/remove theo Id
        private static void SyncCollection<TEntity, TModel>(
            ICollection<TEntity> existing,
            IEnumerable<TModel> incoming,
            Func<TEntity, Guid> keyE,
            Func<TModel, Guid?> keyM,
            Action<TEntity, TModel> updater,
            Func<TModel, TEntity> creator) where TEntity : class
        {
            // remove
            var toRemove = existing.Where(e => !incoming.Any(m => keyM(m).HasValue && keyM(m)!.Value == keyE(e))).ToList();
            foreach (var r in toRemove) existing.Remove(r);

            // upsert
            foreach (var m in incoming)
            {
                var k = keyM(m);
                var found = k.HasValue ? existing.FirstOrDefault(e => keyE(e) == k.Value) : null;
                if (found != null) updater(found, m);
                else existing.Add(creator(m));
            }
        }
    }
}
