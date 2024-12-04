using System.Linq.Expressions;
using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.UeUseCases.Create;

namespace UniversiteDomainUnitTests;

public class UeUnitTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task CreateUeUseCase()
    {
        long id = 1;
        string numeroUe = "UE101";
        string intitule = "Programmation Avancée";

        // On crée l'UE qui doit être ajoutée en base
        Ue ueSansId = new Ue { NumeroUe = numeroUe, Intitule = intitule };

        // Créons le mock du repository
        // On initialise une fausse datasource qui va simuler un IUeRepository
        var mock = new Mock<IUeRepository>();

        // Simulation de la fonction FindByCondition
        // On dit à ce mock que l'UE n'existe pas déjà
        var reponseFindByCondition = new List<Ue>();
        mock.Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>())).ReturnsAsync(reponseFindByCondition);

        // Simulation de la fonction Create
        // On lui dit que l'ajout d'une UE renvoie une UE avec l'Id 1
        Ue ueCree = new Ue { Id = id, NumeroUe = numeroUe, Intitule = intitule };
        mock.Setup(repoUe => repoUe.CreateAsync(ueSansId)).ReturnsAsync(ueCree);

        // On crée le bouchon (un faux UeRepository). Il est prêt à être utilisé
        var fauxUeRepository = mock.Object;

        // Création du use case en injectant notre faux repository
        CreateUeUseCase useCase = new CreateUeUseCase(fauxUeRepository);

        // Appel du use case
        var ueTestee = await useCase.ExecuteAsync(ueSansId);

        // Vérification du résultat
        Assert.That(ueTestee.Id, Is.EqualTo(ueCree.Id));
        Assert.That(ueTestee.NumeroUe, Is.EqualTo(ueCree.NumeroUe));
        Assert.That(ueTestee.Intitule, Is.EqualTo(ueCree.Intitule));
    }
}