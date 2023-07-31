namespace Cloudsume.Models;

using System.ComponentModel.DataAnnotations;
using Candidate.Globalization;

public sealed class CreateJob
{
    public CreateJob(TranslationCollection names)
    {
        this.Names = names;
    }

    [MinLength(1)]
    public TranslationCollection Names { get; }
}
