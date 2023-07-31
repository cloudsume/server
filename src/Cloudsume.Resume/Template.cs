namespace Cloudsume.Resume;

using System;
using System.Collections.Generic;
using NetUlid;
using Ultima.Extensions.Collections;

public sealed class Template
{
    public Template(
        Ulid id,
        Guid registrationId,
        IEnumerable<string> applicableData,
        IKeyedByTypeCollection<TemplateRenderOptions> renderOptions,
        RegistrationCategory category,
        string releaseNote,
        long resumeCount)
    {
        if (resumeCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(resumeCount));
        }

        this.Id = id;
        this.RegistrationId = registrationId;
        this.ApplicableData = applicableData;
        this.RenderOptions = renderOptions;
        this.Category = category;
        this.ReleaseNote = releaseNote;
        this.ResumeCount = resumeCount;
    }

    public Ulid Id { get; }

    public Guid RegistrationId { get; }

    public IEnumerable<string> ApplicableData { get; }

    public IKeyedByTypeCollection<TemplateRenderOptions> RenderOptions { get; }

    public RegistrationCategory Category { get; }

    public string ReleaseNote { get; }

    public long ResumeCount { get; }
}
