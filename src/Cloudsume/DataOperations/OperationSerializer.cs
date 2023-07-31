namespace Cloudsume.DataOperations
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
    using IDataManagerCollection = Cloudsume.Resume.IDataActionCollection<Cloudsume.IDataManager>;

    internal abstract class OperationSerializer
    {
        private readonly IDataManagerCollection managers;
        private readonly IActionContextAccessor contextAccessor;
        private readonly IObjectModelValidator validator;

        protected OperationSerializer(IDataManagerCollection managers, IActionContextAccessor contextAccessor, IObjectModelValidator validator)
        {
            this.managers = managers;
            this.contextAccessor = contextAccessor;
            this.validator = validator;
        }

        protected ModelStateDictionary ModelState => this.ActionContext.ModelState;

        private ActionContext ActionContext => this.contextAccessor.ActionContext ?? throw new InvalidOperationException("No action context available.");

        protected ParsedRequest<TDeletionId> ParseRequest<TDeletionId>(IFormCollection request, Func<string, TDeletionId> deletionId)
            where TDeletionId : struct, IComparable<TDeletionId>
        {
            var updates = new List<KeyValuePair<UpdateKey, object>>();
            var deletes = new SortedSet<DeleteKey<TDeletionId>>();
            var contents = new Dictionary<string, object>();

            foreach (var (name, value) in this.FlattenForm(request))
            {
                // Parse name.
                FormKey key;

                try
                {
                    key = FormKey.Parse(name, deletionId);
                }
                catch (ArgumentException ex)
                {
                    throw new RequestException(name, ex.Message, ex);
                }

                // Check type.
                if (key is UpdateKey u)
                {
                    updates.Add(new KeyValuePair<UpdateKey, object>(u, value));
                }
                else if (key is DeleteKey<TDeletionId> d)
                {
                    if (!deletes.Add(d))
                    {
                        throw new RequestException(name, "Duplicated delete.");
                    }
                }
                else if (key is ContentKey c)
                {
                    try
                    {
                        contents.Add(c.Id, value);
                    }
                    catch (ArgumentException ex)
                    {
                        throw new RequestException(name, "Duplicated content.", ex);
                    }
                }
                else
                {
                    throw new NotImplementedException($"{key.GetType()} is not implemented.");
                }
            }

            return new(updates, deletes, contents);
        }

        protected async Task<T?> DeserializeAsync<T>(UpdateKey key, object value, Func<IDataManager, UpdateKey, Stream, Task<T>> deserializer)
        {
            // Get the manager for target data.
            if (!this.managers.TryGetValue(key.Type, out var manager))
            {
                this.ModelState.AddModelError(key, "Unknow data type.");
                return default;
            }

            // Get entry data.
            if (value is not IFormFile file || !string.Equals(file.ContentType, "application/json", StringComparison.OrdinalIgnoreCase))
            {
                this.ModelState.AddModelError(key, "Invalid data.");
                return default;
            }

            await using var data = file.OpenReadStream();

            // Invoke deserializer.
            return await deserializer.Invoke(manager, key, data);
        }

        protected async Task<Candidate.Server.Resume.ResumeData?> DeserializeUpdateAsync(
            IDataManager manager,
            UpdateKey key,
            Stream data,
            IReadOnlyDictionary<string, object> contents,
            CancellationToken cancellationToken = default)
        {
            object dto;

            try
            {
                dto = await manager.ReadUpdateAsync(data, cancellationToken);
            }
            catch (DataUpdateException ex)
            {
                this.ModelState.AddModelError(key, ex.Message);
                return null;
            }

            return await this.ToDomainAsync(manager, key, dto, contents, cancellationToken);
        }

        protected async Task<Candidate.Server.Resume.ResumeData?> ToDomainAsync(
            IDataManager manager,
            UpdateKey key,
            object dto,
            IReadOnlyDictionary<string, object> contents,
            CancellationToken cancellationToken = default)
        {
            // Validate the DTO.
            // FIXME: Don't append property name of the model in to model state.
            this.validator.Validate(this.ActionContext, null, key, dto);

            if (!this.ModelState.IsValid)
            {
                return null;
            }

            // Convert the DTO to domain.
            try
            {
                return await manager.ToDomainAsync(dto, contents, cancellationToken);
            }
            catch (DataUpdateException ex)
            {
                this.ModelState.AddModelError(key, ex.Message);
                return null;
            }
        }

        private IEnumerable<(string Name, object Value)> FlattenForm(IFormCollection form)
        {
            foreach (var (name, values) in form)
            {
                foreach (var value in values)
                {
                    yield return (name, value);
                }
            }

            foreach (var part in form.Files)
            {
                yield return (part.Name, part);
            }
        }
    }
}
