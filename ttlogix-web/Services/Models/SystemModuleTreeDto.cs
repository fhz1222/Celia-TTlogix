using System.Collections.Generic;
using TT.Core.Entities;

namespace TT.Services.Models
{
    public class SystemModuleTreeDto
    {
        public string Code { get; set; }
        public string ModuleName { get; set; }
        public string ShortName { get; set; }
        public IEnumerable<SystemModuleTreeDto> Children { get; set; }
        public bool IsChecked { get; set; }
        public SystemModuleTreeDto Parent { get; set; }
        public SystemModuleTreeDto()
        {
            /*Children = Enumerable.Empty<SystemModuleTreeDto>();*/
        }
        public SystemModuleTreeDto(SystemModule systemModule, SystemModuleTreeDto parent, bool isChecked)
        {
            Code = systemModule.Code;
            ModuleName = systemModule.ModuleName;
            ShortName = systemModule.ShortName;
            Parent = parent;
            IsChecked = isChecked;
        }
    }
}
