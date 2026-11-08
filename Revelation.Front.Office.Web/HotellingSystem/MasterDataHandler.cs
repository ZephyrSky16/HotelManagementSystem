using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Revelation.Front.Office.Core.Interfaces.HotellingSystem;
using Revelation.Front.Office.Models.HotellingSystem;

namespace Revelation.Front.Office.Web.HotellingSystem
{
    public class MasterDataRow
    {
        public int Id { get; set; }
        public IDictionary<string, string> Values { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? InsertedDate { get; set; }
        public string InsertedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }

    /// <summary>Type-erased access to one lookup table so a single controller and view serve all of them.</summary>
    public interface IMasterDataHandler
    {
        IList<MasterDataRow> GetRows();

        /// <returns>An error message, or null on success.</returns>
        string Save(int? id, Func<string, string> readField, bool? isActive, string user);

        void SetActive(int id, bool active, string user);
        void Delete(int id);
    }

    public class MasterDataHandler<T> : IMasterDataHandler where T : HsEntity, new()
    {
        private readonly MasterDataDefinition _definition;
        private readonly IMasterDataService<T> _service;
        private readonly Dictionary<string, PropertyInfo> _properties;

        public MasterDataHandler(MasterDataDefinition definition, IMasterDataService<T> service)
        {
            _definition = definition;
            _service = service;
            _properties = definition.Fields.ToDictionary(f => f.Name, f => typeof(T).GetProperty(f.Name));
        }

        public IList<MasterDataRow> GetRows() =>
            _service.GetAll().Select(e => new MasterDataRow
            {
                Id = e.Id,
                Values = _definition.Fields.ToDictionary(f => f.Name, f => (string)_properties[f.Name].GetValue(e)),
                IsActive = (e as IActivatable)?.IsActive,
                InsertedDate = (e as IInsertAudited)?.InsertedDate,
                InsertedBy = (e as IInsertAudited)?.InsertedBy,
                UpdatedDate = (e as IUpdateAudited)?.UpdatedDate,
                UpdatedBy = (e as IUpdateAudited)?.UpdatedBy
            }).ToList();

        public string Save(int? id, Func<string, string> readField, bool? isActive, string user)
        {
            var values = new Dictionary<string, string>();
            foreach (var field in _definition.Fields)
            {
                var value = (readField(field.Name) ?? "").Trim();
                if (field.Required && value.Length == 0) { return field.Label + " is required."; }
                if (value.Length > field.MaxLength) { return field.Label + " must be " + field.MaxLength + " characters or fewer."; }
                values[field.Name] = value.Length == 0 ? null : value;
            }

            var entity = id.HasValue ? _service.GetById(id.Value) : new T();
            if (entity == null) { return _definition.Singular + " not found."; }

            foreach (var pair in values) { _properties[pair.Key].SetValue(entity, pair.Value); }
            if (entity is IActivatable activatable && isActive.HasValue) { activatable.IsActive = isActive.Value; }

            if (id.HasValue) { _service.Update(entity, user); } else { _service.Create(entity, user); }
            return null;
        }

        public void SetActive(int id, bool active, string user) => _service.SetActive(id, active, user);

        public void Delete(int id) => _service.Delete(id);
    }
}
