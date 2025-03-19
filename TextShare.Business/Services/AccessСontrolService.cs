using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TextShare.Business.Interfaces;
using TextShare.DAL.Interfaces;
using TextShare.DAL.Repositories;
using TextShare.Domain.Entities.AccessRules;
using TextShare.Domain.Entities.Complaints;
using TextShare.Domain.Entities.Groups;
using TextShare.Domain.Entities.TextFiles;
using TextShare.Domain.Entities.Users;

namespace TextShare.Business.Services
{
    /// <summary>
    /// Класс сервиса для управления доступом к файлам и полкам.
    /// </summary>
    public class AccessСontrolService : BaseService, IAccessСontrolService
    {
        private readonly IRepository<AccessRule> _accessRuleRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Group> _groupRepository;
        private readonly IRepository<TextFile> _textFileRepository;
        private readonly IRepository<Shelf> _shelfRepository;
        public AccessСontrolService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _accessRuleRepository = unitOfWork.AccessRuleRepository;
            _userRepository = unitOfWork.UserRepository;
            _groupRepository = unitOfWork.GroupRepository;
            _textFileRepository = unitOfWork.TextFileRepository;
            _shelfRepository = unitOfWork.ShelfRepository;

        }

        public async Task<AccessRule> GetCopyAccessRule(AccessRule accessRule)
        {
            await Task.CompletedTask;
            AccessRule accessRuleCopy = new()
            {
                AvailableAll = accessRule.AvailableAll,
                AvailableGroups = accessRule.AvailableGroups.ToList(),
                AvailableUsers = accessRule.AvailableUsers.ToList()
            };
            return accessRuleCopy;
        }

        public async Task<List<TextFile>> AvailableFilesFromGroups(int userId,
            params Expression<Func<TextFile, object>>[] includes)
        {
            List<TextFile> files = new();

            User? user = await _userRepository.GetAsync(userId,
                u => u.Groups, u => u.GroupMemberships
                );
            if (user == null)
            {
                return new List<TextFile>();
            }

            // id групп в которых пользователь состоит
            List<int> userGroupsIds = user.GroupMemberships
                .Where(gm => gm.IsConfirmed == true)
                .Select(gm => gm.GroupId)
                .ToList();

            // id групп созданных пользователем
            userGroupsIds.AddRange(user.Groups.Select(g => g.GroupId));
            userGroupsIds.Distinct();

            files = (await _textFileRepository.FindAsync(
                t => t.AccessRule.AvailableGroups.Any(ag => userGroupsIds.Contains(ag.GroupId)),
                includes
                )).DistinctBy(f => f.TextFileId).ToList();

            return files;
        }

        public async Task<List<TextFile>> AvailableFilesFromUsers(int userId,
            params Expression<Func<TextFile, object>>[] includes)
        {

            List<TextFile> files = new();

            User? user = await _userRepository.GetAsync(userId);
            if (user == null)
            {
                return new List<TextFile>();
            }

            files = (await _textFileRepository.FindAsync(
                t => t.AccessRule.AvailableUsers.Any(u => u.Id == user.Id),
                includes
                )).ToList();

            return files;
        }

        public async Task<List<TextFile>> AvailableFiles(int userId,
            params Expression<Func<TextFile, object>>[] includes)
        {
            List<TextFile> files = new();

            User? user = await _userRepository.GetAsync(userId);
            if (user == null)
            {
                return new List<TextFile>();
            }

            files.AddRange(await AvailableFilesFromGroups(userId, includes));
            files.AddRange(await AvailableFilesFromUsers(userId, includes));
            files = files.DistinctBy(t => t.TextFileId).ToList();

            return files;

        }

        public async Task<List<Shelf>> AvailableShelvesFromUsers(int userId,
            params Expression<Func<Shelf, object>>[] includes)
        {
            List<Shelf> shelves = new();

            User? user = await _userRepository.GetAsync(userId);
            if (user == null)
            {
                return new List<Shelf>();
            }

            shelves = (await _shelfRepository.FindAsync(
                s => s.AccessRule.AvailableUsers.Any(u => u.Id == user.Id),
                includes
                )).ToList();

            return shelves;
        }

        public async Task<List<Shelf>> AvailableShelvesFromGroups(int userId,
            params Expression<Func<Shelf, object>>[] includes)
        {
            List<Shelf> shelves = new();

            User? user = await _userRepository.GetAsync(userId,
                u => u.Groups, u => u.GroupMemberships);
            if (user == null)
            {
                return new List<Shelf>();
            }

            // ID групп, в которых пользователь состоит
            List<int> userGroupsIds = user.GroupMemberships
                .Where(gm => gm.IsConfirmed)
                .Select(gm => gm.GroupId)
                .ToList();

            // ID групп, созданных пользователем
            userGroupsIds.AddRange(user.Groups.Select(g => g.GroupId));
            userGroupsIds = userGroupsIds.Distinct().ToList();

            shelves = (await _shelfRepository.FindAsync(
                s => s.AccessRule.AvailableGroups.Any(g => userGroupsIds.Contains(g.GroupId)),
                includes
                )).ToList();

            return shelves;
        }

        public async Task<List<Shelf>> AvailableShelves(int userId,
            params Expression<Func<Shelf, object>>[] includes)
        {
            List<Shelf> shelves = new();

            User? user = await _userRepository.GetAsync(userId);
            if (user == null)
            {
                return new List<Shelf>();
            }

            shelves.AddRange(await AvailableShelvesFromGroups(userId, includes));
            shelves.AddRange(await AvailableShelvesFromUsers(userId, includes));
            shelves = shelves.DistinctBy(s => s.ShelfId).ToList(); // Удаление дубликатов

            return shelves;
        }

        public async Task<bool?> CheckTextFileAccess(User? user, TextFile textFile)
        {
            // Если правило доступа не загружено, загружаем
            if (textFile.AccessRule == null ||
                textFile.AccessRule.AvailableGroups == null ||
                textFile.AccessRule.AvailableUsers == null)
            {
                textFile = await _textFileRepository.GetAsync(textFile.TextFileId,
                    t => t.AccessRule,
                    t => t.AccessRule.AvailableGroups,
                    t => t.AccessRule.AvailableUsers);

                if (textFile == null) return null; // Проверка не удалась
            }

            // Если доступ разрешён всем — разрешаем для любого пользователя (в т.ч. null)
            if (textFile.AccessRule.AvailableAll) return true;

            // Если пользователь не авторизован, а доступ не открыт всем, запрещаем доступ
            if (user == null) return false;
            // Если пользователь не авторизован и является владельцем файла.
            if (textFile.OwnerId == user.Id) return true;

            // Если у пользователя не загружены группы, загружаем их
            if (user.Groups == null || user.GroupMemberships == null)
            {
                user = await _userRepository.GetAsync(user.Id, u => u.Groups, u => u.GroupMemberships);
                if (user == null) return null;
            }

            // id всех групп, к которым относится пользователь
            HashSet<int> userGroupsIds = new HashSet<int>(
                user.GroupMemberships
                    .Where(gm => gm.IsConfirmed)
                    .Select(gm => gm.GroupId)
                    .Concat(user.Groups.Select(g => g.GroupId))
            );

            // Проверяем, есть ли пользователь в списке разрешённых
            if (textFile.AccessRule.AvailableUsers.Any(u => u.Id == user.Id)) return true;

            // Проверяем, пересекаются ли группы пользователя с доступными группами
            if (textFile.AccessRule.AvailableGroups.Any(g => userGroupsIds.Contains(g.GroupId))) return true;

            return false;
        }

        public async Task<bool?> CheckShelfAccess(User? user, Shelf shelf)
        {
            // Если правило доступа не загружено, загружаем
            if (shelf.AccessRule == null ||
                shelf.AccessRule.AvailableGroups == null ||
                shelf.AccessRule.AvailableUsers == null)
            {
                shelf = await _shelfRepository.GetAsync(shelf.ShelfId,
                    s => s.AccessRule,
                    s => s.AccessRule.AvailableGroups,
                    s => s.AccessRule.AvailableUsers);

                if (shelf == null) return null;
            }

            // Если доступ разрешён всем — разрешаем для любого пользователя (в т.ч. null)
            if (shelf.AccessRule.AvailableAll) return true;

            // Если пользователь не авторизован, а доступ не открыт всем, запрещаем доступ
            if (user == null) return false;
            // Если пользователь авторизован и является владельцем полки.
            if (shelf.CreatorId == user.Id) return true;

            // Если у пользователя не загружены группы, загружаем их
            if (user.Groups == null || user.GroupMemberships == null)
            {
                user = await _userRepository.GetAsync(user.Id, u => u.Groups, u => u.GroupMemberships);
                if (user == null) return null;
            }

            // id всех групп, к которым относится пользователь
            HashSet<int> userGroupsIds = new HashSet<int>(
                user.GroupMemberships
                    .Where(gm => gm.IsConfirmed)
                    .Select(gm => gm.GroupId)
                    .Concat(user.Groups.Select(g => g.GroupId))
            );

            // Проверяем, есть ли пользователь в списке разрешённых
            if (shelf.AccessRule.AvailableUsers.Any(u => u.Id == user.Id)) return true;

            // Проверяем, пересекаются ли группы пользователя с доступными группами
            if (shelf.AccessRule.AvailableGroups.Any(g => userGroupsIds.Contains(g.GroupId))) return true;

            return false;
        }

        public async Task<List<Shelf>> AvailableShelvesForGroup(int groupId,
            params Expression<Func<Shelf, object>>[] includes)
        {
            return (await _shelfRepository.FindAsync(
                s => s.AccessRule.AvailableGroups.Any(g => g.GroupId == groupId),
                includes
            )).ToList();
        }

        public async Task<List<TextFile>> AvailableFilesForGroup(int groupId,
            params Expression<Func<TextFile, object>>[] includes)
        {
            return (await _textFileRepository.FindAsync(
                t => t.AccessRule.AvailableGroups.Any(g => g.GroupId == groupId),
                includes
            )).ToList();
        }

    }
}
