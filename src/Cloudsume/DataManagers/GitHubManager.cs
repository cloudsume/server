namespace Cloudsume.DataManagers;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cloudsume.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Domain = Candidate.Server.Resume.Data.GitHub;
using Update = Cloudsume.Server.Models.DataProperty<string?>;

internal sealed class GitHubManager : UniqueDataManager<Domain, Update>
{
    public GitHubManager(IOptions<JsonOptions> json)
        : base(json)
    {
    }

    public override async Task<SampleUpdate<Update>> ReadSampleUpdateAsync(Stream data, CancellationToken cancellationToken = default)
    {
        var dto = await this.ReadJsonAsync<SampleUpdate<Update>>(data, cancellationToken);

        this.SetValidators(dto.Update);

        return dto;
    }

    public override async Task<Update> ReadUpdateAsync(Stream data, CancellationToken cancellationToken = default)
    {
        var dto = await this.ReadJsonAsync<Update>(data, cancellationToken);

        this.SetValidators(dto);

        return dto;
    }

    public override Domain ToDomain(Update update, IReadOnlyDictionary<string, object> contents)
    {
        return new(update, DateTime.Now);
    }

    public override Update ToDto(Domain domain, IDataMappingServices services)
    {
        return new(domain.Username);
    }

    private void SetValidators(Update dto)
    {
        dto.AddValidator(new UsernameAttribute());
    }

    private sealed class UsernameAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is not string username)
            {
                return true;
            }

            switch (username.Length)
            {
                case 0:
                case > 39:
                    return false;
            }

            var hyphens = 0;

            for (var i = 0; i < username.Length; i++)
            {
                var c = username[i];

                if (c == '-')
                {
                    if (++hyphens > 1 || i == 0 || i == (username.Length - 1))
                    {
                        return false;
                    }
                }
                else if (!IsAlphanumeric(c))
                {
                    return false;
                }
                else
                {
                    hyphens = 0;
                }
            }

            return true;

            static bool IsAlphanumeric(char c)
            {
                var isNumeric = c >= 0x30 && c <= 0x39;
                var isLowerCaseAlphabetic = c >= 0x41 && c <= 0x5a;
                var isUpperCaseAlphabetic = c >= 0x61 && c <= 0x7a;

                return isNumeric || isLowerCaseAlphabetic || isUpperCaseAlphabetic;
            }
        }
    }
}
