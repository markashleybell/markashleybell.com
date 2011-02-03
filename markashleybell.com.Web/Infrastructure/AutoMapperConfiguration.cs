using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using markashleybell.com.Web.Models;
using markashleybell.com.Domain.Entities;

namespace markashleybell.com.Web.Infrastructure
{
    public class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.Initialize(x =>
            {
                x.AddProfile<DomainToViewModelMappingProfile>();
                x.AddProfile<ViewModelToDomainMappingProfile>();
            });
        }

        public class DomainToViewModelMappingProfile : Profile
        {
            public override string ProfileName
            {
                get { return "DomainToViewModelMappings"; }
            }

            protected override void Configure()
            {
                Mapper.CreateMap<Article, ArticleViewModel>();
                Mapper.CreateMap<Article, ArticleDetailPageViewModel>();
                Mapper.CreateMap<Comment, CommentViewModel>();

                /*
                Mapper.CreateMap<Address, AddressViewModel>()
                    .ForMember(x => x.PersonAddressLineOne, opt => opt.MapFrom(source => source.FirstLine))
                    .ForMember(x => x.PersonCountryOfResidence, opt => opt.MapFrom(source => source.Country));

                Mapper.CreateMap<Note, NoteViewModel>();
                 */
            }
        }

        public class ViewModelToDomainMappingProfile : Profile
        {
            public override string ProfileName
            {
                get { return "ViewModelToDomainMappings"; }
            }

            protected override void Configure()
            {
                Mapper.CreateMap<ArticleViewModel, Article>();
                Mapper.CreateMap<ArticleDetailPageViewModel, Article>();
                Mapper.CreateMap<CommentViewModel, Comment>();

                /*
                Mapper.CreateMap<AddressViewModel, Address>()
                    .ForMember(x => x.FirstLine, opt => opt.MapFrom(source => source.PersonAddressLineOne))
                    .ForMember(x => x.Country, opt => opt.MapFrom(source => source.PersonCountryOfResidence));

                Mapper.CreateMap<NoteViewModel, Note>();
                 */
            }
        }

    }
}