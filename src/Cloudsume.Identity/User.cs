namespace Cloudsume.Identity
{
    using System;
    using System.Collections.Generic;
    using System.Net.Mail;
    using System.Security.Claims;

    public sealed class User
    {
        public User(IEnumerable<Claim> claims)
        {
            Guid? id = null;
            string? username = null;
            MailAddress? email = null;
            bool? emailVerified = null;

            foreach (var claim in claims)
            {
                switch (claim.Type)
                {
                    case "sub":
                        id = Guid.Parse(claim.Value);
                        break;
                    case "name":
                        username = claim.Value;
                        break;
                    case "email":
                        email = new MailAddress(claim.Value);
                        break;
                    case "email_verified":
                        emailVerified = bool.Parse(claim.Value);
                        break;
                }
            }

            if (id == null)
            {
                throw new ArgumentException("The value does not contain 'sub'.", nameof(claims));
            }

            if (username == null)
            {
                throw new ArgumentException("The value does not contain 'name'.", nameof(claims));
            }

            if (email == null)
            {
                throw new ArgumentException("The value does not contain 'email'.", nameof(claims));
            }

            if (emailVerified == null)
            {
                throw new ArgumentException("The value does not contain 'email_verified'.", nameof(claims));
            }

            this.Id = id.Value;
            this.Username = username;
            this.Email = email;
            this.EmailVerified = emailVerified.Value;
        }

        public Guid Id { get; }

        public string Username { get; }

        public MailAddress Email { get; }

        public bool EmailVerified { get; }
    }
}
