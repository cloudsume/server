namespace Cloudsume.Cassandra.Tests;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Candidate.Server.Resume.Data;
using Cloudsume.Resume.Data;
using global::Cassandra;
using global::Cassandra.Mapping;
using Moq;
using Xunit;

public sealed class ResumeDataManagerTests
{
    private readonly IReadOnlyDictionary<string, ResumeDataManager> subjects;
    private readonly Mock<IMapper> db;

    public ResumeDataManagerTests()
    {
        this.db = new();
        this.subjects = ResumeDataManager.BuildTable(this.db.Object.GetType());
    }

    [Fact]
    public void NewDto_ForUniqueData_ShouldReturnCorrectInstance()
    {
        var subject = this.subjects[Name.StaticType];
        var result = subject.NewDto();

        Assert.IsType<Cloudsume.Cassandra.Models.ResumeName>(result);
    }

    [Fact]
    public void NewDto_ForMultiplicableData_ShouldReturnCorrectInstance()
    {
        var subject = this.subjects[Experience.StaticType];
        var result = subject.NewDto();

        Assert.IsType<Cloudsume.Cassandra.Models.ResumeExperience>(result);
    }

    [Fact]
    public void UpdateAsync_WhenInvoked_ShouldInvokeCorrectMapperMethod()
    {
        // Arrange.
        var subject = this.subjects[Address.StaticType];
        var row = new Cloudsume.Cassandra.Models.ResumeAddress();
        var options = CqlQueryOptions.New();
        var returns = new Task(() => { });

        this.db.Setup(m => m.UpdateAsync(row, options)).Returns(returns);

        // Act.
        var result = subject.UpdateAsync(this.db.Object, row, options);

        // Assert.
        Assert.Same(returns, result);

        this.db.Verify(m => m.UpdateAsync(row, options), Times.Once());
        this.db.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetAsync_ForUniqueData_ShouldInvokeCorrectStaticMethod()
    {
        // Arrange.
        var subject = this.subjects[EmailAddress.StaticType];
        var userId = Guid.NewGuid();
        var resumeId = Guid.NewGuid();
        var returns = new Cloudsume.Cassandra.Models.ResumeEmail();

        this.db
            .Setup(m => m.FirstOrDefaultAsync<Cloudsume.Cassandra.Models.ResumeEmail>(It.Is<Cql>(q => q.Statement == "WHERE user_id = ? AND resume_id = ? AND language = ?" && q.Arguments[0].Equals(userId) && q.Arguments[1].Equals(resumeId) && q.Arguments[2].Equals(string.Empty))))
            .ReturnsAsync(returns)
            .Verifiable();

        // Act.
        var result = await subject.GetAsync(this.db.Object, userId, resumeId, ConsistencyLevel.Any);

        // Assert.
        Assert.Same(returns, result);

        this.db.Verify();
        this.db.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetAsync_ForMultiplicableData_ShouldInvokeCorrectStaticMethod()
    {
        // Arrange.
        var subject = this.subjects[Education.StaticType];
        var userId = Guid.NewGuid();
        var resumeId = Guid.NewGuid();
        var returns = new[] { new Cloudsume.Cassandra.Models.ResumeEducation() };

        this.db
            .Setup(m => m.FetchAsync<Cloudsume.Cassandra.Models.ResumeEducation>(It.Is<Cql>(q => q.Statement == "WHERE user_id = ? AND resume_id = ? AND language = ? ORDER BY resume_id ASC, language ASC, position ASC" && q.Arguments[0].Equals(userId) && q.Arguments[1].Equals(resumeId) && q.Arguments[2].Equals(string.Empty))))
            .ReturnsAsync(returns)
            .Verifiable();

        // Act.
        var result = await subject.GetAsync(this.db.Object, userId, resumeId, ConsistencyLevel.Any);

        // Assert.
        Assert.Same(returns, result);

        this.db.Verify();
        this.db.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetAsync_ForGlobalData_ShouldInvokeCorrectStaticMethod()
    {
        // Arrange.
        var subject = this.subjects[GitHub.StaticType];
        var expects = Cql.New("WHERE x = ?", 0);
        var returns = new[] { new Cloudsume.Cassandra.Models.ResumeGitHub() };

        this.db
            .Setup(m => m.FetchAsync<Cloudsume.Cassandra.Models.ResumeGitHub>(expects))
            .ReturnsAsync(returns)
            .Verifiable();

        // Act.
        var result = await subject.GetAsync(this.db.Object, expects);

        // Assert.
        Assert.Same(returns, result);

        this.db.Verify();
        this.db.VerifyNoOtherCalls();
    }

    [Fact]
    public void DeleteAsync_WhenInvoked_ShouldInvokeCorrectMapperMethod()
    {
        // Arrange.
        var subject = this.subjects[Headline.StaticType];
        var row = new Cloudsume.Cassandra.Models.ResumeHeadline();
        var options = CqlQueryOptions.New();
        var returns = new Task(() => { });

        this.db
            .Setup(m => m.DeleteAsync(row, options))
            .Returns(returns)
            .Verifiable();

        // Act.
        var result = subject.DeleteAsync(this.db.Object, row, options);

        // Assert.
        Assert.Same(returns, result);

        this.db.Verify();
        this.db.VerifyNoOtherCalls();
    }

    [Fact]
    public void ClearAsync()
    {
        // Arrange.
        var subject = this.subjects[Language.StaticType];
        var userId = Guid.NewGuid();
        var resumeId = Guid.NewGuid();
        var returns = new Task(() => { });

        this.db
            .Setup(m => m.DeleteAsync<Cloudsume.Cassandra.Models.ResumeLanguage>("WHERE user_id = ? AND resume_id = ?", userId, resumeId))
            .Returns(returns)
            .Verifiable();

        // Act.
        var result = subject.ClearAsync(this.db.Object, userId, resumeId);

        // Assert.
        Assert.Same(returns, result);

        this.db.Verify();
        this.db.VerifyNoOtherCalls();
    }
}
