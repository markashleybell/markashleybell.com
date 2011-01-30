using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using markashleybell.com.Domain.Abstract;
using markashleybell.com.Domain.Entities;

namespace markashleybell.com.Domain.Concrete
{
    public class CommentRepository : RepositoryBase<Comment>, ICommentRepository
    {
        public CommentRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public Comment Get(int id)
        {
            return base.Get(id);
        }
    }
}
