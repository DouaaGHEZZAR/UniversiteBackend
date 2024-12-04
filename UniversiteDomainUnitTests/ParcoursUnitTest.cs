using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.EtudiantUseCases;
using UniversiteDomain.UseCases.ParcoursUseCases;
using UniversiteDomain.UseCases.ParcoursUseCases.Create;
using UniversiteDomain.UseCases.ParcoursUseCases.EtudiantDansParcours;
using UniversiteDomain.UseCases.ParcoursUseCases.UeDansParcours;

namespace UniversiteDomainUnitTests;

public class ParcoursUnitTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task CreateParcoursUseCase()
    {
        long idParcours = 1;
        String nomParcours = "Ue 1";
        int anneFormation = 2;
        
        // On crée le parcours qui doit être ajouté en base
        Parcours parcoursAvant = new Parcours{NomParcours = nomParcours, AnneeFormation = anneFormation};
        
        // On initialise une fausse datasource qui va simuler un EtudiantRepository
        var mockParcours = new Mock<IParcoursRepository>();
        
        // Il faut ensuite aller dans le use case pour simuler les appels des fonctions vers la datasource
        // Nous devons simuler FindByCondition et Create
        // On dit à ce mock que le parcours n'existe pas déjà
        mockParcours
            .Setup(repo=>repo.FindByConditionAsync(p=>p.Id.Equals(idParcours)))
            .ReturnsAsync((List<Parcours>)null);
        // On lui dit que l'ajout d'un étudiant renvoie un étudiant avec l'Id 1
        Parcours parcoursFinal =new Parcours{Id=idParcours,NomParcours= nomParcours, AnneeFormation = anneFormation};
        mockParcours.Setup(repo=>repo.CreateAsync(parcoursAvant)).ReturnsAsync(parcoursFinal);
        
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto=>facto.ParcoursRepository()).Returns(mockParcours.Object);
        
        // Création du use case en utilisant le mock comme datasource
        CreateParcoursUseCase useCase=new CreateParcoursUseCase(mockFactory.Object);
        
        // Appel du use case
        var parcoursTeste=await useCase.ExecuteAsync(parcoursAvant);
        
        // Vérification du résultat
        Assert.That(parcoursTeste.Id, Is.EqualTo(parcoursFinal.Id));
        Assert.That(parcoursTeste.NomParcours, Is.EqualTo(parcoursFinal.NomParcours));
        Assert.That(parcoursTeste.AnneeFormation, Is.EqualTo(parcoursFinal.AnneeFormation));
    }
    
    [Test]
    public async Task AddEtudiantDansParcoursUseCase()
    {
        long idEtudiant = 1;
        long idParcours = 3;
        Etudiant etudiant= new Etudiant { Id = 1, NumEtud = "1", Nom = "nom1", Prenom = "prenom1", Email = "1" };
        Parcours parcours = new Parcours{Id=3, NomParcours = "Ue 3", AnneeFormation = 1};
        
        // On initialise des faux repositories
        var mockEtudiant = new Mock<IEtudiantRepository>();
        var mockParcours = new Mock<IParcoursRepository>();
        List<Etudiant> etudiants = new List<Etudiant>();
        etudiants.Add(new Etudiant{Id=1});
        mockEtudiant
            .Setup(repo=>repo.FindByConditionAsync(e=>e.Id.Equals(idEtudiant)))
            .ReturnsAsync(etudiants);

        List<Parcours> parcourses = new List<Parcours>();
        parcourses.Add(parcours);
        
        List<Parcours> parcoursFinaux = new List<Parcours>();
        Parcours parcoursFinal = parcours;
        parcoursFinal.Inscrits.Add(etudiant);
        parcoursFinaux.Add(parcours);
        
        mockParcours
            .Setup(repo=>repo.FindByConditionAsync(e=>e.Id.Equals(idParcours)))
            .ReturnsAsync(parcourses);
        mockParcours
            .Setup(repo => repo.AddEtudiantAsync(idParcours, idEtudiant))
            .ReturnsAsync(parcoursFinal);
        
        // Création d'une fausse factory qui contient les faux repositories
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto=>facto.EtudiantRepository()).Returns(mockEtudiant.Object);
        mockFactory.Setup(facto=>facto.ParcoursRepository()).Returns(mockParcours.Object);
        
        // Création du use case en utilisant le mock comme datasource
        AddEtudiantDansParcoursUseCase useCase=new AddEtudiantDansParcoursUseCase(mockFactory.Object);
        
        // Appel du use case
        var parcoursTest=await useCase.ExecuteAsync(idParcours, idEtudiant);
        // Vérification du résultat
        Assert.That(parcoursTest.Id, Is.EqualTo(parcoursFinal.Id));
        Assert.That(parcoursTest.Inscrits, Is.Not.Null);
        Assert.That(parcoursTest.Inscrits.Count, Is.EqualTo(1));
        Assert.That(parcoursTest.Inscrits[0].Id, Is.EqualTo(idEtudiant));
    }
    
    [Test]
public async Task AddUeDansParcoursUseCase()
{
    // Données de test
    long idUe = 1;
    long idParcours = 3;

    // Création d'un parcours et d'une UE
    Ue ue = new Ue { Id = idUe, NumeroUe = "UE101", Intitule = "Programmation Avancée" };
    Parcours parcours = new Parcours
    {
        Id = idParcours,
        NomParcours = "Parcours Informatique",
        AnneeFormation = 1,
        UesEnseignees = new List<Ue>() // Assurez que la liste est vide
    };

    // Simulation des faux repositories
    var mockUeRepository = new Mock<IUeRepository>();
    var mockParcoursRepository = new Mock<IParcoursRepository>();

    // Simuler la recherche d'une UE existante
    mockUeRepository
        .Setup(repo => repo.FindByConditionAsync(u => u.Id.Equals(idUe)))
        .ReturnsAsync(new List<Ue> { ue });

    // Simuler la recherche d'un parcours existant
    mockParcoursRepository
        .Setup(repo => repo.FindByConditionAsync(p => p.Id.Equals(idParcours)))
        .ReturnsAsync(new List<Parcours> { parcours });

    // Simuler l'ajout d'une UE dans le parcours
    Parcours parcoursFinal = new Parcours
    {
        Id = idParcours,
        NomParcours = parcours.NomParcours,
        AnneeFormation = parcours.AnneeFormation,
        UesEnseignees = new List<Ue> { ue } // L'UE est ajoutée ici
    };

    mockParcoursRepository
        .Setup(repo => repo.AddUeAsync(idParcours, idUe))
        .ReturnsAsync(parcoursFinal);

    // Création d'une fausse factory contenant les faux repositories
    var mockFactory = new Mock<IRepositoryFactory>();
    mockFactory.Setup(f => f.UeRepository()).Returns(mockUeRepository.Object);
    mockFactory.Setup(f => f.ParcoursRepository()).Returns(mockParcoursRepository.Object);

    // Création du use case
    var useCase = new AddUeDansParcoursUseCase(mockFactory.Object);

    // Appel du use case
    var parcoursTest = await useCase.ExecuteAsync(idParcours, idUe);

    // Vérification des résultats
    Assert.That(parcoursTest.Id, Is.EqualTo(parcoursFinal.Id));
    Assert.That(parcoursTest.UesEnseignees, Is.Not.Null);
    Assert.That(parcoursTest.UesEnseignees.Count, Is.EqualTo(1));
    Assert.That(parcoursTest.UesEnseignees[0].Id, Is.EqualTo(idUe));
    Assert.That(parcoursTest.UesEnseignees[0].NumeroUe, Is.EqualTo(ue.NumeroUe));
}

}