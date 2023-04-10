using FluentValidation;
using Nop.Plugin.Widgets.Firework.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Widgets.Firework.Validators
{
    /// <summary>
    /// Represents configuration model validator
    /// </summary>
    public class ConfigurationValidator : BaseNopValidator<ConfigurationModel>
    {
        #region Ctor

        public ConfigurationValidator(ILocalizationService localizationService)
        {
            RuleFor(model => model.ClientSecret)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Widgets.Firework.Configuration.Fields.ClientSecret.Required"))
                .When(model => !string.IsNullOrEmpty(model.ClientId));
        }

        #endregion
    }
}