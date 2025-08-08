using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Application.Services.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using NArchitecture.Core.Persistence.Paging;
using StarterProject.Application.Tests.Mocks.FakeDatas;

namespace StarterProject.Application.Tests.Mocks.Repositories.Auth
{
    public class MockUserSessionRepository
    {
        private readonly SessionFakeData _sessionFakeData;
        private readonly List<UserSession> _sessions;

        public MockUserSessionRepository(SessionFakeData sessionFakeData)
        {
            _sessionFakeData = sessionFakeData;
            _sessions = sessionFakeData.CreateFakeData();
        }

        public IUserSessionRepository GetUserSessionMockRepository()
        {
            var mockRepo = new Mock<IUserSessionRepository>();

            #region GetAsync Mock
            mockRepo
                .Setup(s =>
                    s.GetAsync(
                        It.IsAny<Expression<Func<UserSession, bool>>>(),
                        It.IsAny<Func<IQueryable<UserSession>, IIncludableQueryable<UserSession, object>>>(),
                        It.IsAny<bool>(),
                        It.IsAny<bool>(),
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync(
                    (
                        Expression<Func<UserSession, bool>> predicate,
                        Func<IQueryable<UserSession>, IIncludableQueryable<UserSession, object>>? include,
                        bool withDeleted,
                        bool enableTracking,
                        CancellationToken cancellationToken
                    ) =>
                    {
                        return _sessions.FirstOrDefault(predicate.Compile());
                    }
                );
            #endregion

            #region GetListAsync Mock
            mockRepo
                .Setup(s =>
                    s.GetListAsync(
                        It.IsAny<Expression<Func<UserSession, bool>>>(),
                        It.IsAny<Func<IQueryable<UserSession>, IOrderedQueryable<UserSession>>>(),
                        It.IsAny<Func<IQueryable<UserSession>, IIncludableQueryable<UserSession, object>>>(),
                        It.IsAny<int>(),
                        It.IsAny<int>(),
                        It.IsAny<bool>(),
                        It.IsAny<bool>(),
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync(
                    (
                        Expression<Func<UserSession, bool>>? predicate,
                        Func<IQueryable<UserSession>, IOrderedQueryable<UserSession>>? orderBy,
                        Func<IQueryable<UserSession>, IIncludableQueryable<UserSession, object>>? include,
                        int index,
                        int size,
                        bool withDeleted,
                        bool enableTracking,
                        CancellationToken cancellationToken
                    ) =>
                    {
                        var query = _sessions.AsQueryable();

                        if (predicate != null)
                            query = query.Where(predicate);

                        if (orderBy != null)
                            query = orderBy(query);

                        var paginatedList = query.ToList();
                        return new Paginate<UserSession>
                        {
                            Items = paginatedList.Skip(index * size).Take(size).ToList(),
                            Index = index,
                            Size = size,
                            Count = paginatedList.Count,
                            Pages = (int)Math.Ceiling(paginatedList.Count / (double)size)
                        };
                    }
                );
            #endregion

            #region AddAsync Mock
            mockRepo
                .Setup(s =>
                    s.AddAsync(It.IsAny<UserSession>(), It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(
                    (UserSession session, CancellationToken cancellationToken) =>
                    {
                        session.Id = Guid.NewGuid();
                        _sessions.Add(session);
                        return session;
                    }
                );
            #endregion

            #region UpdateAsync Mock
            mockRepo
                .Setup(s =>
                    s.UpdateAsync(It.IsAny<UserSession>(), It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(
                    (UserSession session, CancellationToken cancellationToken) =>
                    {
                        var index = _sessions.FindIndex(s => s.Id == session.Id);
                        if (index >= 0)
                            _sessions[index] = session;
                        return session;
                    }
                );
            #endregion

            #region DeleteAsync Mock
            mockRepo
                .Setup(s =>
                    s.DeleteAsync(It.IsAny<UserSession>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(
                    (UserSession session, bool permanent, CancellationToken cancellationToken) =>
                    {
                        var index = _sessions.FindIndex(s => s.Id == session.Id);
                        if (index >= 0)
                            _sessions.RemoveAt(index);
                        return session;
                    }
                );
            #endregion

    

            return mockRepo.Object;
        }
    }
}