namespace Cloudsume.Cassandra.ResumeDataMappers;

using System;
using System.Net.Mail;
using Cloudsume.Cassandra.Models;
using Domain = Candidate.Server.Resume.Data.EmailAddress;

internal sealed class EmailMapper : ResumeDataMapper<Domain, EmailData>
{
    public override EmailData ToCassandra(Domain domain) => new()
    {
        Address = TextProperty.From(domain.Value, v => v?.Address),
        UpdatedTime = domain.UpdatedAt,
    };

    public override Domain ToDomain(Guid id, Guid? parent, EmailData cassandra)
    {
        var address = cassandra.Address.ToDomain(v =>
        {
            if (v == null)
            {
                return null;
            }

            try
            {
                return new MailAddress(v);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is FormatException)
            {
                throw new ArgumentException($"Property '{nameof(cassandra.Address)}' contains invalid email address.", nameof(cassandra), ex);
            }
        });

        return new(address, cassandra.UpdatedTime.LocalDateTime);
    }
}
