using UserMicroservice.Modules.Users.Domain.ValueObjects;

namespace UserMicroservice.Modules.Users.Domain.Entities
{
    public class User
    {
        public UserId Id { get; private set; }
        public UserName Name { get; private set; }
        public UserEmail Email { get; private set; }
        public PasswordHash Password { get; private set; }
        public CreatedAt CreatedAt { get; private set; }

        public User(UserId id, UserName name, UserEmail email, PasswordHash password, CreatedAt createdAt)
        {
            Id = id;
            Name = name;
            Email = email;
            Password = password;
            CreatedAt = createdAt;
        }

        public void UpdateInfo(UserName? name = null, UserEmail? email = null, PasswordHash? password = null)
        {
            if (name != null)
                Name = name;

            if (email != null)
                Email = email;

            if (password != null)
                Password = password;
        }

        protected User() { }
    }
}
