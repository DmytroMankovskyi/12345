﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EventsExpress.Core.DTOs;
using EventsExpress.Core.Infrastructure;
using EventsExpress.Core.IServices;
using EventsExpress.Db.Entities;
using EventsExpress.Db.IRepo;

namespace EventsExpress.Core.Services
{
    public class CommentService : ICommentService
    {
        private readonly IMapper _mapper;

        public CommentService(IUnitOfWork uow, IMapper mapper)
        {
            Db = uow;
            _mapper = mapper;
        }

        public IUnitOfWork Db { get; set; }

        public IEnumerable<CommentDTO> GetCommentByEventId(Guid id, int page, int pageSize, out int count)
        {
            count = Db.CommentsRepository.Get().Count(x => x.EventId == id && x.CommentsId == null);

            var comments = Db.CommentsRepository
                .Get("User.Photo,Children")
                .Where(x => x.EventId == id && x.CommentsId == null)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsEnumerable();

            var com = _mapper.Map<IEnumerable<CommentDTO>>(comments);
            foreach (var c in com)
            {
                c.Children = _mapper.Map<IEnumerable<CommentDTO>>(c.Children);
                foreach (var child in c.Children)
                {
                    child.User = Db.UserRepository.Get("Photo").FirstOrDefault(u => u.Id == child.UserId);
                }
            }

            return com;
        }

        public async Task<OperationResult> Create(CommentDTO comment)
        {
            if (Db.UserRepository.Get(comment.UserId) == null)
            {
                return new OperationResult(false, "Current user does not exist!", string.Empty);
            }

            if (Db.EventRepository.Get(comment.EventId) == null)
            {
                return new OperationResult(false, "Wrong event id!", string.Empty);
            }

            Db.CommentsRepository.Insert(new Comments()
            {
                Text = comment.Text,
                Date = DateTime.Now,
                UserId = comment.UserId,
                EventId = comment.EventId,
                CommentsId = comment.CommentsId,
            });

            await Db.SaveAsync();

            return new OperationResult(true);
        }

        public async Task<OperationResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                return new OperationResult(false, "Id field is null", string.Empty);
            }

            var comment = Db.CommentsRepository.Get("Children").Where(x => x.Id == id).FirstOrDefault();
            if (comment == null)
            {
                return new OperationResult(false, "Not found", string.Empty);
            }

            if (comment.Children != null)
            {
                foreach (var com in comment.Children)
                {
                    var res = Db.CommentsRepository.Delete(Db.CommentsRepository.Get(com.Id));
                }
            }

            var result = Db.CommentsRepository.Delete(comment);
            await Db.SaveAsync();
            return new OperationResult(true);
        }
    }
}
