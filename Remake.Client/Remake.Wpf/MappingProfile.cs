using AutoMapper;
using Remake.Wpf.Services;
using Remake.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remake.Wpf
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Employee, EditEmployeeViewModel>().ReverseMap();
            CreateMap<Employee, AddEmployeeViewModel>().ReverseMap();
        }
    }
}
