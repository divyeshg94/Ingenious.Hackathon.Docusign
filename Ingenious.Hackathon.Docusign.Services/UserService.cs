using Ingenious.Hackathon.Docusign.Sql;
using Ingenious.Hackathon.Docusign.Sql.Models;

namespace Ingenious.Hackathon.Docusign.Services
{
    public class UserService
    {
        private readonly IRepository<Users> _userRepository;

        public UserService(IRepository<Users> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Users> Add(Users user)
        {
            if (user == null)
                throw new Exception("Invalid Input - User");

            var existingUser = await _userRepository.GetAsync(new RepositoryModel<Users>() { Where = u => u.UserUpn == user.UserUpn });
            if (existingUser != null)
            {
                existingUser.LastLoginTime = DateTime.UtcNow;
                await _userRepository.UpdateAsync(existingUser);
                return existingUser;
            }

            await _userRepository.AddAsync(user);
            return user;
        }
    }
}
