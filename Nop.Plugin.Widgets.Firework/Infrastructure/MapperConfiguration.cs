using AutoMapper;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.Widgets.Firework.Domain;
using Nop.Plugin.Widgets.Firework.Models;

namespace Nop.Plugin.Widgets.Firework.Infrastructure
{
    /// <summary>
    /// Represents AutoMapper configuration for plugin models
    /// </summary>
    public class MapperConfiguration : Profile, IOrderedMapperProfile
    {
        #region Ctor

        public MapperConfiguration()
        {
            CreateMap<FireworkEmbedWidget, EmbedWidgetModel>()
                .ForMember(model => model.AvailableStores, options => options.Ignore())
                .ForMember(model => model.AvailableWidgetZones, options => options.Ignore())
                .ForMember(model => model.AvailableLayoutTypes, options => options.Ignore())
                .ForMember(model => model.AvailableChannels, options => options.Ignore())
                .ForMember(model => model.AvailablePlaylists, options => options.Ignore())
                .ForMember(model => model.AvailableVideos, options => options.Ignore())
                .ForMember(model => model.WidgetZoneValue, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.CustomProperties, options => options.Ignore())
                .ForMember(model => model.SelectedStoreIds, options => options.Ignore());
            CreateMap<EmbedWidgetModel, FireworkEmbedWidget>()
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore());
        }

        #endregion

        #region Properties

        /// <summary>
        /// Order of this mapper implementation
        /// </summary>
        public int Order => 1;

        #endregion
    }
}