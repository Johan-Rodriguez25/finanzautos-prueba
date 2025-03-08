using PublicationMicroservice.Modules.Publications.Domain;
using PublicationMicroservice.Modules.Publications.Domain.Entities;
using PublicationMicroservice.Modules.Publications.Domain.ValueObjects;

namespace PublicationMicroservice.Modules.Publications.GetPublicationById
{
  public class GetPublicationByIdUseCase
  {
    private readonly IPublicationRepository _publicationRepository;

    public GetPublicationByIdUseCase(IPublicationRepository publicationRepository)
    {
      _publicationRepository = publicationRepository;
    }

    public async Task<Publication?> Run(string id)
    {
      return await _publicationRepository.GetPublicationById(PublicationId.FromString(id));
    }
  }
}