using PublicationMicroservice.Modules.Publications.Domain.Entities;
using PublicationMicroservice.Modules.Publications.Domain.ValueObjects;
using PublicationMicroservice.Modules.Publications.Infrastructure.SqlServer.Entities;

namespace PublicationMicroservice.Modules.Publications.Infrastructure.SqlServer.Mappers
{
    public static class PublicationMapper
    {
        public static Publication MapToDomain(SqlServerPublicationEntity entity)
        {
            return new Publication(
                PublicationId.FromString(entity.Id),
                PublicationTitle.FromString(entity.Title),
                PublicationContent.FromString(entity.Content),
                entity.UserId,
                CreatedAt.FromDateTime(entity.CreatedAt),
                UpdatedAt.FromDateTime(entity.UpdatedAt)
            );
        }

        public static SqlServerPublicationEntity MapToEntity(Publication publication)
        {
            return new SqlServerPublicationEntity
            {
                Id = publication.Id.Value,
                Title = publication.Title.Value,
                Content = publication.Content.Value,
                UserId = publication.UserId,
                CreatedAt = publication.CreatedAt.Value,
                UpdatedAt = publication.UpdatedAt.Value
            };
        }
    }
}