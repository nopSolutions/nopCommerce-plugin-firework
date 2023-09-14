using FluentValidation;
using Nop.Plugin.Widgets.Firework.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Widgets.Firework.Validators
{
    /// <summary>
    /// Represents widget model validator
    /// </summary>
    public class EmbedWidgetValidator : BaseNopValidator<EmbedWidgetModel>
    {
        #region Ctor

        public EmbedWidgetValidator(ILocalizationService localizationService)
        {
            RuleFor(model => model.ChannelId)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Widgets.Firework.EmbedWidget.Fields.Channel.Required"));

            RuleFor(model => model.PlaylistId)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Widgets.Firework.EmbedWidget.Fields.Playlist.Required"));
        }

        #endregion
    }
}