namespace Cloudsume.Cassandra;

using System;
using System.Globalization;
using Candidate.Server.Resume;
using Cloudsume.Resume;

internal abstract class ResumeDataMapper<TDomain, TCassandra> : IResumeDataMapper
    where TDomain : ResumeData
    where TCassandra : Models.DataObject
{
    public Type TargetData => typeof(TDomain);

    public abstract TCassandra ToCassandra(TDomain domain);

    public abstract TDomain ToDomain(Guid id, Guid? parent, TCassandra cassandra);

    GlobalData IResumeDataMapper.CreateGlobal(CultureInfo culture, ResumeData data) => new GlobalData<TDomain>(culture, (TDomain)data);

    Models.DataObject IResumeDataMapper.ToCassandra(ResumeData domain) => this.ToCassandra((TDomain)domain);

    ResumeData IResumeDataMapper.ToDomain(Guid id, Guid? parent, Models.DataObject cassandra) => this.ToDomain(id, parent, (TCassandra)cassandra);
}
