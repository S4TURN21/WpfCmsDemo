using Microsoft.Extensions.DependencyInjection;
using Remake.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remake.Wpf.Services
{
    public class ViewModelLocator
    {
        private readonly IServiceProvider _provider;

        public ViewModelLocator(IServiceProvider provider)
        {
            _provider = provider;
        }

        public MainViewModel MainViewModel => _provider.GetRequiredService<MainViewModel>();
        public LoginViewModel LoginViewModel => _provider.GetRequiredService<LoginViewModel>();
    }
}
